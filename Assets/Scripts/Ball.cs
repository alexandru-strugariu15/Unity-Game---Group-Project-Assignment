using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [Header("Bounds & Speed")]
    public float minY = -5.5f;
    public float launchSpeed = 10f;
    public float maxVelocity = 15.0f;
    [Header("Angle Control")]
    [Tooltip("Unghiul minim (în grade) față de orizontală; bila va fi corectată dacă devine prea orizontală.")]
    public float minVerticalAngle = 15f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject[] livesImage;
    public GameObject gameOverPanel;
    public GameObject youWinPanel;

    [Header("References")]
    public Transform paddleTransform;
    public PlayerMovement paddleMovement;

    private Rigidbody2D rb;
    private int score = 0;
    private int lives = 3;
    private int brickCount;
    private bool isAttached = true;
    private Vector3 paddleAttachOffset;
    private Vector3 initialPaddlePosition;
    private Vector3 initialBallPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (paddleMovement == null)
            paddleMovement = FindObjectOfType<PlayerMovement>();
        if (paddleTransform == null && paddleMovement != null)
            paddleTransform = paddleMovement.transform;

        if (paddleMovement != null)
            initialPaddlePosition = paddleMovement.StartPosition;
        else if (paddleTransform != null)
            initialPaddlePosition = paddleTransform.position;

        if (paddleTransform != null)
            paddleAttachOffset = transform.position - paddleTransform.position;
        else
            paddleAttachOffset = Vector3.zero;

        initialBallPosition = paddleTransform != null ? initialPaddlePosition + paddleAttachOffset : transform.position;

        AttachToPaddle();

        LevelGenerator generator = FindObjectOfType<LevelGenerator>();
        if (generator != null)
            brickCount = generator.transform.childCount;
        UpdateScoreUI();
    }

    void Update()
    {
        if (isAttached && paddleTransform != null)
        {
            FollowPaddle();
            if (Input.GetKeyDown(KeyCode.Space))
                LaunchFromPaddle();
        }

        if (!isAttached && transform.position.y < minY)
            LoseLifeAndReset();

        if (!isAttached && rb.linearVelocity.magnitude > maxVelocity)
            rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxVelocity);

        if (!isAttached)
            ClampShallowAngles();
    }

    private void FollowPaddle()
    {
        transform.position = paddleTransform.position + paddleAttachOffset;
        rb.linearVelocity = Vector2.zero;
    }

    private void LaunchFromPaddle()
    {
        isAttached = false;
        Vector2 launchDirection = new Vector2(1f, 1f).normalized;
        rb.linearVelocity = launchDirection * launchSpeed;
    }

    private void LoseLifeAndReset()
    {
        lives--;
        if (livesImage != null && lives >= 0 && lives < livesImage.Length)
            livesImage[lives].SetActive(false);

        if (lives <= 0)
        {
            GameOver();
            return;
        }

        ResetPositions();
    }

    private void ResetPositions()
    {
        if (paddleMovement != null)
        {
            paddleMovement.ResetToStart();
            initialPaddlePosition = paddleMovement.StartPosition;
        }
        else if (paddleTransform != null)
        {
            paddleTransform.position = initialPaddlePosition;
        }

        transform.position = paddleTransform != null ? paddleTransform.position + paddleAttachOffset : initialBallPosition;
        AttachToPaddle();
    }

    private void AttachToPaddle()
    {
        isAttached = true;
        rb.linearVelocity = Vector2.zero;
    }

    public bool IsAttached => isAttached;

    private void ClampShallowAngles()
    {
        Vector2 velocity = rb.linearVelocity;
        float speed = velocity.magnitude;
        if (speed < 0.0001f)
            return;

        float minSin = Mathf.Sin(minVerticalAngle * Mathf.Deg2Rad);
        float ny = Mathf.Abs(velocity.y) / speed;
        if (ny >= minSin)
            return;

        float signY = Mathf.Sign(velocity.y != 0 ? velocity.y : 1f);
        float newVy = minSin * speed * signY;
        float newVx = Mathf.Sign(velocity.x) * Mathf.Sqrt(Mathf.Max(0f, speed * speed - newVy * newVy));
        rb.linearVelocity = new Vector2(newVx, newVy);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            score += 1;
            UpdateScoreUI();
            brickCount--;
            if (brickCount <= 0)
            {
                if (youWinPanel != null)
                    youWinPanel.SetActive(true);
                Time.timeScale = 0;
            }
            return;
        }

        if (collision.gameObject.CompareTag("Paddle"))
        {
            HandlePaddleBounce(collision);
            return;
        }
    }

    private void HandlePaddleBounce(Collision2D collision)
    {
        if (collision.contactCount == 0)
            return;

        ContactPoint2D contact = collision.GetContact(0);
        Vector2 velocity = rb.linearVelocity;
        float speed = Mathf.Max(launchSpeed, velocity.magnitude);

        if (Mathf.Abs(contact.normal.x) > Mathf.Abs(contact.normal.y))
        {
            velocity.x = -velocity.x;
            rb.linearVelocity = velocity.normalized * speed;
            return;
        }

        float paddleWidth = collision.collider.bounds.size.x;
        float hitOffset = contact.point.x - collision.transform.position.x;
        float normalizedOffset = Mathf.Clamp(hitOffset / (paddleWidth * 0.5f), -1f, 1f);

        Vector2 newDirection = new Vector2(normalizedOffset, 1f).normalized;
        rb.linearVelocity = newDirection * speed;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString("00000");
    }

    private void GameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        Destroy(gameObject);
    }
}

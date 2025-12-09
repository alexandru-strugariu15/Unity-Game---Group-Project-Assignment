using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5.0f;
    public float maxX = 20.0f;

    private float movementHorizontal;
    private Vector3 startPosition;
    private Ball ball;

    private void Awake()
    {
        startPosition = transform.position;
        ball = FindObjectOfType<Ball>();
    }

    void Update()
    {
        if (ball != null && ball.IsAttached)
            return;

        movementHorizontal = Input.GetAxis("Horizontal");

        if ((movementHorizontal > 0 && transform.position.x < maxX) || (movementHorizontal < 0 && transform.position.x > -maxX))
        {
            transform.position += Vector3.right * movementHorizontal * speed * Time.deltaTime;
        }
    }

    public Vector3 StartPosition => startPosition;

    public void ResetToStart()
    {
        transform.position = startPosition;
    }
}

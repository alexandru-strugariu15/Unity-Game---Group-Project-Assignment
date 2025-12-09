using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class StartButtonController : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Image targetImage;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;
    [SerializeField] private string breakoutSceneName = "BreakoutScene";

    private Button button;
    private bool subscribed;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (targetImage == null)
            targetImage = GetComponent<Image>();

        if (gridManager == null)
            gridManager = FindFirstObjectByType<GridManager>();

        button.onClick.AddListener(OnStartClicked);
    }

    private void OnEnable()
    {
        Subscribe(); // Primeste validarea de la GridManager ca nava respecta constrangerile
        Refresh(gridManager != null && gridManager.HasValidLayout());
    }

    private void OnDisable()
    {
        Unsubscribe();
    }

    public void AttachGridManager(GridManager manager)
    {
        gridManager = manager;
        Subscribe();
        Refresh(gridManager != null && gridManager.HasValidLayout());
    }

    private void Subscribe()
    {
        if (gridManager != null && !subscribed)
        {
            gridManager.PlacedBlocksChanged += OnPlacedBlocksChanged;
            subscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (gridManager != null && subscribed)
        {
            gridManager.PlacedBlocksChanged -= OnPlacedBlocksChanged;
            subscribed = false;
        }
    }

    private void OnPlacedBlocksChanged(int _, bool hasValidLayout)
    {
        Refresh(hasValidLayout);
    }

    private void Refresh(bool unlocked)
    {
        if (button != null)
            button.interactable = unlocked; // daca e totul ok putem trece la joc
        if (targetImage != null)
            targetImage.color = unlocked ? unlockedColor : lockedColor;
    }

    private void OnStartClicked()
    {
        if (gridManager == null)
            return;

        if (!gridManager.HasValidLayout())
            return;

        SaveLayoutForNextScene(gridManager.GetPlacedBlockPositions());
        if (!string.IsNullOrEmpty(breakoutSceneName))
            SceneManager.LoadScene(breakoutSceneName);
    }

    private void SaveLayoutForNextScene(List<Vector2Int> cells)
    {
        LayoutCarrier.SaveLayout(cells);
    }
}

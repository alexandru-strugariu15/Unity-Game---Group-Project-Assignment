using UnityEngine;
using UnityEngine.EventSystems;

public class PlacedBlockRemover : MonoBehaviour, IPointerClickHandler
{
    private GridManager gridManager;
    private Vector2Int gridPosition;

    public void Init(GridManager manager, Vector2Int position)
    {
        gridManager = manager;
        gridPosition = position;
    }

    // Sterge un block cand se apasa click-dreapta
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (gridManager != null && gridManager.TryRemoveBlock(gridPosition))
        {
            Destroy(gameObject);
        }
    }
}

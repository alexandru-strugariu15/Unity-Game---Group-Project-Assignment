using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DropZone : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    // Dimensiunea vizuala a unei singure celule (Setata in Inspector)
    public float cellSize = 80f; 
    
    private Image cellImage;
    private GridManager gridManager;
    private Color originalColor = Color.blue;

    void Awake()
    {
        cellImage = GetComponent<Image>();
        gridManager = FindFirstObjectByType<GridManager>(); 
        
        if (cellImage != null)
        {
            originalColor = cellImage.color;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem droppedItem = eventData.pointerDrag.GetComponent<DraggableItem>();

        if (droppedItem != null && gridManager != null)
        {
            GridManager.PlacedBlock newBlock = new GridManager.PlacedBlock();
            newBlock.position = gridManager.GetGridIndex(this);
            newBlock.type = droppedItem.blockType;
            
            List<Vector2Int> occupiedCells;

            // 1. Verificam constrangerile logice in GridManager
            if (gridManager.CheckPlacementConstraints(newBlock, droppedItem.size, out occupiedCells))
            {
                gridManager.PlaceBlock(newBlock, occupiedCells);
                
                // Setam obiectul tras ca fiind copilul celulei de baz��
                droppedItem.transform.SetParent(transform);
                droppedItem.rectTransform.anchoredPosition = Vector2.zero;
                
                // Ajustarea dimensiunii Vizuale
                droppedItem.rectTransform.sizeDelta = new Vector2(
                    cellSize * droppedItem.size.x, 
                    cellSize * droppedItem.size.y
                );

                // Opacitate
                CanvasGroup droppedCanvasGroup = droppedItem.GetComponent<CanvasGroup>();
                if (droppedCanvasGroup == null)
                    droppedCanvasGroup = droppedItem.gameObject.AddComponent<CanvasGroup>();
                    
                droppedCanvasGroup.alpha = 1f; // Complet opac (CerinE>a 2)
                droppedCanvasGroup.blocksRaycasts = true;

                // Stergere cu right click
                PlacedBlockRemover remover = droppedItem.gameObject.GetComponent<PlacedBlockRemover>();
                if (remover == null)
                    remover = droppedItem.gameObject.AddComponent<PlacedBlockRemover>();
                remover.Init(gridManager, newBlock.position);
                
                droppedItem.transform.SetAsLastSibling(); // Se plaseaza deasupra patratelor albastre
                Destroy(droppedItem);
            }
            else
            {
                // nu respecta constrangerile
                Destroy(droppedItem.gameObject);
            }
        }
        cellImage.color = originalColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            cellImage.color = Color.yellow; 
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cellImage.color = originalColor;
    }
}

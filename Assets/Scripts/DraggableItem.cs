using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // dimnesiunea blocului
    public Vector2Int size = new Vector2Int(1, 1);
    
    public GridManager.BlockType blockType = GridManager.BlockType.SolidBlock;

    private Transform originalParent;
    
    public RectTransform rectTransform; 
    
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Clonam obiectul tras pentru a lasa originalul in panoul sursa
        GameObject dragInstance = Instantiate(gameObject, transform.position, Quaternion.identity, canvas.transform);
        DraggableItem dragItem = dragInstance.GetComponent<DraggableItem>();
        
        eventData.pointerDrag = dragInstance; 

        dragItem.originalParent = transform.parent; 
        dragItem.transform.SetParent(canvas.transform); 
        
        // Dezactivam Raycast Blocking si setam opacitatea pentru feedback vizual
        dragItem.canvasGroup.blocksRaycasts = false; 
        dragItem.canvasGroup.alpha = 0.6f; 
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Mutam clona sub cursorul mouse-ului
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Daca parintele este Canvas (nu a fost plasat pe o zona DropZone valida), distrugem clona.
        if (transform.parent == canvas.transform)
        {
            Destroy(gameObject); 
        }
        // Daca a fost plasat, DropZone.cs se ocupa de el (setare parinte, distrugere script)
    }
}
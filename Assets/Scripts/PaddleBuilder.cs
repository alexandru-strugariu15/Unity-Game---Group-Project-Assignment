using System.Collections.Generic;
using UnityEngine;
public class PaddleBuilder : MonoBehaviour
{
    public GameObject blockPrefab;   // Sprite pentru paleta
    public float cellSize = 0.4f;    // Scalare fata de layout-ul initial
    public Vector2 offset = Vector2.zero; // Pozitia de start in scena

    void Start() // Se apeleaza o singura data inainte de Update()
    {
        IReadOnlyList<Vector2Int> layout = LayoutCarrier.GetLayout();
        if (layout == null || layout.Count == 0)
            return;

        // Centreaza forma la origine
        Vector2 avg = Vector2.zero;
        foreach (var c in layout) avg += (Vector2)c;
        avg /= layout.Count;

        // Construieste nava 
        foreach (var cell in layout)
        {
            Vector2 pos = ((Vector2)cell - avg) * cellSize + offset;
            Instantiate(blockPrefab, pos, Quaternion.identity, transform);
        }
    }

    void Update()
    {
        
    }
}

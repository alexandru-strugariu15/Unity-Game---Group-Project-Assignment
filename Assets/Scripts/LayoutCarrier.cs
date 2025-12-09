using System.Collections.Generic;
using UnityEngine;


public static class LayoutCarrier
{
    private static List<Vector2Int> layout = new List<Vector2Int>();

    public static void SaveLayout(IEnumerable<Vector2Int> cells)
    {
        layout = new List<Vector2Int>(cells);
    }

    public static IReadOnlyList<Vector2Int> GetLayout()
    {
        return layout;
    }
}

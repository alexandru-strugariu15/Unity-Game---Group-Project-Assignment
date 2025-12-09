using System.Collections.Generic;
using UnityEngine;

public class GreenTokenBar : MonoBehaviour
{
    private readonly List<GameObject> tokens = new List<GameObject>();
    private int activeCount;

    private void Awake()
    {
        Rebuild();
    }

    private void Rebuild()
    {
        tokens.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            tokens.Add(transform.GetChild(i).gameObject);
        }
        activeCount = tokens.Count;
    }

    public void ConsumeOne()
    {
        if (tokens.Count == 0)
            return;

        if (activeCount <= 0)
            return;

        activeCount--;
        tokens[activeCount].SetActive(false); // Ascundem cate un patratel de la dreapta la stanga
    }

    public void RestoreOne()
    {
        if (tokens.Count == 0)
            return;

        if (activeCount >= tokens.Count)
            return;

        tokens[activeCount].SetActive(true); // Afisam din nou, daca obiectul a fost sters din grid
        activeCount++;
    }
}

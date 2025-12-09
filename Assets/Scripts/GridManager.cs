using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;

public class GridManager : MonoBehaviour
{
    public enum BlockType { None, SolidBlock, Cannon, Bumper, Motor }

    // Dimensiunile gridului
    public int COLS = 10;
    public int ROWS = 6;

    private BlockType[,] gridState; 
    [SerializeField] private GreenTokenBar greenTokenBar;
    public Action<int, bool> PlacedBlocksChanged;

    [SerializeField] 
    public List<PlacedBlock> placedBlocks = new List<PlacedBlock>();

    public struct PlacedBlock
    {
        public Vector2Int position;
        public BlockType type;
    }

    void Awake()
    {
        gridState = new BlockType[COLS, ROWS]; 
        if (greenTokenBar == null)
            greenTokenBar = FindFirstObjectByType<GreenTokenBar>();
        if (greenTokenBar == null)
        {
            GameObject barObj = GameObject.Find("ButoaneVerzi");
            if (barObj != null)
                greenTokenBar = barObj.GetComponent<GreenTokenBar>() ?? barObj.AddComponent<GreenTokenBar>();
        }

        AttachStartButtonController();
    }

    void Start()
    {
        // Se asigura ca gridul este gol la pornirea jocului
        for (int x = 0; x < COLS; x++)
        {
            for (int y = 0; y < ROWS; y++)
            {
                gridState[x, y] = BlockType.None;
            }
        }
        placedBlocks.Clear(); 
        NotifyPlacedBlocksChanged();
    }

    public Vector2Int GetGridIndex(DropZone zone)
    {
        int index = zone.transform.GetSiblingIndex();
        int x = index % COLS;
        int y = index / COLS;
        return new Vector2Int(x, y);
    }
    
    private bool IsInBounds(int x, int y)
    {
        return x >= 0 && x < COLS && y >= 0 && y < ROWS;
    }

    // Constrangeri
    public bool CheckPlacementConstraints(PlacedBlock newBlock, Vector2Int requiredSize, out List<Vector2Int> occupiedCells)
    {
        occupiedCells = new List<Vector2Int>();
        
        // 1. Constrangerea: Nu depasi Grid-ul si nu te suprapune

        Vector2Int currentPos = newBlock.position;
        
        if (!IsInBounds(currentPos.x, currentPos.y)) 
        { 
            Debug.LogError("Constrangere 1 (Depasire Grid) incalcata."); 
            return false; 
        }
        
        if (gridState[currentPos.x, currentPos.y] != BlockType.None) 
        { 
            Debug.LogError("Constrangere 1 (Suprapunere) incalcata."); 
            return false; 
        }
        occupiedCells.Add(currentPos);
        placedBlocks.Add(newBlock);
        
        // 2. Constrangerea: Max 10 Obiecte
        if (placedBlocks.Count > 10) 
        { 
            Debug.LogError("Constrangere 2 (Max 10) incalcata."); 
            placedBlocks.Remove(newBlock); 
            return false; 
        }

        // 3. Constrangerea: Nava sa fie conexa
        bool isFirstBlock = placedBlocks.Count == 1;
        
        if (!isFirstBlock && !IsVehicleConnected()) 
        { 
            Debug.LogError("Constrangere 3 (Conexitate) incalcata."); 
            placedBlocks.Remove(newBlock); 
            return false; 
        }
        
        // Toate constrangerile sunt indeplinite
        return true;
    }

    public void PlaceBlock(PlacedBlock newBlock, List<Vector2Int> cells)
    {
        // Actualizam gridState (Doar pentru 1x1)
        gridState[newBlock.position.x, newBlock.position.y] = newBlock.type;
        greenTokenBar?.ConsumeOne();
        NotifyPlacedBlocksChanged();
    }

    public bool TryRemoveBlock(Vector2Int position)
    {
        if (!IsInBounds(position.x, position.y))
            return false;

        if (gridState[position.x, position.y] == BlockType.None)
            return false;

        gridState[position.x, position.y] = BlockType.None;
        placedBlocks.RemoveAll(b => b.position == position);
        greenTokenBar?.RestoreOne();
        NotifyPlacedBlocksChanged();
        return true;
    }
    
    // BFS pentru conexitate
    private bool IsVehicleConnected()
    {
        if (placedBlocks.Count <= 1) return true; 

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Vector2Int startPos = placedBlocks[0].position;
        queue.Enqueue(startPos);
        visited.Add(startPos);
        HashSet<Vector2Int> occupiedPositions = new HashSet<Vector2Int>(placedBlocks.Select(b => b.position));
        Vector2Int[] directions = { new Vector2Int(0, 1), new Vector2Int(0, -1), new Vector2Int(-1, 0), new Vector2Int(1, 0) };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (var dir in directions)
            {
                Vector2Int neighbor = current + dir;
                if (occupiedPositions.Contains(neighbor) && !visited.Contains(neighbor)) 
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        return visited.Count == placedBlocks.Count;
    }

    public List<Vector2Int> GetPlacedBlockPositions()
    {
        return placedBlocks.Select(b => b.position).ToList();
    }

    public bool HasValidLayout()
    {
        return placedBlocks.Count > 0 && IsVehicleConnected();
    }

    private void NotifyPlacedBlocksChanged()
    {
        PlacedBlocksChanged?.Invoke(placedBlocks.Count, HasValidLayout());
    }

    private void AttachStartButtonController()
    {
        Button startButton = GameObject.Find("Button")?.GetComponent<Button>();
        if (startButton == null)
            return;

        StartButtonController controller = startButton.GetComponent<StartButtonController>();
        if (controller == null)
            controller = startButton.gameObject.AddComponent<StartButtonController>();

        controller.AttachGridManager(this);
    }
}

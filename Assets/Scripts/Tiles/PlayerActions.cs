using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerActions : MonoBehaviour
{
    
    public bool IsUsingProtest { get; private set; }
    
    [SerializeField] private float zOffset = 0.75f;
    [SerializeField] private Color darkenedTilesColor; // 9DB1DA
    [SerializeField] private TileBase greenProtestorsTile;
    
    private Tilemap _map;
    private Grid _grid;
    
    private Vector3Int _selectingActionTile;

    void Start()
    {
        _map = GetComponent<Tilemap>();
        _grid = _map.layoutGrid;
    }

    public void SelectProtestButton()
    {
        Debug.Log("Testing button");

        IsUsingProtest = true;
        GetFirstCenteredCell(greenProtestorsTile);
    }

    private void GetFirstCenteredCell(TileBase actionTile)
    {
        BoundsInt bounds = _map.cellBounds;
        for (int x = 0; x >= bounds.min.x; x--)
        {
            for (int y = 0; y >= bounds.min.y; y--)
            {
                Debug.Log("Primeiro");
                if (GetFirstCenteredCellWhenFound(actionTile, x, y)) return;
            }
        }

        for (int x = 0; x < bounds.max.x; x++)
        {
            for (int y = 0; y < bounds.min.y; y++)
            {
                Debug.Log("Segundo");
                if (GetFirstCenteredCellWhenFound(actionTile, x, y)) return;
            }
        }
        
        for (int x = 0; x < bounds.max.x; x++)
        {
            for (int y = 0; y >= bounds.min.y; y--)
            {
                Debug.Log("Terceiro");
                if (GetFirstCenteredCellWhenFound(actionTile, x, y)) return;
            }
        }
        
        for (int x = 0; x >= bounds.min.x; x--)
        {
            for (int y = 0; y < bounds.min.y; y++)
            {
                Debug.Log("Cuarto");
                if (GetFirstCenteredCellWhenFound(actionTile, x, y)) return;
            }
        }
    }

    private bool GetFirstCenteredCellWhenFound(TileBase actionTile, int x, int y)
    {
        var cellPosition = new Vector3Int(x, y, 0);
        var maybeTerrainTile = _map.GetTile(cellPosition);

        if (maybeTerrainTile != null && maybeTerrainTile.name.Contains("-terrain"))
        {
            _map.SetTile(cellPosition, greenProtestorsTile);
            _map.SetColor(cellPosition, Color.blue);
            return true;
        }

        return false;
    }

    void Update()
    {
        
    }
}

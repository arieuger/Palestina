using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerActions : MonoBehaviour
{
    
    public bool IsUsingProtest { get; private set; }
    
    [SerializeField] private float zOffset = 0.75f;
    [SerializeField] private Color darkenedTilesColor; // 9DB1DA
    
    private Tilemap _map;
    private Grid _grid;

    void Start()
    {
        _map = GetComponent<Tilemap>();
        _grid = _map.layoutGrid;
    }

    public void TestingButton()
    {
        Debug.Log("Testing button");

        GetFirstCenteredCell();
    }

    private void GetFirstCenteredCell()
    {
        BoundsInt bounds = _map.cellBounds;
        for (int x = 0; x >= bounds.min.x; x--)
        {
            for (int y = 0; y >= bounds.min.y; y--)
            {
                if (GetFirstCenteredCellWhenFound(x, y)) return;
            }
        }
        for (int x = 0; x < bounds.max.x; x++)
        {
            for (int y = 0; y < bounds.min.y; y++)
            {
                if (GetFirstCenteredCellWhenFound(x, y)) return;
            }
        }
    }

    private bool GetFirstCenteredCellWhenFound(int x, int y)
    {
        var cellPosition = new Vector3Int(x, y, 0);
        var maybeTerrainTile = _map.GetTile(cellPosition);

        if (maybeTerrainTile != null && maybeTerrainTile.name.Contains("-terrain"))
        {
            _map.SetColor(cellPosition, Color.blue);
            return true;
        }

        return false;
    }

    void Update()
    {
        
    }
}

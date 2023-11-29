using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private TileManager _tileManager;
    
    private TileBase _selectedActionTile;
    private Vector3Int _actionTilePosition;
    
    private TileBase _oldHoveredTile;
    private Vector3Int _oldHoveredTilePos;
    
    private Vector3Int _selectingActionTile;

    void Start()
    {
        _map = GetComponent<Tilemap>();
        _grid = _map.layoutGrid;
        _tileManager = GetComponent<TileManager>();
    }

    public void SelectProtestButton()
    {
        Debug.Log("Testing button");

        IsUsingProtest = true;
        _tileManager.MakeCellDeselection();
        _selectedActionTile = greenProtestorsTile;
        GetFirstCenteredCell();
    }

    void Update()
    {
        if (!IsUsingProtest) return;
        
        Vector3Int currentCellPos = GetMousePosition();
        TileBase tile = _map.GetTile(currentCellPos);
        
        HoverSelectActionPosition(tile, currentCellPos);
        if (Input.GetButtonDown("Fire1")) IsUsingProtest = false;
    }

    private void HoverSelectActionPosition(TileBase tile, Vector3Int currentCellPos)
    {
        if (tile != null && tile.name.Contains("-terrain"))
        {
            _actionTilePosition = currentCellPos;
            UpdateToActionTile();
        }
    }

    private bool GetFirstCenteredCellWhenFound(int x, int y)
    {
        var cellPosition = new Vector3Int(x, y, 0);
        var maybeTerrainTile = _map.GetTile(cellPosition);

        if (maybeTerrainTile != null && maybeTerrainTile.name.Contains("-terrain"))
        {
            _actionTilePosition = cellPosition;
            UpdateToActionTile();
            return true;
        }

        return false;
    }

    private void UpdateToActionTile()
    {
        Matrix4x4 transformMatrix;
        
        if (_oldHoveredTile != null && _oldHoveredTilePos != _actionTilePosition)
        {
            transformMatrix = _map.GetTransformMatrix(_oldHoveredTilePos);
            Vector3 downTransform = transformMatrix.GetPosition();
            downTransform.y -= 0.1f;
            _map.SetTransformMatrix(_oldHoveredTilePos, Matrix4x4.TRS(downTransform, Quaternion.Euler(0, 0, 0), Vector3.one));
            _map.SetTile(_oldHoveredTilePos, _oldHoveredTile);
        }

        transformMatrix = _map.GetTransformMatrix(_actionTilePosition);
        Vector3 upTransform = transformMatrix.GetPosition();
        upTransform.y += 0.1f;
        _map.SetTransformMatrix(_actionTilePosition, Matrix4x4.TRS(upTransform, Quaternion.Euler(0, 0, 0), Vector3.one));
        
        _oldHoveredTile = _map.GetTile(_actionTilePosition);
        _oldHoveredTilePos = _actionTilePosition;
        _map.SetTile(_actionTilePosition, _selectedActionTile);
        
        DarkenNoActionCells();
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

        for (int x = 0; x < bounds.max.x; x++)
        {
            for (int y = 0; y >= bounds.min.y; y--)
            {
                if (GetFirstCenteredCellWhenFound(x, y)) return;
            }
        }
        
        for (int x = 0; x >= bounds.min.x; x--)
        {
            for (int y = 0; y < bounds.min.y; y++)
            {
                if (GetFirstCenteredCellWhenFound(x, y)) return;
            }
        }
    }
    
    private void DarkenNoActionCells()
    {
        BoundsInt bounds = _map.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);
                var tile = _map.GetTile(cellPosition);
                if (tile == null) continue;
                
                _map.SetColor(cellPosition, _tileManager.TileColors.TryGetValue(cellPosition, out var color) ? 
                    color : cellPosition == _actionTilePosition ? Color.white : darkenedTilesColor); 
                
            }
        }
    }
        
    private Vector3Int GetMousePosition () {
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, zOffset));
    }
}

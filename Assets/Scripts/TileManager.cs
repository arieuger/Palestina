using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private float zOffset = 0.25f;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private Color noSelectedTilesColor; // 9DB1DA
    [SerializeField] private Color dangeredTilesColor; // 
    [SerializeField] private Color noSelectedDangeredTilesColor; // 

    private Tilemap _map;
    private Grid _grid;
    private TilemapRenderer _tilemapRenderer;
    private RaycastHit2D _hit;

    private TileBase _oldHoveredTile;
    private Vector3Int _oldHoveredTilePos;
    private string _oldHoveredTileName;

    private string _nonHoverColor;
    private string _hoverColor;
    private string _oldTileNonHoverColor;
    private string _oldTileHoverColor;

    private bool _isCellSelected;
    private Vector3Int _selectedCellPos;

    private const string Zionist = "-z-";
    private const string Palestinian = "-p-";
    private const string White = "w-";
    private const string Green = "g-";
    private const string Red = "r-";
    private const string DarkRed = "d-";

    private bool _shouldRepaintTiles = true;

    private List<Vector3Int> _dangeredTiles = new List<Vector3Int>();

    void Start()
    {
        _map = GetComponent<Tilemap>();
        _map.RefreshAllTiles();
        _grid = _map.layoutGrid;
        _tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    void Update()
    {
        Vector3Int currentCellPos = GetMousePosition();
        TileBase tile = _map.GetTile(currentCellPos);
        
        HoverTileColor(tile, currentCellPos);
        if (_shouldRepaintTiles) PaintDangeredTiles();
        
        if (tile == null) return;
        if (Input.GetButtonDown("Fire1")) SelectTile(currentCellPos);
        
    }

    private void HoverTileColor(TileBase tile, Vector3Int currentCellPos)
    {
        if (_oldHoveredTile != null && ((_oldHoveredTilePos != _selectedCellPos && _isCellSelected) || !_isCellSelected))
        {
            _oldTileNonHoverColor = _oldHoveredTile.name.Contains(Zionist) ? Red : White;
            _oldTileHoverColor = _oldHoveredTile.name.Contains(Zionist) ? DarkRed : Green;
            _map.SetTile(_oldHoveredTilePos, tiles.Find(t => t.name.Equals(_oldHoveredTileName.Replace(_oldTileHoverColor, _oldTileNonHoverColor))));
        }

        if (tile == null) return;
        
        _nonHoverColor = tile.name.Contains(Zionist) ? Red : White;
        _hoverColor = tile.name.Contains(Zionist) ? DarkRed : Green;

        TileBase replaceTile = tiles.Find(t => t.name.Equals(tile.name.Replace(_nonHoverColor, _hoverColor)));
        _map.SetTile(currentCellPos, replaceTile);
        
        _oldHoveredTile = tile;
        _oldHoveredTilePos = currentCellPos;
        _oldHoveredTileName = tile.name;
    }

    private void PaintDangeredTiles()
    {
        // Set danger by bulldozer tiles
        BoundsInt bounds = _map.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);
                var dangerousTile = _map.GetTile(cellPosition);
                if (dangerousTile == null || !dangerousTile.name.Contains("-bulldoz-")) continue;

                string direction = dangerousTile.name.Substring(dangerousTile.name.Length - 2);
                Debug.Log(direction);
                var paintDangerPosition = cellPosition;
                switch (direction)
                {
                    case "do":
                        paintDangerPosition.x -= 1;
                        break;
                    case "le":
                        paintDangerPosition.y += 1;
                        break;
                    case "ri":
                        paintDangerPosition.y -= 1;
                        break;
                    case "up":
                        paintDangerPosition.x += 1;
                        break;
                }

                if (_map.GetTile(paintDangerPosition) != null)
                {
                    _map.SetColor(paintDangerPosition, dangeredTilesColor);
                    _dangeredTiles.Add(paintDangerPosition);
                }
            }
        }

        _shouldRepaintTiles = false;
    }

    private void SelectTile(Vector3Int currentCellPos)
    {
        if (_isCellSelected)
        {
            MakeCellDeselection();
        }
        
        if (!_isCellSelected || _selectedCellPos != currentCellPos)
        {
            MakeCellSelection(currentCellPos);
        }
        else
        {
            _isCellSelected = false;
        }
        
        DarkenUnselectedcells();

        Debug.Log(_map.GetTile(currentCellPos)  + " - " + _isCellSelected);
    }

    private void MakeCellSelection(Vector3Int currentCellPos)
    {
        _isCellSelected = true;
        Matrix4x4 transformMatrix = _map.GetTransformMatrix(currentCellPos);
        Vector3 upTransform = transformMatrix.GetPosition();
        upTransform.y += 0.1f;
        _map.SetTransformMatrix(currentCellPos, Matrix4x4.TRS(upTransform, Quaternion.Euler(0, 0, 0), Vector3.one));
        _selectedCellPos = currentCellPos;

    }

    private void MakeCellDeselection()
    {
        Matrix4x4 oldTransformMatrix = _map.GetTransformMatrix(_selectedCellPos);
        Vector3 downTransform = oldTransformMatrix.GetPosition();
        downTransform.y -= 0.1f;
        _map.SetTransformMatrix(_selectedCellPos, Matrix4x4.TRS(downTransform, Quaternion.Euler(0, 0, 0), Vector3.one));

        TileBase deselectTile = _map.GetTile(_selectedCellPos);
        string deselectTileHColor = deselectTile.name.Contains(Zionist) ? DarkRed : Green;
        string deselectTileNhColor = deselectTile.name.Contains(Zionist) ? Red : White;
        _map.SetTile(_selectedCellPos,
            tiles.Find(t => t.name.Equals(deselectTile.name.Replace(deselectTileHColor, deselectTileNhColor))));
    }
    
    private void DarkenUnselectedcells()
    {
        BoundsInt bounds = _map.cellBounds;
        for (int x = bounds.min.x; x < bounds.max.x; x++)
        {
            for (int y = bounds.min.y; y < bounds.max.y; y++)
            {
                var cellPosition = new Vector3Int(x, y, 0);
                var tile = _map.GetTile(cellPosition);
                if (tile == null) continue;

                if (_isCellSelected && cellPosition != _selectedCellPos)
                {
                    _map.SetColor(cellPosition, _dangeredTiles.Contains(cellPosition) ? noSelectedDangeredTilesColor : noSelectedTilesColor); 
                }
                else
                {
                    _map.SetColor(cellPosition, _dangeredTiles.Contains(cellPosition) ? dangeredTilesColor : Color.white);
                }
            }
        }
    }
    
    private Vector3Int GetMousePosition () {
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, zOffset));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private float zOffset = 0.25f;
    [SerializeField] private List<TileBase> tiles;

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

        if (tile == null) return;

        if (!_isCellSelected) HoverTileColor(tile, currentCellPos);
        if (Input.GetButtonDown("Fire1")) SelectTile(currentCellPos);
        
    }

    private void HoverTileColor(TileBase tile, Vector3Int currentCellPos)
    {
        _nonHoverColor = tile.name.Contains(Zionist) ? Red : White;
        _hoverColor = tile.name.Contains(Zionist) ? DarkRed : Green;
        if (_oldHoveredTile != null)
        {
            _oldTileNonHoverColor = _oldHoveredTile.name.Contains(Zionist) ? Red : White;
            _oldTileHoverColor = _oldHoveredTile.name.Contains(Zionist) ? DarkRed : Green;
            _map.SetTile(_oldHoveredTilePos,
                tiles.Find(t =>
                    t.name.Equals(_oldHoveredTileName.Replace(_oldTileHoverColor, _oldTileNonHoverColor))));
        }

        TileBase replaceTile = tiles.Find(t => t.name.Equals(tile.name.Replace(_nonHoverColor, _hoverColor)));
        _map.SetTile(currentCellPos, replaceTile);
        
        _oldHoveredTile = tile;
        _oldHoveredTilePos = currentCellPos;
        _oldHoveredTileName = tile.name;
    }

    private void SelectTile(Vector3Int currentCellPos)
    {
        if (!_isCellSelected) _selectedCellPos = currentCellPos;

        if (_selectedCellPos == currentCellPos || !_isCellSelected)
        {
            _isCellSelected = !_isCellSelected;
            Matrix4x4 transformMatrix = _map.GetTransformMatrix(currentCellPos);
            Vector3 upTransform = transformMatrix.GetPosition();
            upTransform.y += 0.1f * (_isCellSelected ? 1 : -1);
            _map.SetTransformMatrix(currentCellPos,
                Matrix4x4.TRS(upTransform, Quaternion.Euler(0, 0, 0), Vector3.one));
        }
    }

    Vector3Int GetMousePosition () {
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, zOffset));
    }
}
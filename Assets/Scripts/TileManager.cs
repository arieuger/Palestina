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


    // Start is called before the first frame update
    void Start()
    {
        _map = GetComponent<Tilemap>();
        _map.RefreshAllTiles();
        _grid = _map.layoutGrid;
        _tilemapRenderer = GetComponent<TilemapRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int currentCellPos = _grid.WorldToCell(new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, zOffset));
        TileBase tile = _map.GetTile(currentCellPos);
        if (tile != null)
        {
            if (!_isCellSelected)
            {

                _nonHoverColor = tile.name.Contains("z-") ? "r-" : "w-";
                _hoverColor = tile.name.Contains("z-") ? "d-" : "g-";
                if (_oldHoveredTile != null)
                {
                    _oldTileNonHoverColor = _oldHoveredTile.name.Contains("z-") ? "r-" : "w-";
                    _oldTileHoverColor = _oldHoveredTile.name.Contains("z-") ? "d-" : "g-";
                    _map.SetTile(_oldHoveredTilePos,
                        tiles.Find(t =>
                            t.name.Equals(_oldHoveredTileName.Replace(_oldTileHoverColor, _oldTileNonHoverColor))));
                }


                // _map.SetColor(currentCellPos, Color.red);
                TileBase replaceTile = tiles.Find(t => t.name.Equals(tile.name.Replace(_nonHoverColor, _hoverColor)));
                _map.SetTile(currentCellPos, replaceTile);
            }

            if (Input.GetButtonDown("Fire1"))
            {
//                 _isCellSelected = !_isCellSelected;
                if (!_isCellSelected)
                {
                    _selectedCellPos = currentCellPos;
                }

                if (_selectedCellPos == currentCellPos || !_isCellSelected)
                {
                    _isCellSelected = !_isCellSelected;
                    Matrix4x4 transformMatrix = _map.GetTransformMatrix(currentCellPos);
                    Vector3 upTransform = transformMatrix.GetPosition();
                    upTransform.y += 0.1f * (_isCellSelected ? 1 : -1);
                    _map.SetTransformMatrix(currentCellPos, Matrix4x4.TRS(upTransform, Quaternion.Euler(0,0,0), Vector3.one));
                    // _isCellSelected = !_isCellSelected;
                }
                
                // Debug.Log(tile.name);
                
            }

            _oldHoveredTile = tile;
            _oldHoveredTilePos = currentCellPos;
            _oldHoveredTileName = tile.name;
            
            Debug.Log(_isCellSelected);
        }
    }
    
    Vector3Int GetMousePosition () {

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(mouseWorldPos);

    }
}

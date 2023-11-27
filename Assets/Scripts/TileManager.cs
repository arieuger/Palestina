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

    private TileBase oldHoveredTile;
    private Vector3Int oldHoveredTilePos;
    private string oldHoveredTileName;

    private string nonHoverColor;
    private string hoverColor;


    // Start is called before the first frame update
    void Start()
    {
        _map = GetComponent<Tilemap>();
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
            nonHoverColor = tile.name.Contains("z-") ? "r-" : "w-";
            hoverColor = tile.name.Contains("z-") ? "d-" : "g-";
            if (oldHoveredTile != null)
            {
                _map.SetTile(oldHoveredTilePos, tiles.Find(t => t.name.Equals(oldHoveredTileName.Replace(hoverColor, nonHoverColor))));
            }
            
            
            // _map.SetColor(currentCellPos, Color.red);
            TileBase replaceTile = tiles.Find(t => t.name.Equals(tile.name.Replace(nonHoverColor, hoverColor)));
            _map.SetTile(currentCellPos, replaceTile);
            
            if (Input.GetButtonDown("Fire1"))
            {
            
              //  map.GetSprite(currentCellPos).;
                Debug.Log(tile.name);
                
            }

            oldHoveredTile = tile;
            oldHoveredTilePos = currentCellPos;
            oldHoveredTileName = tile.name;
        }
    }
    
    Vector3Int GetMousePosition () {

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(mouseWorldPos);

    }
}

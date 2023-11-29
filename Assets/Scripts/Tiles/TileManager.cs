using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour
{
    [SerializeField] private float zOffset = 0.75f;
    [SerializeField] private List<TileBase> tiles;
    [SerializeField] private Color noSelectedTilesColor; // 9DB1DA
    [SerializeField] private Color dangeredTilesColor; // 
    [SerializeField] private Color noSelectedDangeredTilesColor; // 

    private Tilemap _map;
    private Grid _grid;

    private TileBase _oldHoveredTile;
    private Vector3Int _oldHoveredTilePos;
    private string _oldHoveredTileName;
    private PlayerActions _playerActions;

    private string _nonHoverColor;
    private string _hoverColor;
    private string _oldTileNonHoverColor;
    private string _oldTileHoverColor;

    private bool _isCellSelected;
    private Vector3Int _selectedCellPos;

    private bool _shouldRepaintTiles = true;

    public Dictionary<Vector3Int, Color> TileColors { get; set; }

    void Start()
    {
        _map = GetComponent<Tilemap>();
        _map.RefreshAllTiles();
        _grid = _map.layoutGrid;
        _playerActions = GetComponent<PlayerActions>();
        TileColors = new Dictionary<Vector3Int, Color>();
    }

    void Update()
    {

        if (_playerActions.IsUsingProtest) return;
        
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
            _oldTileNonHoverColor = _oldHoveredTile.name.Contains(Constants.Zionist) ? Constants.Red : Constants.White;
            _oldTileHoverColor = _oldHoveredTile.name.Contains(Constants.Zionist) ? Constants.DarkRed : Constants.Green;
            _map.SetTile(_oldHoveredTilePos, tiles.Find(t => t.name.Equals(_oldHoveredTileName.Replace(_oldTileHoverColor, _oldTileNonHoverColor))));
        }

        if (tile == null) return;
        
        _nonHoverColor = tile.name.Contains(Constants.Zionist) ? Constants.Red : Constants.White;
        _hoverColor = tile.name.Contains(Constants.Zionist) ? Constants.DarkRed : Constants.Green;

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
                var paintDangerPosition = cellPosition;
                switch (direction)
                {
                    case "do":
                    case "up":
                        paintDangerPosition.x += direction.Equals("up") ? 1 : -1;
                        break;
                    case "le":
                    case "ri":
                        paintDangerPosition.y += direction.Equals("le") ? 1 : -1;;
                        break;
                }

                if (_map.GetTile(paintDangerPosition) != null)
                {
                    _map.SetColor(paintDangerPosition, dangeredTilesColor);
                    TileColors[paintDangerPosition] = dangeredTilesColor;
                }
            }
        }

        _shouldRepaintTiles = false;
    }

    private void SelectTile(Vector3Int currentCellPos)
    {

        MakeCellDeselection();
        
        if (!_isCellSelected || _selectedCellPos != currentCellPos)
        {
            MakeCellSelection(currentCellPos);
        }
        else
        {
            _isCellSelected = false;
            UiManager.Instance.SetSelectedElementText("");
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
        UiManager.Instance.SetSelectedElementText(_map.GetTile(_selectedCellPos).name);

    }

    public void MakeCellDeselection()
    {
        if (!_isCellSelected) return;
        Matrix4x4 oldTransformMatrix = _map.GetTransformMatrix(_selectedCellPos);
        Vector3 downTransform = oldTransformMatrix.GetPosition();
        downTransform.y -= 0.1f;
        _map.SetTransformMatrix(_selectedCellPos, Matrix4x4.TRS(downTransform, Quaternion.Euler(0, 0, 0), Vector3.one));

        TileBase deselectTile = _map.GetTile(_selectedCellPos);
        string deselectTileHColor = deselectTile.name.Contains(Constants.Zionist) ? Constants.DarkRed : Constants.Green;
        string deselectTileNhColor = deselectTile.name.Contains(Constants.Zionist) ? Constants.Red : Constants.White;
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
                    _map.SetColor(cellPosition, TileColors.ContainsKey(cellPosition) ? noSelectedDangeredTilesColor : noSelectedTilesColor); 
                    if (_map.GetColor(cellPosition).Equals(noSelectedDangeredTilesColor)) TileColors[cellPosition] = noSelectedDangeredTilesColor;
                    
                }
                else
                {
                    _map.SetColor(cellPosition, TileColors.ContainsKey(cellPosition) ? dangeredTilesColor : Color.white);
                    if (_map.GetColor(cellPosition).Equals(dangeredTilesColor)) TileColors[cellPosition] = dangeredTilesColor;
                }
            }
        }
    }
    
    private Vector3Int GetMousePosition () {
        Vector3 screenToWorldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return _grid.WorldToCell(new Vector3(screenToWorldPoint.x, screenToWorldPoint.y, zOffset));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestShaderColor : MonoBehaviour
{

    [SerializeField] private Tilemap map;
    private Material _material;
    private TilemapRenderer _tilemapRenderer;
    private RaycastHit2D _hit;
    
    // Start is called before the first frame update
    void Start()
    {
        _tilemapRenderer = GetComponent<TilemapRenderer>();
        _material = _tilemapRenderer.material;
        // _material.SetColor("_Color", Color.blue);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            Vector3 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Debug.Log(map.GetTile(Vector3Int.FloorToInt(pos)));
            
            _hit = Physics2D.Raycast(worldPoint, Vector2.down);

            if (_hit.collider != null)
            {
                Debug.Log("click on " + _hit.collider.name);
                Debug.Log(_hit.point);

                var tpos = map.WorldToCell(_hit.point);

                // Try to get a tile from cell position
                var tile = map.GetTile(tpos);
                Debug.Log(tile.name);
            }
        }
    }
}

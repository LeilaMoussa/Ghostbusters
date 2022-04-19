using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Game.GamePlay;
using CodeMonkey.Utils;


public class GameGrid : MonoBehaviour
{
    [SerializeField] public int _width, _height;
 
    [SerializeField] private Tile _tilePrefab;
 
    [SerializeField] private Transform _cam;
    
    [SerializeField] public TextMesh to_bust;


    private Dictionary<Vector2, Tile> _tiles;


    void Start() {
        this.to_bust = UtilsClass.CreateWorldText("Reclick on a tile to bust the ghost", null, new Vector3(10.0f, 8.0f, 0.0f), 20, Color.white, TextAnchor.MiddleCenter);
        this.to_bust.transform.localScale = new Vector3((float)0.4,(float)0.4,(float)0.4);
        GenerateGrid();
    }
 
    void GenerateGrid() {
        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x} {y}";
 
                var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
                spawnedTile.Init(isOffset);
                
                _tiles[new Vector2(x, y)] = spawnedTile;
                InitGamePlay(_tiles, _width, _height);
            }
        }
        _cam.transform.position = new Vector3((float)_width/2 -0.5f, (float)_height / 2 - 0.5f,-10);
    }
 
    
}

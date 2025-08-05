using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private int _width, _height;
    [SerializeField] private Tile _tilePrefab;
    private void Start()
    {
        GenerateGrid();
    }
    void GenerateGrid()
    {
        for(int x = 0; x <= _width; x++)
        {
            for(int  y = 0; y <= _height; y++)
            {
                var isOffset =((x + y) % 2 == 1);
                var spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                spawnedTile.Init(isOffset);
                spawnedTile.name = $"Tile{x}{y}";
            }
        }
    }
}

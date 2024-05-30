using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{

    [Header("Settings")]
    [Range(6, 12)]
    [SerializeField] private uint _width = 12;
    [Range(6, 12)]
    [SerializeField] private uint _height = 12;

    private Tile[,] _tileData;

    private void Start()
    {
        GenerateBoard();
    }

    private void GenerateBoard()
    {
        _tileData = new Tile[_width, _height];

        var xCount = _tileData.GetLength(0);
        var yCount = _tileData.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                //_tileData[x, y] = new Tile(x, y, );
            }
        }
    }

    private void ClearBoard()
    {
        var xCount = _tileData.GetLength(0);
        var yCount = _tileData.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tileData = _tileData[x, y].TileData;

                if (tileData)
                {
                    Destroy(tileData.gameObject);
                }
            }
        }
    }
}

[Serializable]
public struct Tile
{
    private readonly int _x;
    private readonly int _y;

    public BaseTile TileData { get; private set; }

    public Tile(int x, int y, BaseTile tileObject = null)
    {
        _x = x;
        _y = y;
        TileData = tileObject;
    }

    public void SetTileObject(BaseTile tileObject)
    {
        TileData = tileObject;
    }
}

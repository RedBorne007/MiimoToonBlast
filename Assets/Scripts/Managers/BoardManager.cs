using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static TileData;

public class BoardManager : Singleton<BoardManager>
{
    [Header("Settings")]
    [Range(6, 12)]
    [SerializeField] private uint _width = 12;
    [Range(6, 12)]
    [SerializeField] private uint _height = 12;

    [Header("References")]
    [SerializeField] private TileData _tileRef;

    private TileData[] _tileDataRefs;
    private Tile[,] _tiles;

    private PoolManager _poolManager;

    private void Start()
    {
        _poolManager = PoolManager.Instance;
        _tileDataRefs = Resources.LoadAll<TileData>("TileData");

        GenerateBoard();
    }

    #region Board Management

    private void GenerateBoard()
    {
        _tiles = new Tile[_width, _height];

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                //_tiles[x, y] = Instantiate(_prefabManager.GetRandomColorTile());
            }
        }
    }

    private void ClearBoard()
    {
        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tileData = _tiles[x, y];

                if (tileData)
                {
                    Destroy(tileData.gameObject);
                }
            }
        }
    }

    #endregion

    #region Tile

    private TileData GetRandomTileRef(TileType tileType)
    {
        var tileDatas = _tileDataRefs.Where(t => t.Type == tileType).ToArray();
        int index = UnityEngine.Random.Range(0, tileDatas.Length);
        return tileDatas[index];
    }

    public Tile GetTile(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return null;
        }

        return _tiles[x, y];
    }

    /*
    public Tile[] GetNeighborTiles()
    {

    }
    */

    #endregion
}

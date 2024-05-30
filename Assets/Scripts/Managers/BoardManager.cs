using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static TileData;

public class BoardManager : Singleton<BoardManager>
{
    [Header("Settings")]
    [Range(6, 12)]
    [SerializeField] private uint _width = 12;
    [Range(6, 12)]
    [SerializeField] private uint _height = 12;

    [Header("References")]
    [SerializeField] private RectTransform _boardParent;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private Transform _tileParentRef;
    [SerializeField] private BaseTile _tileRef;

    private TileData[,] _tiles;

    private PoolManager _poolManager;

    private void Start()
    {
        _poolManager = PoolManager.Instance;

        GenerateBoard();
    }

    #region Board Management

    private void GenerateBoard()
    {
        ClearBoard();

        _tiles = new TileData[_width, _height];

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        _gridLayout.constraintCount = xCount;

        // Calculate grid layout size.
        var totalSize = xCount * (_gridLayout.cellSize.x + _gridLayout.spacing.x);
        var boardSize = _boardParent.rect.size.x;

        if (totalSize > boardSize)
        {
            var finalSize = (boardSize / xCount) - _gridLayout.spacing.x;
            _gridLayout.cellSize = Vector2.one * finalSize;
        }

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var _tileParent = Instantiate(_tileParentRef);
                _tileParent.name = $"Tile ({x}, {y})";
                _tileParent.SetParent(_boardParent);

                var _tileObj = Instantiate(_tileRef);
                _tileObj.transform.SetParent(_tileParent);

                _tiles[x, y] = new TileData(x, y, _tileParent, _tileObj);
            }
        }
    }

    private void ClearBoard()
    {
        if (_tiles == null)
        {
            return;
        }

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tileData = _tiles[x, y];
                Destroy(tileData.TileParent);
            }
        }

        _tiles = null;
    }

    #endregion

    #region Tile

    public TileData GetTile(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return new TileData();
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

[Serializable]
public struct TileData
{
    public readonly int _x;
    public readonly int _y;

    public readonly Transform TileParent;

    public BaseTile Tile { get; private set; }

    public TileData(int x, int y, Transform tileParent, BaseTile tile)
    {
        _x = x;
        _y = y;

        TileParent = tileParent;
        Tile = tile;
    }

    public void SetTile(BaseTile tile)
    {
        Tile = tile;
        
        Tile.transform.SetParent(TileParent);
        Tile.transform.localPosition = Vector3.zero;
        Tile.transform.localScale = Vector3.one;
    }
}

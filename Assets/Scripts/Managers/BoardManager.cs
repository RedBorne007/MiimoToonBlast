using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColorStyle;

public class BoardManager : Singleton<BoardManager>
{
    [Header("Settings")]
    [Range(6, 12)]
    [SerializeField] private int _width = 12;
    [Range(6, 12)]
    [SerializeField] private int _height = 12;

    [field: Space]

    [field: SerializeField] public uint NormalPopRequirement { get; private set; } = 3;
    [field: SerializeField] public uint SpawnBombRequirement { get; private set; } = 6;
    [field: SerializeField] public uint SpawnDiscoRequirement { get; private set; } = 10;

    [Header("References")]
    [SerializeField] private RectTransform _boardParent;
    [SerializeField] private GridLayoutGroup _gridLayout;
    [SerializeField] private Transform _tileParentRef;
    [SerializeField] private ColorStyle _colorStyle;

    private TileData[,] _tiles;
    private List<BaseTile> _extraSpawnTiles = new List<BaseTile>();

    public ColorStyleData PreviousColorStyleData;

    private int _score;
    public int Score
    {
        get => _score;
        set
        {
            _score = value;
            UIManager.Instance.UpdateScore();
        }
    }

    public int Width => _width;
    public int Height => _height;

    private PoolManager _poolManager;
    private PrefabManager _prefabManager;

    private void Start()
    {
        _poolManager = PoolManager.Instance;
        _prefabManager = PrefabManager.Instance;

        GenerateBoard();
    }

    #region Board Management

    public void GenerateBoard()
    {
        ClearBoard();

        _tiles = new TileData[_width, _height];

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        _gridLayout.constraintCount = xCount;

        // Calculate grid layout size to match with screen.
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
                _tileParent.localScale = Vector3.one;

                _tiles[x, y] = new TileData(x, y, _tileParent, null);

                GenerateTile(_tiles[x, y]);
            }
        }
    }

    public void ClearBoard()
    {
        if (_tiles == null) return;

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tileData = _tiles[x, y];

                if (tileData.Tile)
                {
                    _poolManager.Despawn(tileData.Tile);
                }

                Destroy(tileData.TileParent.gameObject);
            }
        }

        _tiles = null;
    }

    public void RefreshBoard()
    {
        if (_tiles == null) return;

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tileData = _tiles[x, y];

                // If tile is empty, move from above tile to this tile.
                if (!tileData.Tile)
                {
                    int checkY = y + 1;
                    bool move = false;

                    // Search through above tiles...
                    while (checkY < yCount)
                    {
                        var aboveTile = GetTile(x, checkY);

                        if (aboveTile != null)
                        {
                            if (aboveTile.Tile)
                            {
                                move = true;
                                MoveTile(aboveTile, tileData);
                                break;
                            }

                            checkY++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    // If there's no tile above, generate new tile instead.
                    if (!move)
                    {
                        GenerateTile(tileData);
                    }
                }
            }
        }
    }

    private void GenerateTile(TileData tileDataRef)
    {
        BaseTile prefabRef = _prefabManager.NormalTile;

        // If there's extra spawn, random chance to spawn one.
        if (_extraSpawnTiles.Count > 0)
        {
            int index = Random.Range(0, _extraSpawnTiles.Count - 1);
            prefabRef = _extraSpawnTiles[index];
            _extraSpawnTiles.RemoveAt(index);
        }

        var tileObj = _poolManager.GetOrCreate(prefabRef) as BaseTile;

        switch (tileObj)
        {
            case NormalTile normalTile:
                normalTile.SetColorData(_colorStyle.GetRandomColorData());
                break;

            case DiscoTile discoTile:
                discoTile.SetColorData(PreviousColorStyleData);
                break;
        }

        tileDataRef.SetTile(tileObj);
    }

    public void Shuffle()
    {
        if (_tiles == null) return;

        var xCount = _tiles.GetLength(0);
        var yCount = _tiles.GetLength(1);

        for (int x = 0; x < xCount; x++)
        {
            for (int y = 0; y < yCount; y++)
            {
                var tile = _tiles[x, y];
                var tileObj = tile.Tile;

                int randomX = Random.Range(0, xCount - 1);
                int randomY = Random.Range(0, yCount - 1);
                var randomTile = _tiles[randomX, randomY];
                var randomTileObj = randomTile.Tile;

                tile.SetTile(randomTileObj);
                randomTile.SetTile(tileObj);
            }
        }
    }

    #endregion

    #region Tile

    public TileData GetTile(int x, int y)
    {
        if (x < 0 || x >= _width || y < 0 || y >= _height)
        {
            return null;
        }

        return _tiles[x, y];
    }

    public HashSet<TileData> GetNeighborTiles(int x, int y)
    {
        var neighborTiles = new HashSet<TileData>();

        var topTile = GetTile(x, y + 1);
        var bottomTile = GetTile(x, y - 1);
        var leftTile = GetTile(x - 1, y);
        var rightTile = GetTile(x + 1, y);

        if (topTile != null)
        {
            neighborTiles.Add(topTile);
        }

        if (bottomTile != null)
        {
            neighborTiles.Add(bottomTile);
        }

        if (leftTile != null)
        {
            neighborTiles.Add(leftTile);
        }

        if (rightTile != null)
        {
            neighborTiles.Add(rightTile);
        }

        return neighborTiles;
    }

    public TileData[,] GetAllTiles()
    {
        return _tiles;
    }

    private void MoveTile(TileData fromData, TileData toData)
    {
        if (toData.Tile) return;

        if (fromData.Tile)
        {
            toData.SetTile(fromData.Tile);
            fromData.ClearTile(false);
        }
    }

    #endregion

    public void AddExtraSpawn(BaseTile tile)
    {
        _extraSpawnTiles.Add(tile);
    }
}

public class TileData
{
    public readonly int X;
    public readonly int Y;

    public readonly Transform TileParent;

    public BaseTile Tile { get; private set; }

    public TileData(int x, int y, Transform tileParent, BaseTile tile)
    {
        X = x;
        Y = y;

        TileParent = tileParent;
        Tile = tile;
    }

    public void SetTile(BaseTile tile)
    {
        if (!tile) return;

        Tile = tile;
        Tile.SetPosition(X, Y);

        bool isNew = !Tile.transform.parent;

        Tile.transform.SetParent(TileParent);
        Tile.transform.SetAsFirstSibling();
        Tile.transform.localScale = Vector3.one;

        Tile.GetComponent<RectTransform>().sizeDelta = Vector2.zero;

        if (isNew)
        {
            Tile.transform.localPosition = Vector2.up * 1000f;
        }

        Tile.transform.DOLocalMove(Vector2.zero, 0.25f);
    }

    public void ClearTile(bool despawn = true)
    {
        if (Tile && despawn)
        {
            PoolManager.Instance.Despawn(Tile);
        }

        Tile = null;
    }
}
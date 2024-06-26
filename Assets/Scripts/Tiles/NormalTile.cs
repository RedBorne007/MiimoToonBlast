using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColorStyle;

public sealed class NormalTile : BaseTile, IColorable
{
    [SerializeField] private Image _colorRenderer;

    private HashSet<BaseTile> cacheNeighborTiles;

    public ColorStyleData ColorStyleData { get; private set; }

    public override void OnClick()
    {
        if (IsPopable())
        {
            foreach (var tile in cacheNeighborTiles)
            {
                BoardManager.GameEventData.RowChanges.Add(tile.GetTileData().X);
                tile.Pop();
            }

            var popCount = cacheNeighborTiles.Count;

            // If reach bomb spawn requirement, spawn one on next refill.
            if (popCount >= BoardManager.SpawnBombRequirement)
            {
                BoardManager.GameEventData.ExtraSpawns.Add(PrefabManager.BombTile);
            }

            // If reach disco spawn requirement, spawn one on next refill.
            if (popCount >= BoardManager.SpawnDiscoRequirement)
            {
                BoardManager.GameEventData.ColorStyleData = ColorStyleData;
                BoardManager.GameEventData.ExtraSpawns.Add(PrefabManager.DiscoTile);
            }

            BoardManager.Score += popCount;
            BoardManager.RefreshBoard();
        }
    }

    public override bool IsPopable()
    {
        var allNearbyTile = FindAllNearbyMatch();

        return allNearbyTile.Count >= BoardManager.NormalPopRequirement;
    }

    private HashSet<BaseTile> FindAllNearbyMatch()
    {
        cacheNeighborTiles = new HashSet<BaseTile>() { this };
        var searchTiles = new HashSet<TileData> { GetTileData() };

        // Find neighbor tiles until there's no more tile to search.
        while (searchTiles.Count > 0)
        {
            var cachSearchTiles = new HashSet<TileData>(searchTiles);
            searchTiles.Clear();

            foreach (var tile in cachSearchTiles)
            {
                var neighborTiles = BoardManager.GetNeighborTiles(tile.X, tile.Y);

                foreach (var neighborTile in neighborTiles)
                {
                    if (neighborTile.Tile && neighborTile.Tile is IColorable _iColorable &&
                        !cacheNeighborTiles.Contains(neighborTile.Tile) && IsSameColor(_iColorable.ColorStyleData))
                    {
                        cacheNeighborTiles.Add(neighborTile.Tile);
                        searchTiles.Add(BoardManager.GetTile(neighborTile.X, neighborTile.Y));
                    }
                }
            }
        }

        return cacheNeighborTiles;
    }

    public void SetColorData(ColorStyleData colorData)
    {
        ColorStyleData = colorData;
        _colorRenderer.color = colorData.Color;
    }

    public bool IsSameColor(ColorStyleData colorData)
    {
        return ColorStyleData.ColorName == colorData.ColorName;
    }
}

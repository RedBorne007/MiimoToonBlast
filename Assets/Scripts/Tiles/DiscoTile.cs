using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ColorStyle;

public sealed class DiscoTile : BaseTile, IColorable
{
    [SerializeField] private Image _colorRenderer;

    public ColorStyleData ColorStyleData { get; private set; }

    public override void OnClick()
    {
        var allColorTiles = FindAllColorTiles();

        foreach (var tile in allColorTiles)
        {
            if (tile)
            {
                tile?.Pop();
                BoardManager.GameEventData.RowChanges.Add(tile.GetTileData().X);
            }
        }

        BoardManager.Score += allColorTiles.Count;

        GetTileData().ClearTile();
        BoardManager.RefreshBoard();
    }

    public override bool IsPopable()
    {
        return true;
    }

    public override void Pop()
    {
        base.Pop();

        OnClick();
    }

    private HashSet<BaseTile> FindAllColorTiles()
    {
        var colorTiles = new HashSet<BaseTile>();
        var allTiles = BoardManager.GetAllTiles();

        // Search all tiles with same color.
        foreach (var tileData in allTiles)
        {
            if (!tileData.Tile) continue;

            var tile = tileData.Tile;

            if (tile is NormalTile normalTile && normalTile.IsSameColor(ColorStyleData))
            {
                colorTiles.Add(tile);
            }
        }

        return colorTiles;
    }

    public void SetColorData(ColorStyleData colorData)
    {
        ColorStyleData = colorData;
        _colorRenderer.color = colorData.Color;
    }
}

using System.Collections;
using System.Collections.Generic;

public sealed class BombTile : BaseTile
{
    public override void OnClick()
    {
        var allBombTiles = FindAllBombTiles();

        foreach (var tile in allBombTiles)
        {
            if (tile)
            {
                tile?.Pop();
                BoardManager.GameEventData.RowChanges.Add(tile.GetTileData().X);
            }
        }

        BoardManager.Score += allBombTiles.Count;

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

    private HashSet<BaseTile> FindAllBombTiles()
    {
        var bombTiles = new HashSet<BaseTile>();

        int x = 0;
        int y = 0;
        int width = BoardManager.Width;
        int height = BoardManager.Height;

        // Scan in X axis.
        while (x < width)
        {
            var tile = BoardManager.GetTile(x, Y);

            if (tile.Tile)
            {
                bombTiles.Add(tile.Tile);
            }

            x++;
        }

        // Scan in Y axis.
        while (y < height)
        {
            var tile = BoardManager.GetTile(X, y);

            if (tile.Tile)
            {
                bombTiles.Add(tile.Tile);
            }

            y++;
        }

        bombTiles.Remove(this);
        return bombTiles;
    }
}

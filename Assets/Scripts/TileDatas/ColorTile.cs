using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTile : BaseTile
{
    public enum ColorTileType
    {
        Red, Green, Blue, Yellow
    }

    [SerializeField] private ColorTileType _colorType;

    protected override void OnClick()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tile Data", menuName = "Custom/Tile Data")]
public class TileData : ScriptableObject
{
    public enum TileType
    {
        Normal, Bomb, Disco
    }

    [field: SerializeField] public TileType Type { get; private set; }
    [field: SerializeField] public Sprite Sprite { get; private set; }
}

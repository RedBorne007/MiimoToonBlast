using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Color Style", menuName = "Custom/Color Style")]
public class ColorStyle : ScriptableObject
{
    [field: SerializeField] public ColorStyleData[] ColorDatas { get; private set; }

    public ColorStyleData GetRandomColorData()
    {
        int index = Random.Range(0, ColorDatas.Length);
        return ColorDatas[index];
    }

    [System.Serializable]
    public struct ColorStyleData
    {
        [field: SerializeField] public string ColorName { get; private set; }
        [field: SerializeField] public Color Color { get; private set; }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColorStyle;

public interface IColorable
{
    public ColorStyleData ColorStyleData { get; }

    void SetColorData(ColorStyleData colorData);
}

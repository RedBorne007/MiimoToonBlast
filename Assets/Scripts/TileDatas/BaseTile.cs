using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseTile : MonoBehaviour
{
    [SerializeField] private Sprite _sprite;

    protected abstract void OnClick();

    private void OnMouseDown()
    {
        OnClick();
    }
}

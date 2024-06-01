using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseTile : MonoBehaviour, IPoolable
{
    [SerializeField] private Button _button;

    protected int X { get; private set; }
    protected int Y { get; private set; }

    public GameObject PoolObject => gameObject;

    protected BoardManager BoardManager;

    private void Start()
    {
        BoardManager = BoardManager.Instance;

        _button.onClick.AddListener(OnClick);
    }

    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    public abstract void OnClick();
    public abstract bool IsPopable();

    public virtual void Pop()
    {
        SpawnBreakParticle();

        var tile = BoardManager.GetTile(X, Y);
        tile.ClearTile();
    }

    private void SpawnBreakParticle()
    {
        var breakParticle = PoolManager.Instance.GetOrCreate(PrefabManager.Instance.BreakParticle) as ParticlePool;

        var particleTransform = breakParticle.PoolObject.transform;
        particleTransform.SetParent(UIManager.Instance._particleParent.transform);
        particleTransform.transform.position = transform.position;
        particleTransform.localScale = Vector3.one;

        if (this is IColorable iColorable)
        {
            breakParticle.SetColor(iColorable.ColorStyleData);
        }
    }

    #region IPoolable

    public string GetID()
    {
        return name.Replace("(Clone)", "");
    }

    public virtual void Spawn()
    {
        gameObject.SetActive(true);
    }

    public virtual void Despawn()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    #endregion
}

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
    protected PrefabManager PrefabManager;

    private void Start()
    {
        BoardManager = BoardManager.Instance;
        PrefabManager = PrefabManager.Instance;

        _button.onClick.AddListener(OnClick);
    }

    /// <summary>
    /// Set position of this tile.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public void SetPosition(int x, int y)
    {
        X = x;
        Y = y;
    }

    /// <summary>
    /// Get tile data of this tile.
    /// </summary>
    /// <returns></returns>
    public TileData GetTileData()
    {
        return BoardManager.GetTile(X, Y);
    }

    /// <summary>
    /// Event when tile is pressed.
    /// </summary>
    public abstract void OnClick();

    /// <summary>
    /// Condition to check if this tile is able to pop/destroy.
    /// </summary>
    /// <returns></returns>
    public abstract bool IsPopable();

    /// <summary>
    /// Event when this tile is pop/destroy.
    /// </summary>
    public virtual void Pop()
    {
        SpawnPopParticle();

        var tile = BoardManager.GetTile(X, Y);
        tile.ClearTile();
    }

    /// <summary>
    /// Spawn particle on pop/destroy.
    /// </summary>
    private void SpawnPopParticle()
    {
        var popParticle = PoolManager.Instance.GetOrCreate(PrefabManager.Instance.PopParticle) as ParticlePool;

        var particleTransform = popParticle.PoolObject.transform;
        particleTransform.SetParent(UIManager.Instance._particleParent.transform);
        particleTransform.transform.position = transform.position;
        particleTransform.localScale = Vector3.one;

        // If it has color, set color to particle.
        if (this is IColorable iColorable)
        {
            popParticle.SetColor(iColorable.ColorStyleData);
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

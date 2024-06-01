using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ColorStyle;

public class ParticlePool : MonoBehaviour, IPoolable
{
    [SerializeField] private ParticleSystem _particleSystem;

    private bool _isSpawned = false;
    private float _particleInterval;

    public GameObject PoolObject => gameObject;

    private void Update()
    {
        if (_isSpawned)
        {
            _particleInterval -= Time.deltaTime;

            if (_particleInterval <= 0f)
            {
                _isSpawned = false;
                PoolManager.Instance.Despawn(this);
            }
        }
    }

    public void SetColor(ColorStyleData colorData)
    {
        var main = _particleSystem.main;
        main.startColor = colorData.Color;
    }

    #region IPoolable

    public string GetID()
    {
        return GetType().ToString();
    }

    public void Spawn()
    {
        gameObject.SetActive(true);

        _particleInterval = _particleSystem.main.startLifetime.constant;
        _particleSystem.Play();

        _isSpawned = true;
    }

    public void Despawn()
    {
        transform.SetParent(null);
        gameObject.SetActive(false);
    }

    #endregion
}

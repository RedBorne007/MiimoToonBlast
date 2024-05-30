using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();

                if (_instance == null)
                {
                    GameObject instanceObj = new GameObject(typeof(T).ToString());
                    _instance = instanceObj.AddComponent<T>();
                }
            }

            return _instance;
        }
    }
}

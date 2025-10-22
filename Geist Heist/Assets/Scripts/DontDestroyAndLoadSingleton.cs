/*****************************************************************************
// File Name :         DontDestroyOnLoadSingleton.cs
// Author :            Kyle Grenier, Toby Schamberger
// Creation Date :     09/29/2021
//
// Brief Description : Defines a class with a single instance. Becomes dont destroy 
// on load.
*****************************************************************************/
using UnityEngine;

public abstract class DontDestroyOnLoadSingleton<T> : MonoBehaviour where T : DontDestroyOnLoadSingleton<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = (T)this;
        }
        else
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
    }
}

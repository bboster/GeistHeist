/*****************************************************************************
// File Name :         Singleton.cs
// Author :            Kyle Grenier
// Creation Date :     09/29/2021
//
// Brief Description : Defines a class with a single instance.

note from toby: this is the most excessive piece of code ive ever seen i think 
its so funny.

shoutout kyle and oos, this script is so silly
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
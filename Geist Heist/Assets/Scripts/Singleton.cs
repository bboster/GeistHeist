/*****************************************************************************
// File Name :         Singleton.cs
// Author :            Kyle Grenier
// Creation Date :     09/29/2021
//
// Brief Description : Defines a class with a single Instance.

note from toby: this is the most excessive piece of code ive ever seen i think 
its so funny.

shoutout kyle and oos, this script is so silly
*****************************************************************************/
using NaughtyAttributes;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    [Foldout("Singleton Settings")]
    [SerializeField] private bool destroyGameObject = false; 

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
            Destroy(destroyGameObject ? this.gameObject : this);
        }
    }
}
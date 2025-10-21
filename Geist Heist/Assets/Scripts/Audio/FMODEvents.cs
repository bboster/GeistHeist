using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference TestA { get; private set; }
    [field: SerializeField] public EventReference TestB { get; private set; }
    [field: SerializeField] public EventReference TestC { get; private set; }
    

    public static FMODEvents instance { get; private set; }
    
    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEvents in the scene");
        }
        instance = this;
    }

}

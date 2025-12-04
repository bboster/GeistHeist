
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

public class FMODEvents : Singleton<FMODEvents>
{
    [field: Header("BGM")]
    [field: SerializeField] public EventReference LevelBGM { get; private set; }
    [field: SerializeField] public EventReference GlobeBGM { get; private set; }
    [field: SerializeField] public EventReference HubBGM { get; private set; }
    [field: SerializeField] public EventReference MenuBGM { get; private set; }


    [field: Header("Enemy SFX")]
    [field: SerializeField] public EventReference GuardReactions { get; private set; }
    [field: SerializeField] public EventReference GuardRun { get; private set; }
    [field: SerializeField] public EventReference GuardWalk { get; private set; }
    [field: SerializeField] public EventReference PlayerSpotted { get; private set; }

    [field: Header("Environment SFX")]

    [field: Header("Interface SFX")]
    [field: SerializeField] public EventReference PossessionLow { get; private set; }
    [field: SerializeField] public EventReference PossessionOut { get; private set; }
    [field: SerializeField] public EventReference PossessionRefill { get; private set; }

    [field: Header("Interface SFX")]
    [field: SerializeField] public EventReference PossessionEnter { get; private set; }
    [field: SerializeField] public EventReference PossessionExit { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        base.Awake();
        
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEvents in the scene");
        }
        instance = this;
    }

}

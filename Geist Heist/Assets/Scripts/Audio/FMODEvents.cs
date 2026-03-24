
using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

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
    [field: SerializeField] public EventReference PAJingle { get; private set; }
    [field: SerializeField] public EventReference PALines { get; private set; }

    [field: Header("Interface SFX")]
    [field: SerializeField] public EventReference PossessionLow { get; private set; }
    [field: SerializeField] public EventReference PossessionOut { get; private set; }
    [field: SerializeField] public EventReference PossessionRefill { get; private set; }
    [field: SerializeField] public EventReference UIClick { get; private set; }
    [field: SerializeField] public EventReference UIHover { get; private set; }

    [field: Header("Player SFX")]
    [field: SerializeField] public EventReference PossessionEnter { get; private set; }
    [field: SerializeField] public EventReference PossessionExit { get; private set; }
    [field: SerializeField] public EventReference PlayerMovement { get; private set; }
    [field: SerializeField] public EventReference OllieLines { get; private set; }

    [field: Header("Prop SFX")]
    [field: SerializeField] public EventReference CanBounce { get; private set; }
    [field: SerializeField] public EventReference CanCharge { get; private set; }
    [field: SerializeField] public EventReference CanShot { get; private set; }
    [field: SerializeField] public EventReference CarWind { get; private set; }
    [field: SerializeField] public EventReference CarGo { get; private set; }
    [field: SerializeField] public EventReference CarBump { get; private set; }

    [field: Header("Prop SFX")]
    [field: SerializeField] public EventReference DoorOpen { get; private set; }
    [field: SerializeField] public EventReference DoorLocked { get; private set; }
}

/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 10/02/25
 * Summary: Handles initialization of the enemy and activating/deactivating and switching behaviors.
 */

using System;
using System.Collections;
using System.ComponentModel;
using UnityEngine;
using GuardUtilities;
using NaughtyAttributes;
using UnityEngine.Events;
using FMODUnity;
using FMOD.Studio;

public class GuardController : MonoBehaviour
{
    #region Variable Declarations

    private bool changingBehaviors = false;

    [Header("Design Values")]
    [SerializeField] private PatrolPath path;
    public PatrolPath Path { get { return path; } }
    [Tooltip("The location a guard will return to by default")]
    [Required] public Transform ReturnLocation;
    [Tooltip("The rotation the guard should face by default, match this to its placement in the level")]
    public float DefaultRotation;
    [Tooltip("The time between each footstep sound effect")]
    [SerializeField] private float footstepDelay;

    [Header("Behaviors")]
    [Tooltip("Default behavior for the enemy")]
    [Required] public Behavior DefaultBehavior;

    [SerializeField] public Behavior currentBehavior;

    private Coroutine activeBehaviorLoop;

    [SerializeField] private Priority currentPriority;

    [Header("Programming")]
    [SerializeField] private bool showProgrammingValues;

    [ShowIf("showProgrammingValues")]
    [SerializeField] private Animator animator;


    public Vector3 SearchLocation; //TEMP VAR UNTIL I FIND A BETTER WAY TO PASS A SEARCH LOCATION TO A BEHAVIOR

    [HideInInspector] public UnityEvent<GuardStates> OnBehaviorStarted= new();

    //sfx
    private EventInstance guardWalkSFX;
    private EventInstance guardRunSFX;

    #endregion

    #region Getters

    /// <summary>
    /// Returns a reference to the guard GameObject this script is attached to
    /// </summary>
    /// <returns></returns>
    public GameObject GetGuard()
    {
        return gameObject;
    }

    /// <summary>
    /// Returns a reference to the guard's Animator component
    /// </summary>
    /// <returns></returns>
    public Animator GetAnimator()
    {
        return animator;
    }

    #endregion

    /// <summary>
    /// Initializes the enemy. Returns true if successful, false if unsuccessful.
    /// </summary>
    /// <returns></returns>
    public bool InitializeGuard()
    {
        if (CheckBehaviors() == false)
            return false;

        currentBehavior = Instantiate(DefaultBehavior);
        StartBehavior();

        return true;
    }

    #region SFX Functions

    private void Start()
    {
        //only for sfx for now
        guardWalkSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.GuardWalk);
        guardRunSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.GuardRun);
    }

    /// <summary>
    /// Plays footstep sound effects while in certain behaviors
    /// </summary>
    /// <returns></returns>
    private void Update()
    {
        //only for sfx for now
        guardWalkSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));
        guardRunSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));

        if (currentBehavior.StateName == GuardStates.chase)
        {
            guardWalkSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PLAYBACK_STATE playbackState;
            guardRunSFX.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                guardRunSFX.start();
            }
        }
        else if (currentBehavior.StateName == GuardStates.patrol || currentBehavior.StateName == GuardStates.returnToPath)
        {
            guardRunSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            PLAYBACK_STATE playbackState;
            guardWalkSFX.getPlaybackState(out playbackState);
            if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
            {
                guardWalkSFX.start();
            }
        }
        else
        {
            guardRunSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            guardWalkSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    #endregion

    #region Behavior Functions

    /// <summary>
    /// Checks to make sure that behaviors are set up properly.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private bool CheckBehaviors()
    {
        if (DefaultBehavior == null)
        {
            Debug.LogError(gameObject.name + " HAS NO DEFAULT BEHAVIOR");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Swaps the currently running behavior.
    /// </summary>
    /// <param name="newBehavior"></param>
    public void ChangeBehavior(GuardStates state)
    {
        Debug.Log(state);

        StopBehavior();
        currentBehavior = Instantiate(Singleton<BehaviorDatabase>.Instance.GetBehavior(state));
        StartBehavior();
    }
    
    /// <summary>
    /// Swaps the currently running behavior if priority is higher
    /// </summary>
    /// <param name="state"></param>
    /// <param name="priority"></param>
    public void ChangeBehaviorConditional(GuardStates state, Priority priority)
    {
        if(priority > currentPriority)
        {
            StopBehavior();
            currentBehavior = Instantiate(Singleton<BehaviorDatabase>.Instance.GetBehavior(state));
            StartBehavior();
        }
    }

    /// <summary>
    /// Handles edge case for vision breaking after attacking
    /// </summary>
    public void OnVisionBroken()
    {
        if (currentBehavior.StateName == GuardStates.returnToPath)
            return;

        ChangeBehavior(GuardStates.visionBreak);
    }

    #endregion

    #region Start and Stop Behavior

    /// <summary>
    /// Starts the currently selected behavior
    /// </summary>
    public void StartBehavior()
    {
        if (currentBehavior != null)
        {
            currentBehavior.InitializeBehavior(gameObject);
            //GetComponent<StateText>().ChangeText(currentBehavior.StateName);
            currentPriority = currentBehavior.Priority;
            activeBehaviorLoop = StartCoroutine(currentBehavior.BehaviorLoop());
            OnBehaviorStarted.Invoke(currentBehavior.StateName);
        }
    }

    /// <summary>
    /// Stops the currently running behavior
    /// </summary>
    public void StopBehavior()
    {
        if(currentBehavior != null)
            currentBehavior.StopBehavior();

        if(activeBehaviorLoop != null)
        {
            StopCoroutine(activeBehaviorLoop);
            activeBehaviorLoop = null;
        }

        guardRunSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        guardWalkSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    #endregion

    #region RecieveStimulus Functions

    /// <summary>
    /// Recieves a stimulus and determines whether to change behaviors
    /// </summary>
    /// <param name="stimulus"></param>
    public void RecieveStimulus(Stimulus stimulus, GuardStates stateToChangeTo)
    {
        if(stimulus.GetPriority() > currentPriority)
        {
            ChangeBehavior(stateToChangeTo);
        }
    }

    /// <summary>
    /// Recieves a stimulus and determines whether to change behaviors
    /// </summary>
    /// <param name="stimulus"></param>
    /// <param name="stateToChangeTo"></param>
    /// <param name="stimulusLocation"></param>
    public void RecieveStimulus(Stimulus stimulus, GuardStates stateToChangeTo, Vector3 stimulusLocation)
    {
        if(stimulus.GetPriority() > currentPriority)
        {
            SearchLocation = stimulusLocation;
            ChangeBehavior(stateToChangeTo);
        }
    }

    #endregion

    public void OnDestroy()
    {
        guardRunSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        guardWalkSFX.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

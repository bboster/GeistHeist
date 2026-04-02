/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 11/15/25
 * Summary: Handles initialization of the enemy and activating/deactivating and switching behaviors.
 */

using System;
using System.ComponentModel;
using UnityEngine;
using GuardUtilities;
using NaughtyAttributes;
using UnityEngine.Events;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.AI;
//using UnityEditor.ShaderGraph.Internal;

public class GuardController : MonoBehaviour
{
    #region Variable Declarations

    private bool changingBehaviors = false;
    private NavMeshAgent thisAgent;
    private float defaultAngularSpeed;
    private float defaultAcceleration;
    [HideInInspector] public float AngularSpeed;
    [HideInInspector] public float Acceleration;

    public GameObject visionConeRotator;

    [SerializeField, BoxGroup("Design Values")] private PatrolPath path;
    public PatrolPath Path { get { return path; } }
    [Tooltip("The location a guard will return to by default")]
    [Required, BoxGroup("Design Values")] public Transform ReturnLocation;
    [Tooltip("The rotation the guard should face by default, match this to its placement in the level")]
    [BoxGroup("Design Values")] public float DefaultRotation;
    [Tooltip("The index of the point the guard should start at.")]
    [BoxGroup("Design Values")] public int StartIndex = 0;

    [Tooltip("Default behavior for the enemy"), Expandable]
    [Required, BoxGroup("Behaviors")] public Behavior DefaultBehavior;
    [Expandable]
    [SerializeField, BoxGroup("Behaviors")] public Behavior currentBehavior;

    private Coroutine activeBehaviorLoop;

    [SerializeField, BoxGroup("Behaviors")] private int currentPriority;
    [HideInInspector] public UnityEvent<String> VoiceClipPlayed = new();
    [HideInInspector] public UnityEvent VoiceClipStopped = new();

    [Tooltip("How far left the guard can rotate from 0 degrees."), Foldout("Stationary Guards Only")]
    public float leftRotationValue;
    [Tooltip("How far right the guard can rotate from 0 degrees."), Foldout("Stationary Guards Only")]
    public float rightRotationValue;
    [Tooltip("How fast the guard will rotate."), Foldout("Stationary Guards Only")]
    public float coneRotationSpeed;

    [Foldout("Programming Values")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem dustParticles;
    [SerializeField] private ParticleSystem smokeParticlesL;
    [SerializeField] private ParticleSystem smokeParticlesR;
    public Animator searchAnimator;

    [HideInInspector] public Vector3 SearchLocation; //TEMP VAR UNTIL I FIND A BETTER WAY TO PASS A SEARCH LOCATION TO A BEHAVIOR

    [HideInInspector] public UnityEvent<GuardStates> OnBehaviorStarted = new();

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
        thisAgent = GetComponent<NavMeshAgent>();
        defaultAngularSpeed = thisAgent.angularSpeed;
        defaultAcceleration = thisAgent.acceleration;

        //only for sfx for now
        guardWalkSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GuardWalk);
        guardRunSFX = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GuardRun);
    }

    /// <summary>
    /// Plays footstep sound effects while in certain behaviors
    /// </summary>
    /// <returns></returns>
    private void Update()
    {
        FastRotate();

        //only for sfx for now
        guardWalkSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));
        guardRunSFX.set3DAttributes(RuntimeUtils.To3DAttributes(GetComponent<Transform>(), GetComponent<Rigidbody>()));

        if (currentBehavior.StateName == GuardStates.chase)
        {
            if (dustParticles != null && !dustParticles.isPlaying)
            {
                dustParticles.Play();
            }

            if ((smokeParticlesL != null && smokeParticlesR != null) && (!smokeParticlesL.isPlaying && !smokeParticlesR.isPlaying))
            {
                smokeParticlesL.Play();
                smokeParticlesR.Play();

            }

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
            if (dustParticles != null && dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }

            if ((smokeParticlesL != null && smokeParticlesR != null) && (smokeParticlesL.isPlaying && smokeParticlesR.isPlaying))
            {
                smokeParticlesL.Stop();
                smokeParticlesR.Stop();

            }

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
            if (dustParticles != null && dustParticles.isPlaying)
            {
                dustParticles.Stop();
            }

            if ((smokeParticlesL != null && smokeParticlesR != null) && (smokeParticlesL.isPlaying && smokeParticlesR.isPlaying))
            {
                smokeParticlesL.Stop();
                smokeParticlesR.Stop();
            }

            guardRunSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            guardWalkSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }
    }

    public void GuardTalking(String Caption)
    {
        VoiceClipPlayed.Invoke(Caption);
    }

    public void GuardStopsTalking()
    {
        VoiceClipStopped.Invoke();
    }

    private void OnDestroy()
    {
        guardRunSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        guardWalkSFX.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
    #endregion

    /// <summary>
    /// Makes the guard rotate faster
    /// </summary>
    private void FastRotate()
    {
        if (thisAgent.updateRotation)
        {
            thisAgent.angularSpeed = AngularSpeed;
            thisAgent.acceleration = Acceleration;
        }
        else
        {
            thisAgent.angularSpeed = defaultAngularSpeed;
            thisAgent.acceleration = defaultAcceleration;
        }
    }

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
        //Debug.Log(state);

        StopBehavior();
        currentBehavior = Instantiate(Singleton<BehaviorDatabase>.Instance.GetBehavior(state));
        StartBehavior();
    }

    /// <summary>
    /// Swaps the currently running behavior if priority is higher
    /// </summary>
    /// <param name="state"></param>
    /// <param name="priority"></param>
    public void ChangeBehaviorConditional(GuardStates state, int priority)
    {
        if (priority > currentPriority)
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

        Behavior b = Singleton<BehaviorDatabase>.Instance.GetBehavior(GuardStates.visionBreak);

        ChangeBehaviorConditional(GuardStates.visionBreak, b.Priority);
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
        if (currentBehavior != null)
            currentBehavior.StopBehavior();

        if (activeBehaviorLoop != null)
        {
            StopCoroutine(activeBehaviorLoop);
            activeBehaviorLoop = null;
        }
    }

    #endregion

    #region RecieveStimulus Functions

    /// <summary>
    /// Recieves a stimulus and determines whether to change behaviors
    /// </summary>
    /// <param name="stimulus"></param>
    public void RecieveStimulus(Stimulus stimulus, GuardStates stateToChangeTo)
    {
        if (stimulus.GetPriority() > currentPriority)
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
        if (stimulus.GetPriority() > currentPriority)
        {
            SearchLocation = stimulusLocation;
            ChangeBehavior(stateToChangeTo);
        }
    }

    #endregion
}

/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 10/02/25
 * Summary: Handles initialization of the enemy and activating/deactivating and switching behaviors.
 */

using System;
using System.ComponentModel;
using UnityEngine;
using GuardUtilities;

public class GuardController : MonoBehaviour
{
    #region Variable Declarations

    [Header("Design Values")]
    [SerializeField] private PatrolPath path;
    public PatrolPath Path { get { return path; } }

    [Header("Behaviors")]
    [SerializeField, Tooltip("Default behavior for the enemy")]
    private Behavior defaultBehavior;

    private Behavior currentBehavior;

    private Coroutine activeBehaviorLoop;

    private Priority currentPriority;

    #endregion

    //DELETE THIS LATER IN FAVOR OF AN OVERALL ENEMY HANDLER
    private void Start()
    {
        InitializeGuard();
    }

    /// <summary>
    /// Initializes the enemy. Returns true if successful, false if unsuccessful.
    /// </summary>
    /// <returns></returns>
    public bool InitializeGuard()
    {
        if (CheckBehaviors() == false)
            return false;

        currentBehavior = defaultBehavior;
        StartBehavior();

        return true;
    }

    /// <summary>
    /// Checks to make sure that behaviors are set up properly.
    /// </summary>
    /// <exception cref="Exception"></exception>
    private bool CheckBehaviors()
    {
        if (defaultBehavior == null)
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
        StopBehavior();
        currentBehavior = BehaviorDatabase.instance.GetBehavior(state);
        StartBehavior();
    }

    /// <summary>
    /// Starts the currently selected behavior
    /// </summary>
    private void StartBehavior()
    {
        currentBehavior.InitializeBehavior(gameObject);
        activeBehaviorLoop = StartCoroutine(currentBehavior.BehaviorLoop());
    }

    /// <summary>
    /// Stops the currently running behavior
    /// </summary>
    private void StopBehavior()
    {
        currentBehavior.StopBehavior();

        if(activeBehaviorLoop != null)
        {
            StopCoroutine(activeBehaviorLoop);
            activeBehaviorLoop = null;
        }
    }

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
    /// Returns a reference to the guard GameObject this script is attached to
    /// </summary>
    /// <returns></returns>
    public GameObject GetGuard()
    {
        return gameObject;
    }
}

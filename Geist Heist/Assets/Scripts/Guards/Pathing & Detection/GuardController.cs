/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/30/25
 * Summary: Handles initialization of the enemy and activating/deactivating behaviors.
 * To Do: Build in a listener for an action that tells the controller when the player possesses and object.
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
    public void ChangeBehavior(GuardData.GuardStates state)
    {
        StopBehavior();
        currentBehavior = BehaviorDatabase.instance.GetBehavior(state);
        StartBehavior();

        //currentBehavior.StopBehavior();
        //currentBehavior = behaviors[behaviorIndex];
        //currentBehavior.StartBehavior();
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
        StopCoroutine(activeBehaviorLoop);
        activeBehaviorLoop = null;
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

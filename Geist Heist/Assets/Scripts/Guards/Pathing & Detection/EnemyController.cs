/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/18/25
 * Summary: Handles initialization of the enemy and activating/deactivating behaviors.
 * To Do: Build in a listener for an action that tells the controller when the player possesses and object.
 */

using System;
using System.ComponentModel;
using UnityEngine;
using EnemyUtilities;

public class EnemyController : MonoBehaviour
{
    #region Variable Declarations

    [Header("Behaviors")]
    [SerializeField, Tooltip("Default behavior for the enemy")]
    private Behavior defaultBehavior;

    [Tooltip("List of all behaviors the enemy is able to run.")]
    [SerializeField] private Behavior[] behaviors;

    private Behavior currentBehavior;

    #endregion

    //DELETE THIS LATER IN FAVOR OF AN OVERALL ENEMY HANDLER
    private void Start()
    {
        InitializeEnemy();
    }

    /// <summary>
    /// Initializes the enemy. Returns true if successful, false if unsuccessful.
    /// </summary>
    /// <returns></returns>
    public bool InitializeEnemy()
    {
        if (CheckBehaviors() == false)
            return false;

        currentBehavior = defaultBehavior;
        defaultBehavior.StartBehavior();

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
    public void ChangeBehavior(int behaviorIndex)
    {
        currentBehavior.StopBehavior();
        currentBehavior = behaviors[behaviorIndex];
        currentBehavior.StartBehavior();
    }


}

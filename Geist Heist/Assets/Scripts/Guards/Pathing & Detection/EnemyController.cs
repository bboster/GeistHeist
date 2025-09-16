using System;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variable Declarations

    [Header("Behaviors")]
    [SerializeField, Tooltip("Default behavior for the enemy")]
    private Behavior defaultBehavior;
    [SerializeField] private Behavior moveBehavior;
    [SerializeField] private Behavior chaseBehavior;
    [SerializeField] private Behavior attackBehavior;

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
}

/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/18/25
 * Summary: Contains utility functions for enemies to use while running movement behavior.
 */

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : Behavior
{
    protected bool calculatingMovement = false;

    [Header("Enemy Movement Values")]
    [Tooltip("Distance from destination at which movement is considered complete.")]
    [SerializeField] private float moveCompletionThreshold;

    protected NavMeshAgent thisAgent;

    private void Awake()
    {
        thisAgent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Sets the destination for the enemy to move to.
    /// </summary>
    /// <param name="destination"></param>
    protected void MoveToPoint(Vector3 destination)
    {
        thisAgent.SetDestination(destination);
    }

    /// <summary>
    /// Checks to see if the enemy has reached the end of its path.
    /// </summary>
    /// <returns></returns>
    protected bool CheckPathCompletion()
    {
        if (thisAgent.hasPath && thisAgent.remainingDistance <= moveCompletionThreshold)
        {
            //Debug.Log("PATH COMPLETE");
            return true;
        }
        else
            return false;
    }
}

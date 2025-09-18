/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/18/25
 * Summary: Runs the behavior for the patrol movement type for enemies. BehaviorLoop runs every frame
 * and controls when certain aspects of the behavior are triggered. 
 */

using System.Collections;
using UnityEngine;

public class PatrolMovement : EnemyMovement
{
    private int currentPathIndex = 0;

    [Header("Patrol Behavior Values")]
    [SerializeField] private PatrolPath currentPatrolPath;

    #region Start and Stop Behavior

    /// <summary>
    /// Starts the behavior. Should be called by the controller when a behavior starts or changes.
    /// </summary>
    public override void StartBehavior()
    {
        currentLoop = StartCoroutine(BehaviorLoop());
        MoveToPoint(GetNextPoint());
    }

    /// <summary>
    /// Overrides the default StopBehavior function to pause all movement the enemy is doing.
    /// </summary>
    public override void StopBehavior()
    {
        if (currentLoop == null)
            return;

        thisAgent.isStopped = true;

        StopCoroutine(currentLoop);
        currentLoop = null;
    }

    #endregion

    /// <summary>
    /// Controls the overall logic for the behavior.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (CheckPathCompletion() == true && calculatingMovement == false)
            {
                calculatingMovement = true;
                MoveToPoint(GetNextPoint());
                thisAgent.isStopped = false;
            }
            else
            {
                calculatingMovement = false;
                //Debug.Log(gameObject.name + " IS PATROLLING");
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Gets the next point on the patrol path.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetNextPoint()
    {
        Transform destinationTransform = currentPatrolPath.GetPoint(currentPathIndex);

        IncrementPathIndex();

        return destinationTransform.position;
    }

    /// <summary>
    /// Increments currentPathIndex, resets it to the start if the end of the array is reached.
    /// </summary>
    private void IncrementPathIndex()
    {
        if (currentPathIndex >= currentPatrolPath.GetPatrolPoints.Length - 1)
            currentPathIndex = 0;
        else
            currentPathIndex++;
    }
}

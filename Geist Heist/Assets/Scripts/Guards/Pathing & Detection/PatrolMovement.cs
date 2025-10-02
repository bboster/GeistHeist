/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/30/25
 * Summary: Runs the behavior for the patrol movement type for enemies. BehaviorLoop runs every frame
 * and controls when certain aspects of the behavior are triggered. 
 */

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "New Patrol Behavior", menuName = "Guard Behaviors/New Patrol Behavior")]
public class PatrolMovement : GuardMovement
{
    private int currentPathIndex = 0;

    [Header("Patrol Behavior Values")]
    [SerializeField] private PatrolPath currentPatrolPath;

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        currentPatrolPath = selfRef.GetComponent<GuardController>().Path;
        MoveToPoint(GetNextPoint());
        thisAgent.isStopped = false;
    }

    /// <summary>
    /// Controls the overall logic for the behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
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

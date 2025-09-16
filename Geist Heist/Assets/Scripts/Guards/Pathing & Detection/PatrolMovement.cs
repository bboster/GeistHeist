using System.Collections;
using UnityEngine;

public class PatrolMovement : EnemyMovement
{
    private bool calculatingMovement = false;

    private int currentPathIndex = 0;

    [SerializeField] private PatrolPath currentPatrolPath;

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
                yield return new WaitForEndOfFrame();
            }
            else
            {
                calculatingMovement = false;
                Debug.Log(gameObject.name + " IS PATROLLING");
                yield return new WaitForEndOfFrame();
            }
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

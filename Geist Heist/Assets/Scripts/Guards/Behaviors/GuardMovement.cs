/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/30/25
 * Summary: Contains utility functions for enemies to use while running movement behavior.
 */

using NaughtyAttributes;
using NUnit.Framework.Constraints;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuardMovement : Behavior
{
    protected bool calculatingMovement = false;

    [Header("Guard Movement Values")]
    [Tooltip("Distance from destination at which movement is considered complete.")]
    [SerializeField] private float moveCompletionThreshold;

    protected NavMeshAgent thisAgent;

#if UNITY_EDITOR
    [ProgressBar("Path Completion", 100, EColor.Blue)]
    public float pathProgress;
    [HideInInspector] public Coroutine ProgressCoroutine;
    private MonoBehaviour coroutineRunner;
#endif

    /// <summary>
    /// Initializes the behavior.
    /// </summary>
    /// <param name="selfRef"></param>
    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        thisAgent = selfRef.GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Sets the destination for the enemy to move to.
    /// </summary>
    /// <param name="destination"></param>
    protected void MoveToPoint(Vector3 destination)
    {
        thisAgent.SetDestination(destination);
    }

#if UNITY_EDITOR
    /// <summary>
    /// Updates the progress bar to display progress along a path
    /// </summary>
    /// <returns></returns>
    private IEnumerator PathProgress()
    {
        float pathDistance = 0;

        if(thisAgent.path != null)
        {
            for (int i = 1; i < thisAgent.path.corners.Length; i++)
            {
                pathDistance += Vector3.Distance(thisAgent.path.corners[i - 1], thisAgent.path.corners[i]);
            }

            for (; ; )
            {

                pathProgress = (pathDistance - thisAgent.remainingDistance) / pathDistance;
                yield return new WaitForEndOfFrame();
            }
        }
    }
#endif

    /// <summary>
    /// Checks to see if the enemy has reached the end of its path.
    /// </summary>
    /// <returns></returns>
    protected bool CheckPathCompletion()
    {
        if (thisAgent.hasPath && thisAgent.remainingDistance <= moveCompletionThreshold)
        {
            return true;
        }
        else
            return false;
    }
}

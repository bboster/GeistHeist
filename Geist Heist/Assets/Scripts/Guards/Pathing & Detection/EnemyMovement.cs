using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : Behavior
{
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
        if (thisAgent.remainingDistance <= moveCompletionThreshold)
        {
            Debug.Log("PATH COMPLETE");
            return true;
        }
        else
            return false;
    }
}

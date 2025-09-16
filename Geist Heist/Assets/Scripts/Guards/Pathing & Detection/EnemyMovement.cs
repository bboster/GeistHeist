using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : Behavior
{
    protected NavMeshAgent thisAgent;

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
    private IEnumerator CheckPathCompletion()
    {
        for(; ; )
        {


            yield return new WaitForEndOfFrame();
        }
    }
}

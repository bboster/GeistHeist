using System.Collections;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    [SerializeField] private Transform[] patrolPoints;

    public Transform[] GetPatrolPoints { get => patrolPoints; }

    /// <summary>
    /// Gets a point at index in the array patrolPoints.
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Transform GetPoint(int index)
    {
        if(index >= patrolPoints.Length)
        {
            Debug.LogError("INDEX IS GREATER THAN LENGTH OF ARRAY");
            return patrolPoints[0];
        }
        
        return patrolPoints[index];
    }
}

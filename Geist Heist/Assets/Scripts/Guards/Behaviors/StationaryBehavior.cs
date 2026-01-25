/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/23/25
 * Last Edited: 10/23/25
 * Summary: Runs behavior for a stationary guard.
 */
using System.Collections;
using System.Linq.Expressions;
//using UnityEditor.Splines;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Splines.Interpolators;

[CreateAssetMenu(fileName = "New Stationary Behavior", menuName = "Guard Behaviors/New Stationary Behavior")]
public class StationaryBehavior : Behavior
{
    private bool isRotatingRight = true;

    #region Might Be Used Later

    /*[Header("Rotation Values")]
    [Tooltip("How far left the guard can rotate from 0 degrees.")]
    [SerializeField] private float leftRotationValue;
    [Tooltip("How far right the guard can rotate from 0 degrees.")]
    [SerializeField] private float rightRotationValue;
    [Tooltip("How fast the guard will rotate.")]
    [SerializeField] private float rotationSpeed;
    [Tooltip("How long it should take for the guard to rotate from the center to one side")]
    [SerializeField] private float rotationTime;*/

    #endregion 
    //are these going to be needed

    #region Initialize and Stop Behavior

    /// <summary>
    /// Initializes the behavior
    /// </summary>
    /// <param name="selfRef"></param>
    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        selfRef.GetComponent<NavMeshAgent>().isStopped = true;
        selfRef.GetComponent<NavMeshAgent>().enabled = false;

        Vector3 rotation = selfRef.transform.rotation.eulerAngles;
        rotation.y = contRef.DefaultRotation - selfRef.transform.rotation.eulerAngles.y;

        selfRef.transform.Rotate(rotation);
    }

    /// <summary>
    /// Stops the behavior
    /// </summary>
    public override void StopBehavior()
    {
        selfRef.GetComponent<NavMeshAgent>().enabled = true;
        base.StopBehavior();
    }

    #endregion

    public override IEnumerator BehaviorLoop()
    {
        Vector3 rotation = selfRef.transform.rotation.eulerAngles;
        rotation.y = contRef.DefaultRotation - selfRef.transform.rotation.eulerAngles.y;

        selfRef.transform.Rotate(rotation);

        #region Might be used later

        /*Vector3 rightRotation = selfRef.transform.rotation.eulerAngles;
        rightRotation.y += rightRotationValue;
        Vector3 leftRotation = selfRef.transform.rotation.eulerAngles;
        leftRotation.y += leftRotationValue;

        Vector3 rotationGoal = rightRotation;
        Vector3 rotationDefault = selfRef.transform.rotation.eulerAngles;

        for (; ;)
        {
            Vector3 rotation = Vector3.Slerp(rotationDefault, rotationGoal, rotationSpeed * Time.deltaTime);
            selfRef.transform.Rotate(rotation);

            if(selfRef.transform.rotation.eulerAngles.y <= rotationGoal.y && isRotatingRight == true)
            {
                rotationGoal = leftRotation;
                isRotatingRight = false;
            }
            else if(selfRef.transform.rotation.eulerAngles.y >= rotationGoal.y && isRotatingRight == false)
            {
                rotationGoal = rightRotation;
                isRotatingRight = true;
            }

            yield return new WaitForEndOfFrame();
        }*/

        #endregion
        //are these going to be needed if not we can remove them

        yield return new WaitForEndOfFrame();
    }
}

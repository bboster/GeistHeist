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

    [Header("Rotation Values")]
    [Tooltip("How far left the guard can rotate from 0 degrees.")]
    [SerializeField] private float leftRotationValue;
    [Tooltip("How far right the guard can rotate from 0 degrees.")]
    [SerializeField] private float rightRotationValue;
    [Tooltip("How fast the guard will rotate.")]
    [SerializeField] private float coneRotationSpeed;

    #endregion 

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
        contRef.visionConeRotator.transform.localRotation = Quaternion.identity;
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
        leftRotationValue = Mathf.Abs(contRef.leftRotationValue);
        rightRotationValue = Mathf.Abs(contRef.rightRotationValue);
        coneRotationSpeed = contRef.coneRotationSpeed;


        Vector3 rotation = contRef.visionConeRotator.transform.localRotation.eulerAngles;
        //rotation.y = contRef.DefaultRotation - visionCone.transform.rotation.eulerAngles.y;

        //contRef.visionConeRotator.transform.localRotation = Quaternion.Euler(rotation);

        #region Might be used later

        Vector3 rightRotation = contRef.visionConeRotator.transform.localRotation.eulerAngles;
        rightRotation.y += rightRotationValue;
        Vector3 leftRotationChecker = contRef.visionConeRotator.transform.localRotation.eulerAngles;
        leftRotationChecker.y -= leftRotationValue;
        leftRotationChecker.y = leftRotationChecker.y + 360;
        Vector3 LeftRotationGoal = contRef.visionConeRotator.transform.localRotation.eulerAngles;
        LeftRotationGoal.y -= leftRotationValue;

        Vector3 rotationGoal = rightRotation;
        Vector3 rotationDefault = contRef.visionConeRotator.transform.localRotation.eulerAngles;
        Vector3 rotationChecker = rightRotation;

        for (; ;)
        {
            Vector3 currentRotation = Vector3.Slerp(rotationDefault, rotationGoal, coneRotationSpeed * Time.deltaTime);
            contRef.visionConeRotator.transform.localRotation = 
                Quaternion.Euler(currentRotation + contRef.visionConeRotator.transform.localRotation.eulerAngles);

            if(contRef.visionConeRotator.transform.localRotation.eulerAngles.y >= rotationChecker.y && 
                isRotatingRight == true && contRef.visionConeRotator.transform.localRotation.eulerAngles.y < 180)
            {
                rotationGoal = LeftRotationGoal;
                rotationChecker = leftRotationChecker;
                isRotatingRight = false;
            }
            else if(contRef.visionConeRotator.transform.localRotation.eulerAngles.y <= rotationChecker.y && 
                isRotatingRight == false && contRef.visionConeRotator.transform.localRotation.eulerAngles.y >180)
            {
                rotationGoal = rightRotation;
                rotationChecker = rightRotation;
                isRotatingRight = true;
            }

            yield return new WaitForEndOfFrame();
        }

        #endregion
    }
}

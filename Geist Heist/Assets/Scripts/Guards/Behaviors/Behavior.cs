/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/18/25
 * Summary: Basic utility to build behaviors for enemies off of.
 */

using System;
using System.Collections;
using UnityEngine;
using GuardUtilities;
using UnityEngine.AI;
using NaughtyAttributes;

public class Behavior : ScriptableObject
{
    #region Variable Declarations

    [ProgressBar(100f, EColor.Blue)]
    public float progress;

    [Tooltip("The speed the guard will travel at while performing this behavior")]
    [SerializeField, Foldout("Base Values")] private float speed;
    [Tooltip("The speed the guard will rotate at")]
    [Foldout("Base Values")] public float rotationSpeed;
    [Tooltip("How fast the rotation gets up to speed")]
    [Foldout("Base Values")] public float rotationAcceleration;
    [Tooltip("The name of the state this behavior executes")]
    [Foldout("Base Values")] public GuardStates StateName;
    [Tooltip("Controls what states this behavior can override")]
    [Foldout("Base Values"), MaxValue(10), MinValue(1)] public int Priority;
    [Tooltip("The animator controller for the behavior. Can be left blank if there are no animations")]
    [Foldout("Base Values")] public RuntimeAnimatorController stateController;
    [Tooltip("The color the flashlight should be during this state")]
    [Foldout("Base Values")] public Color FlashlightColor;

    [Tooltip("Reference to the gameObject")]
    protected GameObject selfRef;
    [Tooltip("Reference to the guard's controller script")]
    protected GuardController contRef;

    protected Coroutine currentLoop;
    public Coroutine behaviorLoop;
    public Coroutine TimerCoroutine;

    #endregion

    #region StartLoop and StopLoop

    ///Initializes the behavior
    public virtual void InitializeBehavior(GameObject selfRef)
    {
        this.selfRef = selfRef;

        if (stateController != null)
        {
            //selfRef.GetComponent<Animator>().runtimeAnimatorController = stateController;
        }

        contRef = selfRef.GetComponent<GuardController>();
        contRef.AngularSpeed = rotationSpeed;
        contRef.Acceleration = rotationAcceleration;
        selfRef.GetComponent<NavMeshAgent>().speed = speed;
    }

    /// <summary>
    /// Stops the behaviors functions
    /// </summary>
    public virtual void StopBehavior()
    {
        //selfRef.GetComponent<Animator>().StopPlayback();
        //selfRef.GetComponent<Animator>().runtimeAnimatorController = null;
        Destroy(this);
    }

    #endregion

    /// <summary>
    /// Controls the overall flow of the behavior.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            Debug.Log("RUNNING BASE BEHAVIOR SCRIPT");

            yield return new WaitForEndOfFrame();
        }
    }
}

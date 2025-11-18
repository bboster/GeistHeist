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

    [Tooltip("The speed the guard will travel at while performing this behavior")]
    [SerializeField, Foldout("Base Values")] private float speed;
    [Tooltip("The name of the state this behavior executes")]
    [Foldout("Base Values")] public GuardStates StateName;
    [Tooltip("Controls what states this behavior can override")]
    [Foldout("Base Values")] public Priority Priority;
    [Tooltip("The animator controller for the behavior. Can be left blank if there are no animations")]
    [Foldout("Base Values")] public RuntimeAnimatorController stateController;

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
            selfRef.GetComponent<Animator>().runtimeAnimatorController = stateController;
        }

        contRef = selfRef.GetComponent<GuardController>();
        selfRef.GetComponent<NavMeshAgent>().speed = speed;
    }

    /// <summary>
    /// Stops the behaviors functions
    /// </summary>
    public virtual void StopBehavior()
    {
        selfRef.GetComponent<Animator>().StopPlayback();
        selfRef.GetComponent<Animator>().runtimeAnimatorController = null;
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

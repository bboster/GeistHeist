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

    [Header("Base Behavior Values")]
    [SerializeField] private bool showBaseValues;
    [Tooltip("The speed the guard will travel at while performing this behavior")]
    [SerializeField, ShowIf("showBaseValues")] private float speed;
    [ShowIf("showBaseValues")] public GuardStates StateName;
    [ShowIf("showBaseValues")] public Priority Priority;
    [ShowIf("showBaseValues")] public RuntimeAnimatorController stateController;

    protected GameObject selfRef;

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

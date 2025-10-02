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
using UnityEditor.Animations;

public class Behavior : ScriptableObject
{
    protected GameObject selfRef;

    protected Coroutine currentLoop;
    public Coroutine behaviorLoop;
    public Coroutine TimerCoroutine;

    public GuardStates StateName;

    public Priority Priority;

    public AnimatorController stateController;

    #region StartLoop and StopLoop

    ///Initializes the behavior
    public virtual void InitializeBehavior(GameObject selfRef)
    {
        this.selfRef = selfRef;

        if (stateController != null)
        {
            selfRef.GetComponent<Animator>().runtimeAnimatorController = stateController;
        }
    }

    /// <summary>
    /// Stops the behaviors functions
    /// </summary>
    public virtual void StopBehavior()
    {
        selfRef.GetComponent<Animator>().runtimeAnimatorController = null;
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

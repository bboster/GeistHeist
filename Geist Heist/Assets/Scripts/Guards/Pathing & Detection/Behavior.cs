/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited:
 * Summary: Basic utility to build behaviors for enemies off of.
 */

using System;
using System.Collections;
using UnityEngine;
using EnemyUtilities;

public class Behavior : MonoBehaviour
{
    protected Coroutine currentLoop;

    public EnemyData.EnemyStates stateName;

    #region StartLoop and StopLoop

    /// <summary>
    /// Starts the behavior. Should be called by the controller when a behavior starts or changes.
    /// </summary>
    public void StartBehavior()
    {
        currentLoop = StartCoroutine(BehaviorLoop());
    }

    /// <summary>
    /// Stops the current behavior loop.
    /// </summary>
    public virtual void StopBehavior()
    {
        if (currentLoop == null)
            return;

        StopCoroutine(currentLoop);
        currentLoop = null;
    }

    #endregion

    /// <summary>
    /// Controls the overall flow of the behavior.
    /// </summary>
    /// <returns></returns>
    protected virtual IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            Debug.Log("RUNNING BASE BEHAVIOR SCRIPT");

            yield return new WaitForEndOfFrame();
        }
    }
}

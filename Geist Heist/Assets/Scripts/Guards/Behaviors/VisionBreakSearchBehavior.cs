/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/27/25
 * Summary: Search behavior that runs when the player leaves the guard's vision cone
 */

using UnityEngine;
using System.Collections;
using System;
using GuardUtilities;

[CreateAssetMenu(fileName = "New Vision Break Search", menuName = "Guard Behaviors/New Vision Break Search")]
public class VisionBreakSearchBehavior : GuardMovement
{
    private bool atSearchLocation = false;
    private bool searching = false;
    private bool behaviorComplete = false;

    [SerializeField] private float searchLength;

    public Vector3 SearchLocation;

    /// <summary>
    /// Sets unique values required for the vision break behavior.
    /// </summary>
    /// <param name="selfRef"></param>
    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        SearchLocation = PlayerManager.Instance.CurrentObject.transform.position;
        MoveToPoint(SearchLocation);
        thisAgent.isStopped = false;
        behaviorComplete = false;
    }

    /// <summary>
    /// Controls the overall logic for the behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        for (; ; )
        {
            if (CheckPathCompletion() == true && searching == false)
            {
                StartSearch();
            }

            if (behaviorComplete)
            {
                selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.returnToPath);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Runs the search behavior
    /// </summary>
    /// <returns></returns>
    private void StartSearch()
    {
        GuardCoroutineManager.instance.StartBehaviorTimer(searchLength, this);
        selfRef.GetComponent<GuardController>().GetAnimator().SetTrigger("LookingAround");

/*#if UNITY_EDITOR
        selfRef.GetComponent<GuardDebugger>().StartDebugProgress(searchLength, this);
#endif*/
    }

    /// <summary>
    /// Stops the behaviors loop
    /// </summary>
    public override void StopBehavior()
    {
        base.StopBehavior();
        GuardCoroutineManager.instance.StopBehaviorTimer(TimerCoroutine);
        behaviorComplete = true;
        selfRef.GetComponent<GuardController>().GetAnimator().SetTrigger("LookingAround");
        SearchLocation = Vector3.zero;
    }
}

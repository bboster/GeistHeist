/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/02/25
 * Summary: Search behavior that runs when the guard hears something
 */

using GuardUtilities;
using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "New Search Behavior", menuName = "Guard Behaviors/New Search Behavior")]
public class SearchBehavior : GuardMovement
{
    private bool atSearchLocation = false;
    private bool searching = false;
    private bool behaviorComplete = false;

    [SerializeField] private float searchLength;

    public Vector3 SearchLocation;

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        SearchLocation = contRef.SearchLocation;
        MoveToPoint(SearchLocation);
        thisAgent.isStopped = false;
        behaviorComplete = false;
        contRef.GetAnimator().SetBool("isSearching", true);
        contRef.searchAnimator.runtimeAnimatorController = stateController;
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
                contRef.ChangeBehavior(GuardStates.returnToPath);
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
        contRef.GetAnimator().SetBool("isStandingSearching", true);
        MoveToPoint(selfRef.transform.position);
        thisAgent.isStopped = true;
        GuardCoroutineManager.Instance.StartBehaviorTimer(searchLength, this);
        contRef.GetAnimator().SetTrigger("LookingAround");

        progress = 0;

/*#if UNITY_EDITOR
        selfRef.GetComponent<GuardDebugger>().StartDebugProgress(searchLength, this);
#endif*/
    }

    /// <summary>
    /// Stops the behaviors loop
    /// </summary>
    public override void StopBehavior()
    {
        contRef.searchAnimator.StopPlayback();
        contRef.searchAnimator.runtimeAnimatorController = null;
        base.StopBehavior();
        GuardCoroutineManager.Instance.StopBehaviorTimer(TimerCoroutine);
        behaviorComplete = true;
        contRef.GetAnimator().SetTrigger("LookingAround");
        SearchLocation = Vector3.zero;
        contRef.GetAnimator().SetBool("isStandingSearching", false);
        contRef.GetAnimator().SetBool("isSearching", false);
    }
}

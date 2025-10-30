/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/28/25
 * Last Edited: 10/28/25
 * Summary: Search behavior that runs if the guard sees a non-hiding possessable that it possessed
 */

using System.Collections;
using GuardUtilities;
using UnityEngine;

[CreateAssetMenu(fileName = "New Search Possessable Behavior", menuName = "Guard Behaviors/New Search Possessable Behavior")]
public class PossessableSearchBehavior : GuardMovement
{
    private bool atSearchLocation = false;
    private bool searching = false;
    private bool behaviorComplete = false;
    private bool timerComplete = false;

    public bool TimerComplete { get => timerComplete; set => timerComplete = value; }

    [SerializeField] private float searchLength;

    public Vector3 SearchLocation;

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        SearchLocation = selfRef.GetComponent<GuardController>().SearchLocation;
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
                selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.patrol);
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
        GuardCoroutineManager.instance.StartPossessableSearchTimer(searchLength, this);
    }

    /// <summary>
    /// Stops the behaviors loop
    /// </summary>
    public override void StopBehavior()
    {
        base.StopBehavior();

        if(TimerCoroutine != null)
            GuardCoroutineManager.instance.StopBehaviorTimer(TimerCoroutine);

        behaviorComplete = true;
        SearchLocation = Vector3.zero;

        if (timerComplete == true)
            selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.chase);
    }
}

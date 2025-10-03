using GuardUtilities;
using UnityEngine;
using System.Collections;

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
        SearchLocation = //REPLACE WITH A CENTRALIZED REFERENCE TO THE PLAYER WHEN ABLE
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
        GuardCoroutineManager.instance.StartBehaviorTimer(searchLength, this);
        selfRef.GetComponent<GuardController>().GetAnimator().SetTrigger("LookingAround");
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
    }
}

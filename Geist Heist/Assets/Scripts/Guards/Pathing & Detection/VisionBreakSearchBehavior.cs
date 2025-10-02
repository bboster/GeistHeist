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
    /// Controls the overall logic for the behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        MoveToPoint(SearchLocation);

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
        //START ANIMATION THAT CONTROLS SEARCH MOVEMENT
    }

    /// <summary>
    /// Stops the behaviors loop
    /// </summary>
    public override void StopBehavior()
    {
        GuardCoroutineManager.instance.StopBehaviorTimer(TimerCoroutine);
        behaviorComplete = true;
    }
}

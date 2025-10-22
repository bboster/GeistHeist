/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/04/25
 * Last Edited: 10/04/25
 * Summary: Returns the guard to its patrol path.
 */

using UnityEngine;
using GuardUtilities;
using System.Collections;

[CreateAssetMenu(fileName = "New Return Behavior", menuName = "Guard Behaviors/New Return Behavior")]
public class ReturnBehavior : GuardMovement
{
    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        MoveToPoint(selfRef.GetComponent<GuardController>().Path.GetPoint(0).position);
        thisAgent.isStopped = false;
    }

    public override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (CheckPathCompletion() == true)
            {
                thisAgent.ResetPath();
                selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.patrol);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

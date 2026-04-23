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
        if (contRef.SawPlayer)
        {
            contRef.SawPlayer = false;
            AchievementManager.instance.UnlockAchievement(AchievementManager.eAchievements.EscapeGuard);
        }
        if (contRef.DefaultBehavior.StateName == GuardStates.idle)
        {
            MoveToPoint(contRef.ReturnLocation.position);
            thisAgent.isStopped = false;
        }
        else
        {
            MoveToPoint(contRef.Path.GetPoint(0).position);
            thisAgent.isStopped = false;
        }

        contRef.PlayVoiceline(5);
        contRef.GetAnimator().SetBool("isPatrolling", true);
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        contRef.GetAnimator().SetBool("isPatrolling", false);
    }

    public override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (CheckPathCompletion() == true)
            {
                thisAgent.ResetPath();
                contRef.ChangeBehavior(contRef.DefaultBehavior.StateName);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}

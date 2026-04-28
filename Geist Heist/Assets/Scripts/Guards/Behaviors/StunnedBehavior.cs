/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 11/04/25
 * Last Edited: 11/04/25
 * Summary: Runs behavior for when the guard gets stunned
 */

using GuardUtilities;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "New Stunned State", menuName = "Guard Behaviors/New Stunned State")]
public class StunnedBehavior : Behavior
{
    [SerializeField] private float stunLength;

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);
        contRef.PlayVoiceline(2);
        contRef.GetAnimator().SetBool("isStunned", true);
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        contRef.GetAnimator().SetBool("isSetunned", false);
    }

    public override IEnumerator BehaviorLoop()
    {
        NavMeshAgent thisAgent = selfRef.GetComponent<NavMeshAgent>();

        thisAgent.isStopped = true;

/*#if UNITY_EDITOR
        selfRef.GetComponent<GuardDebugger>().StartDebugProgress(stunLength, this);
#endif*/

        yield return new WaitForSeconds(stunLength); //REPLACE THIS WITH SOMETHING TO TIE IN ANIMATIONS LATER
        thisAgent.isStopped = false;

        contRef.ChangeBehavior(contRef.DefaultBehavior.StateName);
    }
}

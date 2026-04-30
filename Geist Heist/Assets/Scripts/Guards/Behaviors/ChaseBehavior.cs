/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/30/25
 * Summary: Handles behavior for the enemy when it is chasing the player.
 * To Do: Replace GetPlayerLocation() .Find() with a reference to a manager.
 */

using UnityEngine;
using GuardUtilities;
using System.Collections;

[CreateAssetMenu(fileName = "New Chase Behavior", menuName = "Guard Behaviors/New Chase Behavior")]
public class ChaseBehavior : GuardMovement
{
    private bool attacking = false;

    [SerializeField] private float attackRange;

    #region Initialize Function and OnDisable

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);

        MoveToPoint(GetPlayerLocation());
        thisAgent.isStopped = false;

        contRef.SawPlayer = true;
        contRef.PlayVoiceline(3);
        contRef.GetAnimator().SetBool("isChasing", true);
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        contRef.GetAnimator().SetBool("isChasing", false);
    }

    #endregion

    /// <summary>
    /// Controls the flow of the chase behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (Vector3.Distance(PlayerManager.Instance.CurrentObject.transform.position, selfRef.transform.position) <= attackRange)
            {
                contRef.ChangeBehavior(GuardStates.attack);
            }
            else if(thisAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid)
            {
                contRef.ChangeBehavior(contRef.DefaultBehavior.StateName); //Resets guard if the player is unreachable
            }
            else
            {
                MoveToPoint(GetPlayerLocation());
                thisAgent.isStopped = false;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Gets the location of the player in the world.
    /// </summary>
    /// <returns></returns>
    private Vector3 GetPlayerLocation()
    {
        return PlayerManager.Instance.CurrentObject.transform.position;
    }
}

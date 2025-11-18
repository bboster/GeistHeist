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

    #region Initialize Function and OnDisable

    public override void InitializeBehavior(GameObject selfRef)
    {
        base.InitializeBehavior(selfRef);

        MoveToPoint(GetPlayerLocation());
        thisAgent.isStopped = false;
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
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
            if (CheckPathCompletion() == true) //Consider changing this to be a distance check rather than a path completion check
            {
                contRef.ChangeBehavior(GuardStates.attack);
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

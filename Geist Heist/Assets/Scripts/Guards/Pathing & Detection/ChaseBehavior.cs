/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/16/25
 * Last Edited: 9/18/25
 * Summary: Handles behavior for the enemy when it is chasing the player.
 * To Do: Replace GetPlayerLocation() .Find() with a reference to a manager.
 */

using UnityEngine;
using EnemyUtilities;
using System.Collections;

public class ChaseBehavior : EnemyMovement
{
    private bool attacking = false;

    /// <summary>
    /// Overrides the default StopBehavior function to pause all movement the enemy is doing.
    /// </summary>
    public override void StopBehavior()
    {
        if (currentLoop == null)
            return;

        thisAgent.isStopped = true;

        StopCoroutine(currentLoop);
        currentLoop = null;
    }

    /// <summary>
    /// Controls the flow of the chase behavior.
    /// </summary>
    /// <returns></returns>
    protected override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (CheckPathCompletion() == true && attacking == false)
            {
                attacking = true;
                //Change to attack behavior.
                Debug.Log("ATTACK");
                GetComponent<EnemyController>().ChangeBehavior(0); //Resets behavior to default.
            }
            else
            {
                attacking = false;
                MoveToPoint(GetPlayerLocation());
                thisAgent.isStopped = false;
                Debug.Log(gameObject.name + " IS CHASING");
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
        return GameObject.Find("1st Person Player").transform.position; //REPLACE WITH CENTRALIZED REFERENCE FROM A MANAGER ONCE ABLE.
    }
}

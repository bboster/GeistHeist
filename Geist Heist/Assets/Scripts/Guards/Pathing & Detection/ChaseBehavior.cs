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
        FirstPersonInputHandler.OnPossessObject += selfRef.GetComponent<GuardController>().ChangeBehavior;
    }

    public override void StopBehavior()
    {
        base.StopBehavior();
        FirstPersonInputHandler.OnPossessObject -= selfRef.GetComponent<GuardController>().ChangeBehavior;
    }

    #endregion

    /*protected override void Awake()
{
    base.Awake();
    FirstPersonInputHandler.OnPossessObject += GetComponent<GuardController>().ChangeBehavior;
}*/

    /// <summary>
    /// Overrides the default StopBehavior function to pause all movement the enemy is doing.
    /// </summary>
    /*public override void StopBehavior()
    {
        if (currentLoop == null)
            return;

        thisAgent.isStopped = true;

        StopCoroutine(currentLoop);
        currentLoop = null;
    }*/

    /// <summary>
    /// Controls the flow of the chase behavior.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator BehaviorLoop()
    {
        for(; ; )
        {
            if (CheckPathCompletion() == true /*&& attacking == false*/)
            {
                //attacking = true;
                selfRef.GetComponent<GuardController>().ChangeBehavior(GuardStates.attack);
            }
            else
            {
                //attacking = false;
                MoveToPoint(GetPlayerLocation());
                thisAgent.isStopped = false;
                //Debug.Log(gameObject.name + " IS CHASING");
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

    ~ChaseBehavior()
    {
        if(selfRef != null)
            FirstPersonInputHandler.OnPossessObject -= selfRef.GetComponent<GuardController>().ChangeBehavior;
    }
}

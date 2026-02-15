/******************************************************************
*    Author: Sky Beal
*    Contributors: 
*    Date Created: May 25, 2024
*    Description: Twist on interactable interface!
*    Interface that all ACTIONABLE objects will derive from. 
*       Includes a function to Action, and to turn on and off the UI prompt.
*       The player parameter is included on Action() so that each actionable
        object has a reference to the player built into the function, and we
        shouldn't have to find the player.
*******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IActionable
{
    /// <summary>
    /// Function called from player's Action script when Action input is
    /// detected. Will contain that object's functionality.
    /// </summary>
    void Action();

    /// <summary>
    /// Called when action input is canceled. Is not required to implement
    /// for all interactable objects.
    /// </summary>
    void CancelAction() { }


    //use these methods below if a object needs their own prompt, or ask marissa :)

    /// <summary>
    /// Called when action with an actionable becomes avaliable. Can be
    /// used to displays the specific UI prompt for the actionable object.
    /// </summary>
    void OnPlayerLookStart() { }

    /// <summary>
    /// Called when action with an actionable becomes unavaliable. Can be
    /// used to hide the specific UI prompt for the actionable object.
    /// </summary>
    void OnPlayerLookStop() { }

    bool IsActionable() { return true; }
}
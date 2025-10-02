/******************************************************************
*    Author: Marissa Moser
*    Contributors: 
*    Date Created: May 25, 2024
*    Description: Interface that all interactable objects will derive from. 
*       Includes a function to Interact, and to turn on and off the UI prompt.
*       The player parameter is included on Interact() so that each interactable
        object has a reference to the player built into the function, and we
        shouldn�t have to find the player.
*******************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    /// <summary>
    /// Function called from player's Interact script when Interact input is
    /// detected. Will contain that object's functionality.
    /// </summary>
    /// <param name="player"></param>
    void Interact(PossessableObject possessable);

    /// <summary>
    /// Called when interaction input is canceled. Is not required to implement
    /// for all interactable objects.
    /// </summary>
    void CancelInteract() { }


    //use these methods below if a object needs their own prompt, or ask marissa :)

    /// <summary>
    /// Called when interaction with an interactable becomes avaliable. Can be
    /// used to displays the specific UI prompt for the interactable object.
    /// </summary>
    void DisplayInteractUI() { }

    /// <summary>
    /// Called when interaction with an interactable becomes unavaliable. Can be
    /// used to hide the specific UI prompt for the interactable object.
    /// </summary>
    void HideInteractUI() { }
}
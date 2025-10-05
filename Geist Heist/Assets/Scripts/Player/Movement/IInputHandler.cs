/*
 * Contributors: Toby
 * Creation Date: 9/15/25
 * Last Modified: 10/2/25
 * 
 * Brief Description: interface for anything that can handle player input.
 *  To be placed on any possessible object, and the player ghost.
 * 
 */

using UnityEngine;
using System;

public abstract class IInputHandler : MonoBehaviour
{
    // Possession sometimes refers to entering ghost mode, keep that in mind, I guess
    public abstract void OnPossessionStarted();
    public abstract void OnPossessionEnded();

    // Note: transformed / camera relative move input vectors are on InputEvents.cs
    //  FirstPersonInputDirection, ThirdPersonInputDirection

    public abstract void OnMoveStarted();
    /// <summary>
    /// called every fixed update while move is held
    /// </summary>
    public abstract void WhileMoveHeld();
    public abstract void WhileMoveNotHeld();
    public abstract void OnMoveCanceled();

    /*
    public abstract void OnJumpStarted();
    /// <summary>
    /// called every fixed update while jump is held
    /// </summary>
    public abstract void WhileJumpHeld();
    public abstract void OnJumpCanceled();
    */

    public abstract void OnActionStarted();
    /// <summary>
    /// called every fixed update while action is held
    /// </summary>
    public abstract void WhileActionHeld();
    public abstract void WhileActionNotHeld();
    public abstract void OnActionCanceled();

    public abstract void OnInteractStarted();
    /// <summary>
    /// called every fixed update while escape oject is held
    /// </summary>
    public abstract void WhileInteractHeld();
    public abstract void OnInteractCanceled();

   

}

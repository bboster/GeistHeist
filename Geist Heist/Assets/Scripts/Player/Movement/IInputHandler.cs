/*
 * Contributors: Toby
 * Creation Date: 9/15/25
 * Last Modified: 9/15/25
 * 
 * Brief Description: interface for anything that can handle player input.
 *  To be placed on any possessible object, and the player ghost.
 * 
 */

using UnityEngine;
using System;

public interface IInputHandler
{
    public void OnMoveStarted(Vector2 input);
    /// <summary>
    /// called every fixed update while move is held
    /// </summary>
    public void WhileMoveHeld(Vector2 input);
    public void OnMoveCanceled(Vector2 input);

    public void OnJumpStarted();
    /// <summary>
    /// called every fixed update while jump is held
    /// </summary>
    public void WhileJumpHeld();
    public void OnJumpCanceled();

    public void OnActionStarted();
    /// <summary>
    /// called every fixed update while action is held
    /// </summary>
    public void WhileActionHeld();
    public void OnActionCanceled();

    public void OnEscapeObjectStarted();
    /// <summary>
    /// called every fixed update while escape oject is held
    /// </summary>
    public void WhileEscapeObjectHeld();
    public void OnEscapeObjectCanceled();




}

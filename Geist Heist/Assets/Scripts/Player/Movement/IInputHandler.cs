/*
 * Contributors: Toby, Jacob
 * Creation Date: 9/15/25
 * Last Modified: 3/3/26
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
    public abstract void OnPossessionStart();
    public abstract void OnPossessionEnded();

    // Called every frame while possessed
    public abstract void WhilePossessingUpdate();



    // Note: transformed / camera relative move input vectors are on InputEvents.cs
    public abstract void OnMoveStarted();
    /// <summary>
    /// called every fixed update while move is held
    /// </summary>
    public abstract void WhileMoveHeld(float secondsHeld);
    public abstract void WhileMoveNotHeld();
    public abstract void OnMoveCanceled(float secondsHeld);



    public abstract void OnActionStarted();
    /// <summary>
    /// called every fixed update while action is held
    /// </summary>
    public abstract void WhileActionHeld(float secondsHeld);
    public abstract void WhileActionNotHeld(float secondsNotHeld);
    public abstract void OnActionCanceled(float secondsHeld);


    public abstract void OnInteractStarted();
    /// <summary>
    /// called every fixed update while escape oject is held
    /// </summary>
    public abstract void WhileInteractHeld(float secondsHeld);
    public abstract void OnInteractCanceled(float secondsHeld);

    /* public abstract void OnSpaceStarted();
    public abstract void WhileSpaceHeld(float secondsHeld);
    public abstract void OnSpaceCanceled(float secondsHeld); */

    /// <summary>
    /// Called while a possessable is in a guard's vision range.
    /// </summary>
    /// <returns> Returns true if the object is detectable, false otherwise. </returns>
    public abstract bool IsDetectable();

}

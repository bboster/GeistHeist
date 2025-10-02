/*
 * Delete this later
 */

using UnityEngine;

public class ExampleMovement : IInputHandler
{
    public override void OnActionCanceled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnActionStarted()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInteractCanceled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnInteractStarted()
    {
        throw new System.NotImplementedException();
    }

    public override void OnMoveCanceled()
    {
        Debug.Log("Move canceled");
    }

    public override void OnMoveStarted()
    {
        Debug.Log("Move started");
    }

    public override void WhileActionHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileInteractHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileMoveHeld()
    {
        Debug.Log("move held");
    }

    public override void WhileMoveNotHeld()
    {
        Debug.Log("move held");
    }
}

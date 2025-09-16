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

    public override void OnPossessCanceled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnPossessStarted()
    {
        throw new System.NotImplementedException();
    }

    public override void OnJumpCanceled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnJumpStarted()
    {
        Debug.Log("Jump started");
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

    public override void WhilePossessHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileJumpHeld()
    {
        throw new System.NotImplementedException();
    }

    public override void WhileMoveHeld()
    {
        Debug.Log("move held");
    }
}

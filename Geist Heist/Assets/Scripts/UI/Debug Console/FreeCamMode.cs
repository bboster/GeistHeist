using GuardUtilities;
using NaughtyAttributes;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor.Rendering;

public class FreeCamMode : IInputHandler
{
    [Header("Design Variables")]
    [SerializeField] private float speed = 3;
    [SerializeField] private float maxVelocity = 10;
    [Tooltip("Higher number: reaches desired speed faster")]
    [SerializeField] private float speedPickup = 3;
    [Tooltip("Multiply speed by this number when player is not holding any move keys")]
    [SerializeField] private float slowDownFactor = 0.1f;

    Rigidbody rb;
    GameObject cameraGO;


    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {

        cameraGO = FindFirstObjectByType<Camera>().gameObject;

        //layerToInclude = LayerMask.GetMask("Interactable");
        //CooldownManager.Instance.OnCooldownFinished += OnCooldownFinished;
    }

    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
    }

    public override void OnPossessionEnded()
    {
    }

    #region Action
    public override void OnActionStarted()
    {
    }

    public override void WhileActionHeld(float secondsHeld)
    {
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
    }

    public override void OnActionCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Interact

   
    public override void OnInteractStarted()
    {
        
    }

    public override void WhileInteractHeld(float secondsHeld)
    {
    }

    public override void OnInteractCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld(float secondsHeld)
    {
        var direction = InputEvents.Instance.FirstPersonInputDirection;

        var a = rb.linearVelocity;
        var b = (direction * speed);

        var horizontalVelocity = Vector3.Lerp(rb.linearVelocity, (direction * speed), speedPickup * Time.fixedDeltaTime);
        Vector3.ClampMagnitude(horizontalVelocity, maxVelocity);

    }

    public override void WhileMoveNotHeld()
    {
        // Maintains y velocity
        rb.linearVelocity = Vector3.MoveTowards(rb.linearVelocity, new Vector3(0, rb.linearVelocity.y, 0), slowDownFactor * Time.fixedDeltaTime);
    }


    public override void OnMoveCanceled(float secondsHeld) { }
    #endregion

    #region Other


    #endregion

}

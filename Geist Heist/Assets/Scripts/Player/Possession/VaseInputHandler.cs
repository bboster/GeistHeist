using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
 /*
 * Contributors: Sky
 * Creation Date: 9/17/25
 * Last Modified: 9/30/25
 * 
 * Brief Description: Input Handler for the Vase, handles movement and actions for the vase
 */

public class VaseInputHandler : IInputHandler
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [SerializeField] ParticleSystem possessableParticles;

    private void Start()
    { 
    }

    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        possessableParticles.Play();
    }

    public override void OnPossessionEnded()
    {
        possessableParticles.Stop();
    }

    public override bool IsDetectable() { return false; }

    #region action
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

    #region Possess
    public override void OnInteractStarted()
    {
        PlayerManager.Instance.PossessGhost(GetComponent<PossessableObject>());
    }

    public override void WhileInteractHeld(float secondsHeld)
    { }

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
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled(float secondsHeld) { }


    #endregion
}


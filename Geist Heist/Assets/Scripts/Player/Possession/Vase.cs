using UnityEngine;
using UnityEngine.UI;
 /*
 * Contributors: Sky
 * Creation Date: 9/17/25
 * Last Modified: 9/30/25
 * 
 * Brief Description: Input Handler for the Vase, handles movement and actions for the vase
 */

public class VaseObject : IInputHandler
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;

    private void Start()
    {
        thirdPersoncinemachineCamera.SetActive(false);
    }

    public override void OnPossessionStarted()
    {

    }

    public override void OnPossessionEnded()
    {

    }

    #region action
    public override void OnActionStarted()
    {
        //throw new System.NotImplementedException();
    }

    public override void WhileActionHeld()
    {
        //throw new System.NotImplementedException();
    }

    public override void WhileActionNotHeld()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnActionCanceled()
    {
        //throw new System.NotImplementedException();
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf)
        {
            PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
        }
    }

    public override void WhileInteractHeld()
    { }

    public override void OnInteractCanceled()
    {
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld()
    {
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled() { }
    #endregion
}


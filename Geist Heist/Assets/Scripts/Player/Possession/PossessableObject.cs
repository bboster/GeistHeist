/*
 * Contributors: Toby, Sky
 * Creation Date: 9/16/25
 * Last Modified: 9/18/25
 * 
 * Brief Description: On every possessable object, and the player for simplicity. 
 * Contains reference to input scripts and other stuff.
 *  
 *  TODO:
 */
using UnityEngine;
using Unity.Cinemachine;
using NaughtyAttributes;

public class PossessableObject : MonoBehaviour, IInteractable
{
    public IInputHandler InputHandler;
    [Required] public CinemachineCamera CinemachineCamera;

    private void Awake()
    {
        InputHandler = InputHandler ?? GetComponent<IInputHandler>();
    }

    void IInteractable.Interact()
    {
        PlayerManager.Instance.PossessObject(this);
    }

    // Called in PlayerManager
    public void OnPossessionStarted()
    {
        InputHandler.OnPossessionEnded();
    }

    // Called in PlayerManager
    public void OnPossessionEnded()
    {
        InputHandler.OnPossessionEnded();
    }
}

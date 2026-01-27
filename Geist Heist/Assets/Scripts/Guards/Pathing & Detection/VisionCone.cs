/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 9/18/25
 * Last Edited: 10/02/25
 * Summary: Detects when the player enters or exits and enemy's vision cone and changes behavior accordingly.
 */

using UnityEngine;
using GuardUtilities;

public class VisionCone : MonoBehaviour
{
    private bool hasSeenPlayer = false;

    [SerializeField] private GuardController parentController;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == false)
        {
            hasSeenPlayer = true;
            parentController.ChangeBehavior(GuardStates.surprised);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.Equals(PlayerManager.Instance.PlayerGhostObject) && hasSeenPlayer == true)
        {
            hasSeenPlayer = false;
            parentController.ChangeBehavior(GuardStates.search);
        }
    }
}

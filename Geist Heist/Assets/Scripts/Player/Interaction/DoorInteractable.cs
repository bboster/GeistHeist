/*
 * Contributors: Sky, Josh
 * Creation Date: 9/30/25
 * Last Modified: 10/1/25
 * 
 * Brief Description: Controls interactions for the door, changes scene on button press
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using UnityEngine.Events;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;

    public void Interact()
    {
        GameManager.Instance.NextLevel(sceneName);
    }
}

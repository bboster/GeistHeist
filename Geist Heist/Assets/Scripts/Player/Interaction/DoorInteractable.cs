/*
 * Contributors: Sky
 * Creation Date: 9/30/25
 * Last Modified: 9/30/25
 * 
 * Brief Description: Controls interactions for the door, changes scene on button press
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;
    [SerializeField] private bool EndOfScene = false;
    public void Interact(PossessableObject possessable)
    {
        if (EndOfScene == true)
        {
            GameManager.Instance.NextLevel(sceneName);
            EndOfScene = false;
        }
        else
            SceneManager.LoadScene(sceneName);
        
    }
}

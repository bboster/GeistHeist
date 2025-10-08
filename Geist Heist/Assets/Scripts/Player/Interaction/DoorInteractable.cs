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

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;
    //[SerializeField] private bool EndOfScene = false;
    //[SerializeField] int sceneIndex = 0;
    public void Interact()
    {
        /*if (EndOfScene == true  && GameManager.currentLevel < 1)
        {
            GameManager.Instance.NextLevel(sceneName);
            EndOfScene = false;
        }
        else*/
        //SceneManager.LoadScene(sceneName);
        GameManager.Instance.NextLevel(sceneName);
        
    }
}

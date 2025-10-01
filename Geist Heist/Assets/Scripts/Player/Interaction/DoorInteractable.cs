using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class DoorInteractable : MonoBehaviour, IInteractable
{
    [SerializeField][Scene] private string sceneName;
    
    public void Interact(PossessableObject possessable)
    {
        SceneManager.LoadScene(sceneName);
    }
}

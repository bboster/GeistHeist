/*
 * Contributors:  Josh
 * Creation Date: 10/1/25
 * Last Modified: 10/1/25
 * 
 * Brief Description: Keeps track of game state, such as level.
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject blockingWall;
    public static int currentLevel = 0;
    private void Awake()
    {
        Instance = this;
    }
    public void NextLevel(string sceneName)
    {
        currentLevel++;
        SceneManager.LoadScene(sceneName);
        Debug.Log("Advancing to level index: " + currentLevel);
    }

    public void LoadCurrentLevel()
    {
        Debug.Log("Loading level: " + currentLevel);
        if (currentLevel < 1)
        { 
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                    RemoveBlockingWall();
            }
            else
                Debug.LogWarning("No more levels to load or invalid level index.");
        }
    }
    private void RemoveBlockingWall()
    {
        if (blockingWall != null)
        {
            blockingWall.SetActive(false);
            Debug.Log("Lobby blocking wall removed.");
        }
        else
        {
            Debug.LogWarning($"Lobby blocking wall '{blockingWall}' not found.");
        }
    }

}
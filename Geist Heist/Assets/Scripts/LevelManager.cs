/*
 * Author: Jacob Bateman
 * Contributors: Toby
 * Creation: 10/21/25
 * Last Edited: 10/27/25
 * Summary: Stores data for a level that needs to carry over between scene reloads
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class LevelManager : DontDestroyOnLoadSingleton<LevelManager>
{
    private int previousLevel = -1;

    [HideInInspector] public Vector3 SpawnLocation;

    protected override void Awake()
    {
        base.Awake();
    }

#if UNITY_EDITOR

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("CheckpointTestScene");
        }
    }

#endif

    /// <summary>
    /// Initializes the LevelManager every time a scene is loaded
    /// </summary>
    /// <param name="location"></param>
    public Task InitializeLevelManager(Vector3 location)
    {
        if(previousLevel == -1 || previousLevel != SceneManager.GetActiveScene().buildIndex)
        {
            SpawnLocation = location;
            previousLevel = SceneManager.GetActiveScene().buildIndex;
        }

        PlayerManager.Instance.PlayerGhostObject.gameObject.transform.position = SpawnLocation;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates SpawnLocation
    /// </summary>
    /// <param name="location"></param>
    public void UpdateCheckpoint(Vector3 location)
    {
        SpawnLocation = location;
    }
}
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
    private Checkpoint currentCheckpoint;

    /*
     * delete this?
     * -toby
     */
    protected override void Awake()
    {
        base.Awake();
    }

#if UNITY_EDITOR

    /*
     * delete this?
     * -toby
     */
    private void Update()
    {
        /*if(Input.GetKeyDown(KeyCode.L))
        {
            SceneManager.LoadScene("CheckpointTestScene");
        }*/
    }

#endif

    /// <summary>
    /// Initializes the LevelManager every time a scene is loaded
    /// </summary>
    /// <param name="location"></param>
    public Task InitializeLevelManager(Vector3 location)
    {
        /*
         * rename this function to Initialize?
         * -toby
         */
        if (previousLevel == -1 || previousLevel != SceneManager.GetActiveScene().buildIndex)
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
    public void UpdateCheckpoint(Vector3 location, Checkpoint checkpoint)
    {
        SpawnLocation = location;
        currentCheckpoint = checkpoint;
    }

    public bool IsCheckpointCurrent(Checkpoint checkpoint)
    {
        return (checkpoint == currentCheckpoint);
    }

    /*
     * There are some scene transition scripts in gamemanager, can we move them to this script?
     * -Toby
     */ 
}
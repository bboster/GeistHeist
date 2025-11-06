/*
 * Contributors:  Josh, Toby, Jacob
 * Creation Date: 10/1/25
 * Last Modified: 11/3/25
 * 
 * Brief Description: Instantiates managers scripts that are required for scene to function.
 * Keeps track of game state, such as level.
 */
using UnityEngine;
using UnityEngine.SceneManagement;
using NaughtyAttributes;
using Unity.Cinemachine;
using UnityEngine.UI;
using System;
using System.Threading.Tasks;
using UnityEngine.InputSystem.UI;

public class GameManager : Singleton<GameManager>
{
    // All of this behavior is implemented in TetherPossessable.cs, DoorInteractable.cs, and HubLevelGate.cs

    [Header("Managers")]
    [SerializeField, Required] GameObject InputManagerPrefab;
    [SerializeField, Required] GameObject PlayerManagerPrefab;
    //[SerializeField, Required] GameObject CoolDownManagerPrefab; TODO: waiting until sky finishes refactoring it
    [SerializeField, Required] GameObject SaveDataManagerPrefab;
    [SerializeField, Required] GameObject GuardCoroutineManagerPrefab;
    [SerializeField, Required] GameObject BehaviourDatabasePrefab;
    [SerializeField, Required] GameObject ShaderManagerPrefab;
    [SerializeField, Required] GameObject GuardManagerPrefab;
    [SerializeField, Required] GameObject BillboardUIManagerPrefab;
    [SerializeField, Required] GameObject LevelManagerPrefab;
    [SerializeField, Required] GameObject DailougeManagerPrefab;

    [Header("Canvases")]
    [SerializeField, Required] GameObject PauseMenuPrefab;

    [Header("Other Constants")]
    [SerializeField, Required] GameObject EventSystemPrefab; // for detecting UI input events (unity thing, not us).
    [SerializeField, Required] GameObject CameraPrefab;

    [Header("Player Variables")]
    [SerializeField, Required] GameObject PlayerPrefab;
    [Required] public Transform PlayerStart;

    [Header("Debug")]
    [ReadOnly] public bool IsPaused = false;

    public GameObject Player;

    public static Action OnInitialize;

    protected override void Awake()
    {
        base.Awake();

        if (this == null)
            return;

        // this can be destroyed bc it is a singleton
        if (this == null || gameObject == null) 
            return;

        // All of these should be singletons, which destroy themselves if they already exist, 
        // so its okay if we dont check if this doesnt exist first
        InstantiateManagers();

        SpawnPlayer();

        // I saw a designer not understand why the camera wasnt working (they didnt have a cinemachine brain / the right settings on it).
        // So this should kinda streamline things.
        var currentCamera = Camera.main;
        if (currentCamera.GetComponent<CinemachineBrain>() == null)
        {
            CameraPrefab.GetComponent<CinemachineBrain>().CopyComponent(currentCamera.gameObject);
            CameraPrefab.GetComponent<Transform>().CopyComponent(currentCamera.gameObject);
            CameraPrefab.GetComponent<Camera>().CopyComponent(currentCamera.gameObject);
        }

        OnInitialize?.Invoke();
    }

    /*[SerializeField] private GameObject blockingWall;
    public static int currentLevel = 0;*/

    public void NextLevel(string sceneName)
    {
        //currentLevel++;
        SceneManager.LoadScene(sceneName);
        Debug.Log("Advancing to level: " + sceneName);
    }

    public void NextLevel(int sceneNum)
    {
        //currentLevel++;
        SceneManager.LoadScene(sceneNum);
        Debug.Log("Advancing to level: " + sceneNum);
    }

    /// <summary>
    /// Spawns the player into the level
    /// </summary>
    /// <returns></returns>
    public Task SpawnPlayer()
    {
        /*Player = Instantiate(PlayerPrefab, LevelManager.Instance.SpawnLocation, Quaternion.identity);*/
        //PlayerManager.Instance.InitializePlayerManager();

        return Task.CompletedTask;
    }

    /// <summary>
    /// Instantiates all managers the game depends on
    /// </summary>
    /// <returns></returns>
    public Task InstantiateManagers()
    {
        Instantiate(InputManagerPrefab);
        Instantiate(PlayerManagerPrefab);
        Instantiate(SaveDataManagerPrefab);
        Instantiate(GuardCoroutineManagerPrefab);
        Instantiate(BehaviourDatabasePrefab);
        Instantiate(ShaderManagerPrefab);
        Instantiate(LevelManagerPrefab);
        Instantiate(DailougeManagerPrefab);

        Instantiate(BillboardUIManagerPrefab).GetComponent<BillboardUIManager>().Initialize();
        Instantiate(GuardManagerPrefab).GetComponent<GuardManager>().Initialize();

        Instantiate(PauseMenuPrefab);//.GetComponentInChildren<PauseMenu>().Initialize();

        if (GameObject.FindAnyObjectByType(typeof(InputSystemUIInputModule)) == null)
            Instantiate(EventSystemPrefab);

        return Task.CompletedTask;
    }

    /// <summary>
    /// Resets the level on player death
    /// </summary>
    /// <returns></returns>
    public void DeathReset()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /*public void LoadCurrentLevel()
    {
        Debug.Log("Loading level: " + currentLevel);
        if (currentLevel >= 1)
        { 
            if (SceneManager.GetActiveScene().name == "Lobby")
            {
                    RemoveBlockingWall();
            }
            else
                Debug.LogWarning("No more levels to load or invalid level index.");
        }
    }*/

    // This functionality already exists in HubLevelGate.cs
    /*private void RemoveBlockingWall()
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
    }*/

}
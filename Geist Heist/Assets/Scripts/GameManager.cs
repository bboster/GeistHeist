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
using UnityEngine.Events;

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
    [SerializeField, Required] GameObject GeneralHUDPrefab;

    [Header("Other Constants")]
    [SerializeField, Required] GameObject EventSystemPrefab; // for detecting UI input events (unity thing, not us).
    [SerializeField, Required] GameObject CameraPrefab;
    [SerializeField] GameObject DebugConsolePrefab;

    [Header("Player Variables")]
    [SerializeField, Required] GameObject PlayerPrefab;
    [Required] public Transform PlayerStart;

    public bool IsPaused { get; private set; } = false;
    public UnityEvent OnPauseChanged = new();

    [HideInInspector] public bool InGodMode;

    public GameObject Player;

    public static Action OnInitialize;

    protected override void Awake()
    {
        base.Awake();

        if (this == null)
            return;

        InGodMode = false;

        // this can be destroyed bc it is a singleton
        if (this == null || gameObject == null) 
            return;

        SettingsProfile.ReadSavedSettings();

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

    #region Level Progression
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
    /// Resets the level on player death
    /// </summary>
    public void DeathReset()
    {
        if (!InGodMode)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    #endregion

    #region Initialization

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
        Instantiate(GeneralHUDPrefab);
        Instantiate(DebugConsolePrefab);

        if (GameObject.FindAnyObjectByType(typeof(InputSystemUIInputModule)) == null)
            Instantiate(EventSystemPrefab);

        return Task.CompletedTask;
    }

    #endregion

    #region Game Manipulation

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0;
        OnPauseChanged.Invoke();
    }

    public void UnpauseGame()
    {
        IsPaused = false;
        Time.timeScale = 1;
        OnPauseChanged.Invoke();
    }

    public void TogglePause()
    {
        if (IsPaused) UnpauseGame();
        else PauseGame();
    }
    #endregion
}
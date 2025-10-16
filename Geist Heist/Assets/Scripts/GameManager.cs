/*
 * Contributors:  Josh, Toby
 * Creation Date: 10/1/25
 * Last Modified: 10/9/25
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

public class GameManager : Singleton<GameManager>
{
    // All of this behavior is implemented in TetherPossessable.cs, DoorInteractable.cs, and HubLevelGate.cs

    [Header("Managers")]
    [SerializeField, Required] GameObject InputManagerPrefab;
    //[SerializeField, Required] GameObject CoolDownManagerPrefab; TODO: waiting until sky finishes refactoring it
    [SerializeField, Required] GameObject SaveDataManagerPrefab;
    [SerializeField, Required] GameObject GuardCoroutineManagerPrefab;
    [SerializeField, Required] GameObject BehaviourDatabasePrefab;
    [SerializeField, Required] GameObject ShaderManagerPrefab;
    [SerializeField, Required] GameObject CooldownManagerPrefab;
    [SerializeField, Required] GameObject GuardManagerPrefab;
    [SerializeField, Required] GameObject BillboardUIManagerPrefab;

    [Header("Other Constants")]
    [SerializeField, Required] GameObject CameraPrefab;
    [SerializeField, Required] GameObject InteractionCanvasPrefab;
    [SerializeField, Required] GameObject TimerCanvasPrefab;

    [HideInInspector] public GameObject InteractionCanvas;
    [HideInInspector] public Slider TimerSlider;

    public static Action OnInitialize;

    protected override void Awake()
    {
        base.Awake();

        // this can be destroyed bc it is a singleton
        if (this == null || gameObject == null) 
            return;

        // All of these should be singletons, which destroy themselves if they already exist, 
        // so its okay if we dont check if this doesnt exist first

        Instantiate(InputManagerPrefab);
        Instantiate(SaveDataManagerPrefab);
        Instantiate(GuardCoroutineManagerPrefab);
        Instantiate(BehaviourDatabasePrefab);
        Instantiate(ShaderManagerPrefab);
        Instantiate(CooldownManagerPrefab);
        Instantiate(BillboardUIManagerPrefab);

        Instantiate(GuardManagerPrefab).GetComponent<GuardManager>().Initialize();
        var timerCanvas = Instantiate(TimerCanvasPrefab);
        TimerSlider = timerCanvas.GetComponentInChildren<Slider>();
        TimerSlider.gameObject.SetActive(false);

        InteractionCanvas = Instantiate(InteractionCanvasPrefab);

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
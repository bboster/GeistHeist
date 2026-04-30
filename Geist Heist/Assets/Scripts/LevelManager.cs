/*
 * Author: Jacob Bateman
 * Contributors: Toby
 * Creation: 10/21/25
 * Last Edited: 4/20/2026
 * Summary: Stores data for a level that needs to carry over between scene reloads
 */

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.Events;
using Unity.Cinemachine;
using NaughtyAttributes;
using System.Linq;

public class LevelManager : DontDestroyOnLoadSingleton<LevelManager>
{
    private int previousLevel = -1;

    [HideInInspector] public Vector3 SpawnLocation;
    [HideInInspector] public Vector3 SpawnRotation;
    private readonly HashSet<KeyType> savedKeys = new();
    private readonly HashSet<string> savedDoorIds = new();
    private readonly HashSet<string> activatedCheckpointIds = new();
    [SerializeField] private GameObject fadeToBlack;

    [Header("Scene Name Pairing")]
    [ReorderableList] public List<LevelNamePair> LevelNames;

    /// <summary>
    /// Initializes the LevelManager every time a scene is loaded
    /// </summary>
    /// <param name="location"></param>
    public Task Initialize(Vector3 location)
    {
        if (previousLevel == -1 || previousLevel != SceneManager.GetActiveScene().buildIndex)
        {
            SpawnLocation = location;
            SpawnRotation = PlayerManager.Instance.PlayerGhostObject.transform.rotation.eulerAngles;
            previousLevel = SceneManager.GetActiveScene().buildIndex;
            savedKeys.Clear();
            savedDoorIds.Clear();
            activatedCheckpointIds.Clear();
        }

        PossessableObject player = PlayerManager.Instance.PlayerGhostObject;
        player.gameObject.transform.position = SpawnLocation;
        player.gameObject.transform.rotation = Quaternion.Euler(SpawnRotation);
        player.CinemachineCamera.GetComponent<CinemachineOrbitalFollow>().HorizontalAxis.Value = SpawnRotation.y;
        RestoreKeys();
        RestoreDoors();
        RestoreSettings();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Updates SpawnLocation
    /// </summary>
    /// <param name="location"></param>
    public bool UpdateCheckpoint(Vector3 location, Vector3 rotation, Checkpoint checkpoint)
    {
        if (checkpoint == null)
            return false;

        string checkpointId = checkpoint.GetCheckpointStateId();
        if (string.IsNullOrWhiteSpace(checkpointId))
            return false;

        // Each checkpoint can only create one saved snapshot per scene run.
        if (!activatedCheckpointIds.Add(checkpointId))
            return false;

        SpawnLocation = location;
        SpawnRotation = rotation;
        SaveCurrentKeys();
        SaveCurrentDoors();
        return true;
    }

    public void SaveCurrentKeys()
    {
        savedKeys.Clear();

        if (KeyManager.Instance == null)
            return;

        foreach (var key in KeyManager.Instance.GetKeys())
        {
            if (key != KeyType.None)
                savedKeys.Add(key);
        }
    }

    private void RestoreKeys()
    {
        if (KeyManager.Instance == null)
            return;

        var keysToRestore = new List<KeyType>(savedKeys);
        KeyManager.Instance.Clear();

        foreach (var key in keysToRestore)
        {
            KeyManager.Instance.AddKey(key);
        }
    }

    public bool IsDoorOpened(string doorStateId)
    {
        if (string.IsNullOrEmpty(doorStateId))
            return false;

        return savedDoorIds.Contains(doorStateId);
    }

    public void SaveCurrentDoors()
    {
        savedDoorIds.Clear();

        var doorsInScene = FindObjectsByType<LockedDoorInteractable>(FindObjectsSortMode.None);
        foreach (var door in doorsInScene)
        {
            if (door.TryGetOpenDoorStateId(out var doorStateId))
                savedDoorIds.Add(doorStateId);
        }
    }

    private void RestoreDoors()
    {
        if (savedDoorIds.Count == 0)
            return;

        var doorsInScene = FindObjectsByType<LockedDoorInteractable>(FindObjectsSortMode.None);
        foreach (var door in doorsInScene)
        {
            door.RestoreCheckpointStateIfNeeded();
        }
    }

    private void RestoreSettings()
    {
        SettingsProfile.ReadSavedSettings();
    }

    public void InstantiateFadeToBlack(UnityAction action)
    {
        FadeToBlack ftb = Instantiate(fadeToBlack).GetComponent<FadeToBlack>();
        ftb.Initialize(action);
    }

    #region Scene Transition Scripts

    public void ChangeScene(string sceneName)
    {
        //currentLevel++;
        SceneLoadManager.Instance.LoadScene(sceneName);
        Debug.Log("Advancing to level: " + sceneName);
    }

    public void ChangeScene(int sceneNum)
    {
        //currentLevel++;
        SceneLoadManager.Instance.LoadScene(sceneNum);
        Debug.Log("Advancing to level: " + sceneNum);
    }

    #endregion

    public string GetLevelDisplayName(string sceneName)
    {
        var filtered = LevelNames.Where(l => l.SceneName == sceneName);
        if(filtered.Any() == false)
        {
            Debug.LogError($"{sceneName} does not have a display name in Level Manager. Please go to the Level Manager Prefab and set one.");
            return sceneName;
        }
        return filtered.First().DisplayName;
    }

    public string GetSceneName(string debugName)
    {
        return LevelNames.Where(n => n.InternalDebugName == debugName).First().SceneName;
    }
}

[System.Serializable]
public class LevelNamePair
{
    [AllowNesting, Scene]
    public string SceneName;
    public string DisplayName;
    public string InternalDebugName;
}
/*
 * Contributors: Toby
 * Creation: 10/1/25
 * Last Edited: 10/2/25
 * Summary: Manages player save data. Allows save and loading functionality.
 * Singleton, dont destroy on load
 */

using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveDataManager : DontDestroyOnLoadSingleton<SaveDataManager>
{
    [SerializeField] private TextAsset saveFile;
    private SaveDataFile currentSaveDta;

    [Header("Export Settings")]
    [SerializeField] private string _defaultPath = "Assets/Save Files/";
    [SerializeField] private string _defaultfFileName = "Save File";
    [SerializeField] private string _fileType = "json";

    [Header("Debug")]
    [Tooltip("If true, does not save any data")]
    public bool DontSaveData = false;
    // Debug buttons will be under this area

    private string runtimeSavePath;

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {
        // TODO: move this from start later, probably
        runtimeSavePath = Path.Combine(Application.persistentDataPath, $"{_defaultfFileName}.{_fileType}");
        LoadData();
    }

    public void MarkSceneAsCompleted(string sceneName, bool autoSave=true)
    {
        EnsureSaveData();

        if (IsLevelCompleted(sceneName))
            Debug.Log("This level has already been completed");
        else
            currentSaveDta.ScenesCompleted.Add(sceneName);

        if (autoSave)
            SaveData();
    }

    public void MarkCollectableAsCollected(Collectable collectable, bool autoSave = true)
    {
        EnsureSaveData();

        if (IsCollectableCollected(collectable))
            Debug.Log("Collectable has already been collected");
        else
            currentSaveDta.CollectablesCollected.Add((int)collectable);

        if (autoSave)
            SaveData();
    }

    public bool IsLevelCompleted(string sceneName)
    {
        EnsureSaveData();
        return currentSaveDta.ScenesCompleted.Contains(sceneName);
    }

    public bool IsCollectableCollected(Collectable collectable)
    {
        EnsureSaveData();
        return currentSaveDta.CollectablesCollected.Contains((int)collectable);
    }

    [Button]
    public void LoadData()
    {
#if UNITY_EDITOR

        if(saveFile == null)
        {
            Debug.LogWarning("No save file is present to load");
            currentSaveDta = new();
            return;
        }

        currentSaveDta = JsonUtility.FromJson<SaveDataFile>(saveFile.text);

        Debug.Log($"Loaded save file:\n{currentSaveDta.CollectablesCollected.Count} collectables\n{currentSaveDta.ScenesCompleted.Count} levels completed");
#else

        if (File.Exists(runtimeSavePath))
        {
            string json = File.ReadAllText(runtimeSavePath);
            currentSaveDta = JsonUtility.FromJson<SaveDataFile>(json);
            Debug.Log($"Loaded save data from: {runtimeSavePath}");
        }
        else
        {
            Debug.LogWarning($"No save file found at {runtimeSavePath}. Creating new data.");
            currentSaveDta = new SaveDataFile();
            SaveData();
        }

#endif
    }

    [Button]
    /// <summary>
    /// Overrides current text asset
    /// </summary>
    public void SaveData()
    {
        if (DontSaveData)
            return;

        if(saveFile == null)
        {
            SaveDataAsNewFile();
            return;
        }

        string elemString = JsonUtility.ToJson(currentSaveDta);

#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(saveFile);

        //var textFile = File.CreateText(path);

        //textFile.WriteLine(elemString);

        Debug.Log($"Overwriting save file at {path}");

        File.WriteAllText(path, elemString);
        AssetDatabase.Refresh();

#else
    
        File.WriteAllText(runtimeSavePath, elemString);
        Debug.Log($"Saved runtime file at {runtimeSavePath}");

#endif
    }

    [Button]
    /// <summary>
    /// Creates and saves new text asset
    /// </summary>
    public void SaveDataAsNewFile()
    {
        if (DontSaveData)
            return;

        string path = GetNewPath();
        var textFile = File.CreateText(path);

        string elemString = JsonUtility.ToJson(currentSaveDta);
        textFile.WriteLine(elemString);

        if(saveFile == null)
            saveFile = new TextAsset(path);

        textFile.Close();
        Debug.Log($"Created new save file at {path}");

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
    }

    [Button]
    /// <summary>
    /// Not sure if this function has any practical use, actually.
    /// Better to have it as a debug tool anyways, just in case
    /// </summary>
    public void ClearSaveFile()
    {
        currentSaveDta = new();
        SaveData();
    }

    [Button]
    /// <summary>
    /// Not sure if this function has any practical use, actually.
    /// Better to have it as a debug tool anyways, just in case
    /// </summary>
    public void DeleteSaveFile()
    {
#if UNITY_EDITOR
        currentSaveDta = new();

        if (saveFile == null)
        {
            Debug.LogWarning("No save file is present!");
            return;
        }

        Debug.Log($"Deleting save file at {AssetDatabase.GetAssetPath(saveFile)}");

        string fileContents = saveFile.text;
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(saveFile));

        Debug.Log($"Backup of file contents (copy paste to new file if deleting was a mistake):\n{fileContents}");

        saveFile= null;
        AssetDatabase.Refresh();
#endif
    }

    private string GetNewPath()
    {
#if UNITY_EDITOR
        int number = 1;
        while (true)
        {
            string path = "";

            if (number == 1)
                path = _defaultPath + _defaultfFileName + "." + _fileType;
            else
                path = _defaultPath + _defaultfFileName + " " + number + "." + _fileType;

            if (!File.Exists(path))
                return path;

            number++;
        }
#else
        return runtimeSavePath;
#endif
    }

    private void EnsureSaveData()
    {
        if (currentSaveDta == null)
        {
            LoadData();
            if (currentSaveDta == null)
                currentSaveDta = new SaveDataFile();
        }

        if (currentSaveDta.ScenesCompleted == null)
            currentSaveDta.ScenesCompleted = new List<string>();

        if (currentSaveDta.CollectablesCollected == null)
            currentSaveDta.CollectablesCollected = new List<int>();
    }
}

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
    [SerializeField] private string _defaultPath = "Assets\\Save Files\\";
    [SerializeField] private string _defaultfFileName = "Save File";
    [SerializeField] private string _fileType = "json";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: move this from start later, probably
        LoadData();
    }

    public void MarkSceneAsCompleted(string sceneName, bool autoSave=true)
    {
        if (currentSaveDta == null) LoadData();
        if (currentSaveDta == null) currentSaveDta = new SaveDataFile();
        if (currentSaveDta.ScenesCompleted == null)
            currentSaveDta.ScenesCompleted = new List<string>();

        if (IsLevelCompleted(sceneName))
            Debug.Log("This level has already been completed");
        else
            currentSaveDta.ScenesCompleted.Add(sceneName);

        if (autoSave)
            SaveData();
    }

    public void MarkCollectableAsCollected(Collectable collectable, bool autoSave = true)
    {
        if (currentSaveDta == null) LoadData();
        if (currentSaveDta == null) currentSaveDta = new();
        if (currentSaveDta.CollectablesCollected == null)
            currentSaveDta.CollectablesCollected = new List<int>();

        if(IsCollectableCollected(collectable))
            Debug.Log("Collectable has already been collected");
        else
            currentSaveDta.CollectablesCollected.Add((int)collectable);

        if (autoSave)
            SaveData();
    }

    public bool IsLevelCompleted(string sceneName)
    {
        if (currentSaveDta == null) LoadData();
        if (currentSaveDta == null) return false;
        if (currentSaveDta == null || currentSaveDta.ScenesCompleted == null) return false;
        
        return currentSaveDta.ScenesCompleted.Contains(sceneName);
    }

    public bool IsCollectableCollected(Collectable collectable)
    {
        if (currentSaveDta == null) LoadData();
        if (currentSaveDta == null || currentSaveDta.CollectablesCollected == null) return false;

        return currentSaveDta.CollectablesCollected.Contains((int)collectable);
    }

    [Button]
    public void LoadData()
    {
        if(saveFile == null)
        {
            Debug.LogWarning("No save file is present to load");
            currentSaveDta = new();
            return;
        }

        currentSaveDta = JsonUtility.FromJson<SaveDataFile>(saveFile.text);

        Debug.Log($"Loaded save file:\n{currentSaveDta.CollectablesCollected.Count} collectables\n{currentSaveDta.ScenesCompleted.Count} levels completed");
    }

    [Button]
    /// <summary>
    /// Overrides current text asset
    /// </summary>
    public void SaveData()
    {
        if(saveFile == null)
        {
            SaveDataAsNewFile();
            return;
        }    

        string path = AssetDatabase.GetAssetPath(saveFile);
        //var textFile = File.CreateText(path);

        string elemString = JsonUtility.ToJson(currentSaveDta);
        //textFile.WriteLine(elemString);

        Debug.Log($"Overwriting save file at {path}");

        File.WriteAllText(path, elemString);

    }

    [Button]
    /// <summary>
    /// Creates and saves new text asset
    /// </summary>
    public void SaveDataAsNewFile()
    {
        string path = GetNewPath();
        var textFile = File.CreateText(path);

        string elemString = JsonUtility.ToJson(currentSaveDta);
        textFile.WriteLine(elemString);

        if(saveFile == null)
            saveFile = new TextAsset(path);

        textFile.Close();
        Debug.Log($"Created new save file at {path}");
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
        if (saveFile == null)
        {
            Debug.LogWarning("No save file is present!");
            currentSaveDta = new();
            return;
        }

        Debug.Log($"Deleting save file at {AssetDatabase.GetAssetPath(saveFile)}");

        string fileContents = saveFile.text;
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(saveFile));

        Debug.Log($"Backup of file contents (copy paste to new file if deleting was a mistake):\n{fileContents}");

        saveFile= null; 

    }

    private string GetNewPath()
    {
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

    }
}

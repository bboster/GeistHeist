/*
 * Contributors: Toby
 * Creation: 10/1/25
 * Last Edited: 10/2/25
 * Summary: Manages player save data. Allows save and loading functionality.
 * Singleton, dont destroy on load
 */

using NaughtyAttributes;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveDataManager : DontDestroyOnLoadSingleton<SaveDataManager>
{
    [SerializeField] private TextAsset saveFile;
    private SaveDataFile currentSaveDta;

    [Header("Export Settings")]
    [SerializeField] private string _defaultpath = "Assets\\Save Files\\";
    [SerializeField] private string _defaultfFileName = "Save File";
    [SerializeField] private string _fileType = "JSON";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: move this from start later, probably
        LoadData();
    }

    public void MarkSceneAsCompleted(string sceneName, bool autoSave=true)
    {
        currentSaveDta.ScenesCompleted.Add(sceneName);

        if (autoSave)
            SaveData();
    }

    public void MarkCollectableAsCompleted(Collectable obj, bool autoSave = true)
    {
        currentSaveDta.CollectablesCollected.Add((int)obj);

        if (autoSave)
            SaveData();
    }

    [Button]
    public void LoadData()
    {
        if(saveFile == null)
        {
            Debug.LogWarning("No save file is present!");
            currentSaveDta = new();
            return;
        }

        JsonUtility.FromJson<SaveDataFile>(saveFile.text);
    }

    [Button]
    /// <summary>
    /// Overrides current text asset
    /// </summary>
    public void SaveData()
    {
        string path = saveFile == null ? GetNewPath() : AssetDatabase.GetAssetPath(saveFile);
        var textFile = File.CreateText(path);

        string elemString = JsonUtility.ToJson(currentSaveDta);
        textFile.WriteLine(elemString);

        Debug.Log($"Overwrote save file at {path}");
    }

    [Button]
    /// <summary>
    /// Overrides current text asset
    /// </summary>
    public void SaveDataAsNewFile()
    {
        string path = GetNewPath();
        var textFile = File.CreateText(path);

        string elemString = JsonUtility.ToJson(currentSaveDta);
        textFile.WriteLine(elemString);

        if(saveFile == null)
            saveFile = new TextAsset(path);

        Debug.Log($"Created new save file at {path}");
    }

    private string GetNewPath()
    {
        int number = 1;
        while (true)
        {
            string path = "";

            if (number == 1)
                path = _defaultpath + _defaultfFileName + "." + _fileType;
            else
                path = _defaultpath + _defaultfFileName + " " + number + "." + _fileType;

            if (!File.Exists(path))
                return path;

            number++;
        }

    }
}

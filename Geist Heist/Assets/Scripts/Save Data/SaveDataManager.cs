/*
 * Contributors: Toby, Josh
 * Creation: 10/1/25
 * Last Edited: 11/18/25
 * Summary: Manages player save data. Allows save and loading functionality.
 * Singleton, dont destroy on load
 */

using NaughtyAttributes;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveDataManager : DontDestroyOnLoadSingleton<SaveDataManager>
{
    [Header("Save Data")]
    [SerializeField] private TextAsset saveFile;
    private SaveDataFile currentSaveDta;

    [Header("Export Settings")]
    [SerializeField] private string _defaultPath = "Assets/Save Files/";
    [SerializeField] private string _defaultfFileName = "Save File";
    [SerializeField] private string _fileType = "json";

    [Header("Scene Transition")]
    [SerializeField, Scene] private List<string> ScenesToExcludeFromCompletionCount;
    [SerializeField, Scene] private List<string> ScenesRequiredForCompletion;

    //the scene needed to be completed before a cutscene happens (ex. level 1 is required for level 2 cutscene)
    [Scene] public List<string> ScenesToCutscene;

    [Header("Debug")]
    [Tooltip("If true, does not save any data")]
    public bool DontSaveData = false;
    // Debug buttons will be under this area

    private string runtimeSavePath;

    public void Start()
    {
        runtimeSavePath = Path.Combine(Application.persistentDataPath, $"{_defaultfFileName}.{_fileType}");
        LoadData();
    }


    #region Collectables

    public bool IsCollectableCollected(Collectable collectable)
    {
        EnsureSaveData();
        return currentSaveDta.CollectablesCollected.Contains((int)collectable);
    }

    public void MarkCollectableAsCollected(Collectable collectable, bool autoSave = true)
    {
        EnsureSaveData();

        if (IsCollectableCollected(collectable))
            Debug.Log($"{collectable.ToString()} has already been collected");
        else
            currentSaveDta.CollectablesCollected.Add((int)collectable);

        if (autoSave)
            SaveData();
    }

    public void SetCollectableState(Collectable collectable, bool collectedState, bool autoSave = true)
    {
        EnsureSaveData();

        // try to collect
        if (collectedState)
        {
            if (IsCollectableCollected(collectable))
                Debug.Log($"{collectable.ToString()} has already been collected");
            else
                currentSaveDta.CollectablesCollected.Add((int)collectable);
        }
        else
        {
            if (IsCollectableCollected(collectable))
                currentSaveDta.CollectablesCollected.Remove((int)collectable);
            else
                Debug.Log($"{collectable.ToString()} has already been collected");
        }

        if (autoSave)
            SaveData();
    }

    public int GetCollectedCount()
    {
        EnsureSaveData();
        return currentSaveDta.CollectablesCollected.Count();
    }

    #endregion

    #region Hats
    public int EquipedHat()
    {
        EnsureSaveData();
        return currentSaveDta.CollectableWearing;
    }

    public void MarkCollectableAsWorn(Collectable collectable, bool autoSave = true)
    {
        EnsureSaveData();
        currentSaveDta.CollectableWearing = (int)collectable;
        if (autoSave)
            SaveData();
    }

    /// <returns>True if collectable is one currently being worn</returns>
    public bool IsHatEqupped(Collectable collectable)
    {
        EnsureSaveData();

        // if wearing none
        if ((int)currentSaveDta.CollectableWearing <= 0 && (int)collectable <= 0) return true;

        return (int)collectable == currentSaveDta.CollectableWearing;
    }

#endregion

    #region Level Completion

    /// <summary>
    /// Return true if level is stored in list of saved completed levels
    /// </summary>
    public bool IsLevelCompleted(string sceneName)
    {
        EnsureSaveData();
        return currentSaveDta.ScenesCompleted.Contains(sceneName);
    }

    public void MarkSceneAsCompleted(string sceneName, bool autoSave = true)
    {
        EnsureSaveData();

        if (IsLevelCompleted(sceneName))
            Debug.Log("This level has already been completed");
        else
        {
            currentSaveDta.ScenesCompleted.Add(sceneName);
        }
            

        if (autoSave)
            SaveData();
    }

    public void SetLevelCompletionState(string sceneName, bool collectedState, bool autoSave = true)
    {
        EnsureSaveData();

        // try to collect
        if (collectedState && !IsLevelCompleted(sceneName))
        {
            currentSaveDta.ScenesCompleted.Add(sceneName);

            if (autoSave) SaveData();
        }
        else if (!collectedState && IsLevelCompleted(sceneName))
        {
            currentSaveDta.ScenesCompleted.Remove(sceneName);

            if (autoSave) SaveData();
        }

    }

    public void MarkSceneAsCompleted(int sceneIndex, bool autoSave = true)
    {
        string sceneName = StaticUtilities.BuildIndexToSceneName(sceneIndex);
        MarkSceneAsCompleted(sceneName, autoSave);
    }

    /// <summary>
    /// Specifically excludes scenes like main menu, lobby, globe, etc.
    /// </summary>
    public int GetLevelsCompletedCount()
    {
        EnsureSaveData();
        return currentSaveDta.ScenesCompleted
            .Where(s => ScenesToExcludeFromCompletionCount.Contains(s) == false)
            .Count();
    }

    /// <summary>
    /// Sees if all scenes in required list are completed
    /// </summary>
    public bool AllLevelsCompleted()
    {
        EnsureSaveData();

        foreach (var level in ScenesRequiredForCompletion)
        {
            if (currentSaveDta.ScenesCompleted.Contains(level) == false)
            {
                //Debug.Log($"Player has not completed game because they need to complete {level}");
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Return true if level is stored in list of saved completed levels
    /// </summary>
    public bool IsSceneCutsceneCompleted(string sceneName)
    {
        EnsureSaveData();
        return currentSaveDta.CutscenesCompleted.Contains(sceneName);
    }

    public void MarkSceneCutsceneAsCompleted(string sceneName, bool autoSave = true)
    {
        EnsureSaveData();

        if (IsSceneCutsceneCompleted(sceneName))
            Debug.Log("This cutscene has already been completed");
        else
            currentSaveDta.CutscenesCompleted.Add(sceneName);

        if (autoSave)
            SaveData();
    }

    #endregion

    #region Flavor Text

    /// <summary>
    /// Return true if level is stored in list of saved completed levels
    /// </summary>
    public bool IsFlavorTextRead(List<DialogueTextData> text)
    {
        EnsureSaveData();
        int hash = text.GetHashCode();
        //Debug.Log($"Read display text: {hash}: {currentSaveDta.FlavorTextsRead.Contains(hash)}");
        return currentSaveDta.FlavorTextsRead.Contains(hash);
    }

    public void MarkFlavorTextAsRead(List<DialogueTextData> text, bool autoSave = true)
    {
        EnsureSaveData();

        int hash = text.GetHashCode();
        Debug.Log($"Saving display text: {hash}");

        if (IsFlavorTextRead(text))
            Debug.LogWarning("This flavor text was already read (is there duplicate text across multiple objects?)");
        else
            currentSaveDta.FlavorTextsRead.Add(hash);

        if (autoSave)
            SaveData();
    }

    #endregion

    #region Toy Car

    public bool HasPlayerMovedWithCar()
    {
        EnsureSaveData();
        return currentSaveDta.HasPlayerMovedWithCar;
    }

    public void SetHasPlayerMovedWithCar(bool hasDriven, bool autoSave = true)
    {
        EnsureSaveData();
        currentSaveDta.HasPlayerMovedWithCar = hasDriven;
        if (autoSave) SaveData();
    }

    #endregion

    #region Achievements
    public void VasePossessed()
    {
        currentSaveDta.VasesPossesed++;
    }

    public int getVasesPossessed()
    {
        return currentSaveDta.VasesPossesed;
    }

    public void CarDiven()
    {
        currentSaveDta.CarsDriven++;
    }

    public int getCarsDriven()
    {
        return currentSaveDta.CarsDriven;
    }

    public void CarBonked()
    {
        currentSaveDta.CarsBonked++;
    }

    public int getCarsBonked()
    {
        return currentSaveDta.CarsBonked;
    }

    public void CanThrown()
    {
        currentSaveDta.CansThrown++;
    }

    public int getCansThrown()
    {
        return currentSaveDta.CansThrown;
    }
    public void flavorTextRead()
    {
        currentSaveDta.FlavorTextRead++;
    }

    public int getFlavorTextRead()
    {
        return currentSaveDta.FlavorTextRead;
    }


    #endregion


    #region File Manipulation

    public bool DoesSaveDataExist()
    {
        return (saveFile != null);
    }

    [Button]
    public void LoadData()
    {
#if UNITY_EDITOR

        try
        {
            if (saveFile == null)
            {
                Debug.LogWarning("No save file is present to load");
                currentSaveDta = new();
                return;
            }

            currentSaveDta = JsonUtility.FromJson<SaveDataFile>(saveFile.text);

            Debug.Log($"Loaded save file:\n{currentSaveDta.CollectablesCollected.Count} collectables\n{currentSaveDta.ScenesCompleted.Count} levels completed");
        }

        catch
        {
            currentSaveDta = new();
        }
        
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

        if (currentSaveDta.FlavorTextsRead == null)
            currentSaveDta.FlavorTextsRead = new();
    }

    #endregion
}

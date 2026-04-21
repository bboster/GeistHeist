/*
 * Contributors: Brenden, Toby
 * Creation Date: 10/21/25
 * Last Modified: 4/20/2026
 * 
 * Brief Description: handles the commands from the debug console
 */

using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject Console;
    [SerializeField] TMPro.TMP_InputField inputs;
    [SerializeField] TMPro.TMP_Text TextArea;
    [SerializeField] ScrollRect logScrollRect;
    [SerializeField] GameObject[] Prefabs;

    [SerializeField] GameObject FreeCamPrefab;
    public GameObject FreeCamInstance;

    private bool noClipToggle = false;
    private bool godToggle = false;
    private bool cameraToggle = false;
    private bool freezeToggle = false;

    GameObject Player => PlayerManager.Instance.PlayerGhostObject.gameObject;

    GameObject cameraGO => PlayerManager.Instance.camera.gameObject;

    private static CollectableRegistry collectableRegistry;

    private void Start()
    {
        Console.SetActive(false);
        InputEvents.DebugStarted.AddListener(ToggleConsole);
        FreeCamInstance = Instantiate(FreeCamPrefab, cameraGO.transform.position, Quaternion.identity);

        if (collectableRegistry == null)
            collectableRegistry = Resources.Load<CollectableRegistry>(CollectableRegistry.RESOURCE_PATH);

        if (logScrollRect == null && TextArea != null)
            logScrollRect = TextArea.GetComponentInParent<ScrollRect>();

        if (TextArea != null)
            TextArea.alignment = TMPro.TextAlignmentOptions.BottomLeft;

        StartCoroutine(ScrollToBottomNextFrame());
    }


    public void ToggleConsole()
    {
        Debug.Log("In toggle");
        if (Console != null)
        {

            if (Console.activeSelf)
            {
                Console.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Console.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                inputs.ActivateInputField();
                StartCoroutine(ScrollToBottomNextFrame());
            }
        }
        else
        {
            Debug.Log("No Console Exists");
        }
    }

    public void CallFunction()
    {
        string Command = inputs.text.ToLower();

        inputs.text = "";
        inputs.ActivateInputField();

        if (Command.IsEmptyOrNull<string>())
        {
            Debug.LogWarning("Empty debug command");
            return;
        }

        if (Command == "help")
        {
            AppendConsoleLine(
                Command + "\nNo Clip: nc \nGod Mode: god \nDetatch Camera: dc \n"+
                "Freeze Guards: freeze \n" +
                "List: list <\"scenes\"/\"hats\">\n"+
                "Load Scene: scene <Scene Name/Scene Index> \n"+
                "Complete: c <\"scene\"/\"hat\"> <Scene Name/Scene Index/\"all\"/\"none\"/\"random\">\n" +
                "Spawn Item on camera: spawn <Item Name/Item Index> \n"+
                "Change Players Speed: speed <Speed Value(or \"default\")>"
            );
            return;
        }

        // noclip
        if (Command == "nc" || Command == "noclip")
        {
            NoClip();
            AppendConsoleLine(Command + " " + noClipToggle);
            return;
        }

        // God Mode
        if(Command == "god")
        {
            GodMode();
            AppendConsoleLine(Command + " " + godToggle);
            return;
        }

        // disconnect
        if(Command == "dc" || Command=="freecam")
        {
            FreeCam();
            AppendConsoleLine(Command + " " + cameraToggle);
            return;
        }

        // List Scene
        if (Command.StartsWith("ls") || Command.StartsWith("list"))
        {
            if(Command.EndsWith("hats") || Command.EndsWith("h") || Command.EndsWith("hat"))
            {
                ListHats();
                return;
            }

            if(Command.EndsWith("scene") || Command.EndsWith("s") || Command.EndsWith("scenes") || Command.EndsWith("levels"))
            {
                listScenes();
                return;
            }

            AppendConsoleLine("<color=red>Please enter \"list hats\" or \"list scenes\"</color>");
            return;
        }


        // "scene _..."
        if (Command.StartsWith("scene"))
        {
            if (Command.Length >= 7)
            {
                TryLoadNewScene(Command.Substring(6, Command.Length - 6));
                AppendConsoleLine("<color=red>Scene Failed to load, Please input a valid scene</color>");
            }
            else
            {
                AppendConsoleLine($"<color=red>{Command} Invalid Scene name or index, Please input a valid scene</color>");
                listScenes();
            }
            return;
        }

        #region c / complete / collect

        if (Command.StartsWith("c"))
        {
            if(Command.Length <= 2)
            {
                AppendConsoleLine($"<color=red>{Command} <\"scene\"/\"hat\"> <name/index></color>");
                return;
            }

            TryCompleteCommand(Command.Substring(2, Command.Length-2));
            return;
        }

        if (Command.StartsWith("collect"))
        {
            TryCompleteCommand(Command.Substring(8, Command.Length - 2));
            return;
        }

        if (Command.StartsWith("complete"))
        {
            TryCompleteCommand(Command.Substring(9, Command.Length-9));
            return;
        }

        #endregion

        if (Command == "freeze")
        {
            //waiting for jacob to implement - someone should implement this
            Debug.Log("Freeze");
            AppendConsoleLine(Command + " " + freezeToggle);
            return;
        }

        // spawn item
        if (Command.StartsWith("spawn"))
        {
            if (Command.Length >= 7)
            {
                spawnItem(Command.Substring(6, Command.Length - 6));
            }
            else
            {
                AppendConsoleLine(Command + " Invalid item, Please input a valid item");
            }
            return;
        }

        // player speed
        if (Command.StartsWith("speed"))
        {
            if (Command.Equals("speed default"))
            {
                float defaultSpeed = Player.GetComponent<ThirdPersonInputHandler>().defaultSpeed;
                AppendConsoleLine(Command + " ~ Speed set to default: " + defaultSpeed);
                PlayerSpeed(defaultSpeed);
                return;
            }

            if (Command.Length >= 7)
            {
                int Temp;
                if (int.TryParse(Command.Substring(6, Command.Length - 6), out Temp))
                {
                    AppendConsoleLine(Command + "~ Speed set to: " + Temp);
                    PlayerSpeed(Temp);
                }
                else
                {
                    AppendConsoleLine(Command + " Please put a number after the command");
                }
            }
            else
            {
                AppendConsoleLine(Command + " Please put the speed number (or \"default\") after the command");
            }
            return;
        }

        AppendConsoleLine(Command + " No command found, use Help for all commands");
        Debug.LogWarning("no command found found for " + Command);

    }

    private void AppendConsoleLine(string line)
    {
        TextArea.text = TextArea.text + "\n" + line;
        StartCoroutine(ScrollToBottomNextFrame());
    }

    private void NoClip()
    {
        noClipToggle = !noClipToggle;
        Player.GetComponent<CapsuleCollider>().enabled = !noClipToggle;
        Player.GetComponentInChildren<SphereCollider>().enabled = !noClipToggle;
        Player.GetComponent<Rigidbody>().useGravity = !noClipToggle;
    }

    private void GodMode()
    {
        godToggle = !godToggle;
        GameManager.Instance.InGodMode = godToggle;
    }

    private void FreeCam()
    {
        cameraToggle = !cameraToggle;
        if (cameraToggle)
        {
            PlayerManager.Instance.PossessFreecam(FreeCamInstance.gameObject.GetComponent<PossessableObject>());
        }
        else
        {
            PlayerManager.Instance.PossessGhost(FreeCamInstance.gameObject.GetComponent<PossessableObject>());
        }
    }

    private void listScenes()
    {
        AppendConsoleLine("All Scenes (Debug Names)");
        /*
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            AppendConsoleLine(i + ": " + sceneName);
        } 
        */
        for (int i = 0; i<LevelManager.Instance.LevelNames.Count; i++)
        {
            var debugName = LevelManager.Instance.LevelNames[i].InternalDebugName;
            var sceneName = LevelManager.Instance.LevelNames[i].SceneName;
            if (debugName == "")
            {
                Debug.Log(sceneName + " does not have an internal debug name");
                continue;
            }
            bool completed = SaveDataManager.Instance.IsLevelCompleted(sceneName);
            //bool isCurrentLevel = SceneManager.GetActiveScene().name == sceneName;

            AppendConsoleLine($"{i}: {debugName} {(completed ? "(<color=yellow>Completed</color>)" : "")}");
        }
    }

    private void ListHats()
    {
        AppendConsoleLine("All Hats");
        
        for (int i = 0; i < collectableRegistry.Entries.Count; i++)
        {
            var hat = collectableRegistry.Entries[i];
            bool collected = SaveDataManager.Instance.IsCollectableCollected(hat.collectable);
            //bool isCurrentLevel = SceneManager.GetActiveScene().name == sceneName;

            AppendConsoleLine($"{i}: {hat.collectable.ToString()} {(collected ? "(<color=yellow>Collected</color>)" : "")}");
        }
    }


    private IEnumerator ScrollToBottomNextFrame()
    {
        yield return null;

        if (logScrollRect == null)
            yield break;

        Canvas.ForceUpdateCanvases();
        if (logScrollRect.content != null)
            LayoutRebuilder.ForceRebuildLayoutImmediate(logScrollRect.content);

        logScrollRect.verticalNormalizedPosition = 0f;
    }

    private string GetRealSceneIndexFromInput(int input)
    {
        int index = 0;
        // make the index match the list that appears with the 'list' command
        for (int i = 0; i < LevelManager.Instance.LevelNames.Count; i++)
        {
            var debugName = LevelManager.Instance.LevelNames[i].InternalDebugName;
            if (debugName == "")
            {
                continue;
            }
            index++;

            if (index == input)
            {
                return LevelManager.Instance.LevelNames[i].SceneName;
            }
        }
        return "";
        
    }

    private string GetRealHatNameFromInput(int input)
    {
        return collectableRegistry.Entries[input].collectable.ToString();
    }


    private void TryLoadNewScene(string input)
    {
        // if user entered index
        int Temp;
        if (int.TryParse(input, out Temp))
        {
            SceneLoadManager.Instance.LoadScene(GetRealSceneIndexFromInput(Temp));
            return;
        }

        // if user entered debug name
        if(LevelManager.Instance.LevelNames.Select(n => n.InternalDebugName).Contains(input))
        {
            string sceneToLoad = LevelManager.Instance.LevelNames.Where(n => n.InternalDebugName == input).First().SceneName;
            SceneLoadManager.Instance.LoadScene(sceneToLoad);
        }

        // if user entered scene name
        SceneLoadManager.Instance.LoadScene(input);
    }

    /// <summary>
    /// Command is input string without "c " or "complete "
    /// </summary>
    /// <param name="command"></param>
    private void TryCompleteCommand(string command)
    {
        // Get the word before the space, or the whole string if no space exists
        int index = command.IndexOf(' ');
        string firstWord = index == -1 ? command : command.Substring(0, index);

        // complete levels
        if (firstWord == "level" || firstWord == "scene" || firstWord == "l" || firstWord == "h")
        {
            TryCompleteScene(command.Substring(firstWord.Length + 1, command.Length - firstWord.Length - 1));
            return;
        }

        if (firstWord == "hats" || firstWord == "h" || firstWord == "hat")
        {
            TryCollectHat(command.Substring(firstWord.Length + 1, command.Length - firstWord.Length - 1));
            return;
        }

        AppendConsoleLine($"<color=red>{command} Invalid Scene name or index or hat, Please input a valid scene or hat</color>");
        AppendConsoleLine($"<color=red>{command} Use the \"list scenes\" or \"list hats\" command</color>");
    }
    private void TryCompleteScene(string input)
    {
        if (input == "all")
        {
            foreach (var sceneName in LevelManager.Instance.LevelNames.Select(n => n.SceneName))
            {
                SaveDataManager.Instance.MarkSceneAsCompleted(sceneName, autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            listScenes();
            AppendConsoleLine($"<color=green>all scenes have been marked as completed</color>");
            return;
        }

        if (input == "random")
        {
            foreach (var sceneName in LevelManager.Instance.LevelNames.Select(n => n.SceneName))
            {
                SaveDataManager.Instance.SetLevelCompletionState(sceneName, (UnityEngine.Random.value > 0.5), autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            listScenes();
            AppendConsoleLine($"<color=green>random levels have been marked as completed</color>");
            return;
        }

        if (input == "none")
        {
            foreach (var sceneName in LevelManager.Instance.LevelNames.Select(n => n.SceneName))
            {
                SaveDataManager.Instance.SetLevelCompletionState(sceneName, false, autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            listScenes();
            AppendConsoleLine($"<color=green>all levels have been marked as not completed</color>");
            return;
        }

        // if user entered index
        int Temp;
        if (int.TryParse(input, out Temp))
        {
            FulfillCompleteScene(GetRealSceneIndexFromInput(Temp));
            return;
        }

        // if user entered debug name
        if (LevelManager.Instance.LevelNames.Select(n => n.InternalDebugName).Contains(input))
        {
            string sceneToLoad = LevelManager.Instance.LevelNames.Where(n => n.InternalDebugName == input).First().SceneName;
            FulfillCompleteScene(sceneToLoad);
            return;
        }

        // if user entered scene name
        FulfillCompleteScene(input);
        return;
    }
    private void FulfillCompleteScene(string sceneName)
    {
        SaveDataManager.Instance.MarkSceneAsCompleted(sceneName);
        listScenes();
        AppendConsoleLine($"<color=green>{sceneName} has been marked as completed</color>");
    }

    private void TryCollectHat(string input)
    {
        if (input == "all")
        {
            foreach(var hat in collectableRegistry.Entries.Select(e => e.collectable))
            {
                SaveDataManager.Instance.MarkCollectableAsCollected(hat, autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            ListHats();
            AppendConsoleLine($"<color=green>all hats have been marked as collected</color>");
            return;
        }

        if (input == "random")
        {
            foreach (var hat in collectableRegistry.Entries.Select(e => e.collectable))
            {
                SaveDataManager.Instance.SetCollectableState(hat, (UnityEngine.Random.value > 0.5f), autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            ListHats();
            AppendConsoleLine($"<color=green>random hats have been marked as collected</color>");
            return;
        }

        if (input == "none")
        {
            foreach (var hat in collectableRegistry.Entries.Select(e => e.collectable))
            {
                SaveDataManager.Instance.SetCollectableState(hat, false, autoSave: false);
            }
            SaveDataManager.Instance.SaveData();
            ListHats();
            AppendConsoleLine($"<color=green>all hats have been marked as not collected</color>");
            return;
        }

        // if user entered index
        int Temp;
        if (int.TryParse(input, out Temp))
        {
            FulfillCollectHat(GetRealHatNameFromInput(Temp));
            return;
        }

        // if user entered hat name
        FulfillCollectHat(input);
        return;
    }
    private void FulfillCollectHat(string hatName, bool collected = true)
    {
        Collectable hat = collectableRegistry.Entries.Where(e=> e.collectable.ToString() == hatName).First().collectable;
        SaveDataManager.Instance.MarkCollectableAsCollected(hat);
        ListHats();
        AppendConsoleLine($"<color=green>{hatName} has been marked as collected</color>");
    }

    private void spawnItem(string itemName)
    {
        int Temp;
        if (int.TryParse(itemName, out Temp))
        {
            Instantiate(Prefabs[0], cameraGO.transform.position, Quaternion.identity);
        }
        else
        {
            //maybe three variables instead of the array indexes? In case they get jumbled/we add more items to spawn
            if (itemName == "vase")
            {
                Instantiate(Prefabs[0], cameraGO.transform.position, Quaternion.identity);
            }
            else if (itemName == "vending" || itemName == "vending machine")
            {
                Instantiate(Prefabs[1], cameraGO.transform.position, Quaternion.identity);
            }
            else if (itemName == "car" || itemName == "toy car")
            {
                Instantiate(Prefabs[2], cameraGO.transform.position, Quaternion.identity);
            }
        }
        Instantiate(Prefabs[0]);
    }

    private void PlayerSpeed(float Speed)
    {
        Player.GetComponent<ThirdPersonInputHandler>().speed = Speed;
    }
}

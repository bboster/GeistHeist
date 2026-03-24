/*
 * Contributors: Brenden
 * Creation Date: 10/21/25
 * Last Modified: 1/27/2026
 * 
 * Brief Description: handles the commands from the debug console
 */

using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject Console;
    [SerializeField] TMPro.TMP_InputField inputs;
    [SerializeField] TMPro.TMP_Text TextArea;
    [SerializeField] GameObject[] Prefabs;

    [SerializeField] GameObject FreeCamPrefab;
    public GameObject FreeCamInstance;

    private bool noClipToggle = false;
    private bool godToggle = false;
    private bool cameraToggle = false;
    private bool freezeToggle = false;

    GameObject Player => PlayerManager.Instance.PlayerGhostObject.gameObject;

    GameObject cameraGO => PlayerManager.Instance.camera.gameObject;

    private void Start()
    {
        Console.SetActive(false);
        InputEvents.DebugStarted.AddListener(ToggleConsole);
        FreeCamInstance = Instantiate(FreeCamPrefab, cameraGO.transform.position, Quaternion.identity);
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
            TextArea.text = TextArea.text + "\n" + Command + "\nNo Clip: nc \nGod Mode: god \nDetatch Camera: dc \nFreeze Guards: freeze " +
                "\nList Scene: ls \nLoad Scene: scene <Scene Name/Scene Index> \nSpawn Item on camera: spawn <Item Name/Item Index> \nChange Players Speed: speed <Speed Value(or \"default\") >";
            return;
        }

        // noclip
        if (Command == "nc" || Command == "noclip")
        {
            NoClip();
            TextArea.text = TextArea.text + "\n" + Command + " " + noClipToggle;
            return;
        }

        // God Mode
        if(Command == "god")
        {
            GodMode();
            TextArea.text = TextArea.text + "\n" + Command + " " + godToggle;
            return;
        }

        // disconnect
        if(Command == "dc" || Command=="freecam")
        {
            FreeCam();
            TextArea.text = TextArea.text + "\n" + Command + " " + cameraToggle;
            return;
        }

        // Load Scene
        if (Command.Substring(0, 2) == "ls")
        {
            listScenes();
            return;
        }


        // "scene _..."
        if (Command.Substring(0, 5) == "scene")
        {
            if (Command.Length >= 7)
            {
                LoadNewScene(Command.Substring(6, Command.Length - 6));
                TextArea.text = TextArea.text + "\n" + "Scene Failed to load, Please input a valid scene";
            }
            else
            {
                TextArea.text = TextArea.text + "\n" + Command + " Invalid Scene name or index, Please input a valid scene";
            }
            return;
        }

        if (Command == "freeze")
        {
            //waiting for jacob to implement - someone should implement this
            Debug.Log("Freeze");
            TextArea.text = TextArea.text + "\n" + Command + " " + freezeToggle;
            return;
        }

        // spawn item
        if (Command.Substring(0, 5) == "spawn")
        {
            if (Command.Length >= 7)
            {
                spawnItem(Command.Substring(6, Command.Length - 6));
            }
            else
            {
                TextArea.text = TextArea.text + "\n" + Command + " Invalid item, Please input a valid item";
            }
            return;
        }

        // player speed
        if (Command.Substring(0, 5) == "speed")
        {
            if (Command.Equals("speed default"))
            {
                float defaultSpeed = Player.GetComponent<ThirdPersonInputHandler>().defaultSpeed;
                TextArea.text = TextArea.text + "\n" + Command + " ~ Speed set to default: " + defaultSpeed;
                PlayerSpeed(defaultSpeed);
                return;
            }

            if (Command.Length >= 7)
            {
                int Temp;
                if (int.TryParse(Command.Substring(6, Command.Length - 6), out Temp))
                {
                    TextArea.text = TextArea.text + "\n" + Command + "~ Speed set to: " + Temp;
                    PlayerSpeed(Temp);
                }
                else
                {
                    TextArea.text = TextArea.text + "\n" + Command + " Please put a number after the command";
                }
            }
            else
            {
                TextArea.text = TextArea.text + "\n" + Command + " Please put the speed number (or \"default\") after the command";
            }
            return;
        }

        TextArea.text = TextArea.text + "\n" + Command + " No command found, use Help for all commands";
        Debug.LogWarning("no command found found for " + Command);

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
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            TextArea.text = TextArea.text + "\n" + i + ": " + sceneName;
        }   
    }
    private void LoadNewScene(String sceneName)
    {
        int Temp;
        if (int.TryParse(sceneName, out Temp))
        {
            SceneManager.LoadScene(Temp);
        }
        else
        {

            SceneManager.LoadScene(sceneName);
        }
    }

    private void spawnItem(String itemName)
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

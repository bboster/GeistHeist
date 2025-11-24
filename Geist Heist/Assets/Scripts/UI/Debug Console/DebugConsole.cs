/*
 * Contributors: Brenden
 * Creation Date: 10/21/25
 * Last Modified: 11/23/25
 * 
 * Brief Description: handles the commands from the debug console
 */
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject Console;
    [SerializeField] TMPro.TMP_InputField inputs;
    [SerializeField] TMPro.TMP_Text TextArea;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject cameraGO;
    [SerializeField] GameObject[] Prefabs;

    [SerializeField] GameObject FreeCamPrefab;
    public GameObject FreeCamInstance;

    private bool noClipToggle = false;
    private bool godToggle = false;
    private bool cameraToggle = false;
    private bool freezeToggle = false;

    private void Start()
    {
        Console.SetActive(false);
        InputEvents.DebugStarted.AddListener(ToggleConsole);
        Player = FindFirstObjectByType<ThirdPersonInputHandler>().gameObject;
        cameraGO = FindFirstObjectByType<Camera>().gameObject;
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

        if(Command == "nc")
        {
            NoClip();
            TextArea.text = TextArea.text + "\n" + Command + " " + noClipToggle;
        }
        else if(Command == "god")
        {
            GodMode();
            TextArea.text = TextArea.text + "\n" + Command + " " + godToggle;
        }
        else if(Command == "dc")
        {
            FreeCam();
            TextArea.text = TextArea.text + "\n" + Command + " " + cameraToggle;
        }
        else if(Command.Substring(0, 2) == "ls")
        {
            if(Command.Length >= 4)
            {
                LoadNewScene(Command.Substring(3, Command.Length - 3));
                TextArea.text = TextArea.text + "\n" + "Scene Failed to load, Please input a valid scene";
            }
            else
            {
                TextArea.text = TextArea.text + "\n" + Command + " Invalid Scene name or index, Please input a valid scene";
            }
            
        }
        else if(Command == "freeze")
        {
            //waiting for jacob to implement
            Debug.Log("Freeze");
            TextArea.text = TextArea.text + "\n" + Command + " " + freezeToggle;
        }
        else if(Command == "help")
        {
            TextArea.text = TextArea.text + "\n" + Command + "\nNo Clip: nc \nGod Mode: god \nDetatch Camera: dc \nFreeze Guards: freeze " +
                "\nLoad Scene: scene <Scene Name/Scene Index> \nSpawn Item on camera: spawn <Item Name/Item Index> \nChange Players Speed: speed <Speed Value>";
        }
        else if(Command.Length > 4)
        {
            if (Command.Substring(0, 5) == "spawn")
            {
                if(Command.Length >= 7)
                {
                    spawnItem(Command.Substring(6, Command.Length - 6));
                }
                else
                {
                    TextArea.text = TextArea.text + "\n" + Command + " Invalid item, Please input a valid item";
                }
            }
            else if (Command.Substring(0, 5) == "scene")
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
            }
            else if (Command.Substring(0, 5) == "speed")
            {
                if (Command.Length >= 7)
                {
                    int Temp;
                    if (int.TryParse(Command.Substring(6, Command.Length - 6), out Temp))
                    {
                        PlayerSpeed(Temp);
                    }
                    else
                    {
                        TextArea.text = TextArea.text + "\n" + Command + " Please put a number after the command";
                    }
                }
                else
                {
                    TextArea.text = TextArea.text + "\n" + Command + " Please put the speed number after the command";
                }
            }
            else if(Command.Length != 0)
            {
                TextArea.text = TextArea.text + "\n" + Command + " No command found, use Help for all commands";
            }
        }
        else if(Command.Length != 0)
        {
            TextArea.text = TextArea.text + "\n" + Command + " No command found, use Help for all commands";
        }
        inputs.text = "";
        inputs.ActivateInputField();
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

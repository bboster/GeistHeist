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
        string Command = inputs.text;

        if(Command.ToLower() == "nc")
        {
            Debug.Log("no clip");
            noClipToggle = !noClipToggle;
            Player.GetComponent<CapsuleCollider>().enabled = !noClipToggle;
            Player.GetComponentInChildren<SphereCollider>().enabled = !noClipToggle;
            Player.GetComponent<Rigidbody>().useGravity = !noClipToggle;
            TextArea.text = TextArea.text + "\n" + Command + " " + noClipToggle;
        }
        else if(Command.ToLower() == "god")
        {
            Debug.Log("God");
            godToggle = !godToggle;
            GameManager.Instance.InGodMode = godToggle;
            TextArea.text = TextArea.text + "\n" + Command + " " + godToggle;
        }
        else if(Command.ToLower() == "dc")
        {
            //still haven't implemented
            Debug.Log("Detatch Camera");
            TextArea.text = TextArea.text + "\n" + Command + " " + cameraToggle;
        }
        else if(Command.ToLower().Substring(0,2) == "ls")
        {
            //Still a little buggy
            Debug.Log("Load Scene");
            int Temp;
            if(int.TryParse(Command.Substring(3, Command.Length - 3), out Temp))
            {
                SceneManager.LoadScene(Temp);
            }
            else
            {

                SceneManager.LoadScene(Command.Substring(3, Command.Length - 3));
            }
        }
        else if(Command.ToLower() == "freeze")
        {
            //waiting for jacob to implement
            Debug.Log("Freeze");
            TextArea.text = TextArea.text + "\n" + Command + " " + freezeToggle;
        }
        else if(Command.ToLower() == "help")
        {
            TextArea.text = TextArea.text + "\n" + Command + "\nNo Clip: nc \nGod Mode: god \nDetatch Camera: dc \nFreeze Guards: freeze " +
                "\nLoad Scene: ls <Scene Name/Scnene Index> \nSpawn Item: spawn <Item Name/Item Index>";
            Debug.Log("Help");
        }
        else if(Command.Length > 4)
        {
            if (Command.ToLower().Substring(0, 5) == "spawn")
            {
                int Temp;
                string PostString = Command.Substring(6,Command.Length - 6);
                Debug.Log("Spawn Item");
                if (int.TryParse(PostString, out Temp))
                {
                    Instantiate(Prefabs[0], cameraGO.transform.position, Quaternion.identity);
                }
                else
                {
                    if(PostString.ToLower() == "vase")
                    {
                        Instantiate(Prefabs[0], cameraGO.transform.position, Quaternion.identity);
                    }
                    else if (PostString.ToLower() == "vending" || PostString.ToLower() == "vending machine")
                    {
                        Instantiate(Prefabs[1], cameraGO.transform.position, Quaternion.identity);
                    }
                    else if (PostString.ToLower() == "car")
                    {
                        Instantiate(Prefabs[2], cameraGO.transform.position, Quaternion.identity);
                    }
                }
                Instantiate(Prefabs[0]);
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
}

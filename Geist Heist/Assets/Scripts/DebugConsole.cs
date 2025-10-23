using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject Console;
    [SerializeField] TMPro.TMP_InputField inputs;
    [SerializeField] TMPro.TMP_Text TextArea;
    [SerializeField] GameObject Player;

    private bool noClipToggle = false;
    private bool godToggle = false;

    private void Start()
    {
        Console.SetActive(false);
        InputEvents.DebugStarted.AddListener(ToggleConsole);
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
        TextArea.text = TextArea.text + "\n" + Command;
        if(Command.ToLower() == "nc")
        {
            Debug.Log("no clip");
            noClipToggle = !noClipToggle;
            //make the play no clip
        }
        else if(Command.ToLower() == "god")
        {
            Debug.Log("God");
            godToggle = !godToggle;
        }
        else if(Command.ToLower() == "dc")
        {
            Debug.Log("Detatch Camera");
        }
        else if(Command.ToLower() == "ls")
        {
            Debug.Log("Load Scene");
        }
        else if(Command.ToLower() == "freeze")
        {
            Debug.Log("Freeze");
        }
        else if(Command.ToLower() == "help")
        {
            Debug.Log("Help");
        }
        else if(Command.Length > 4)
        {
            if (Command.ToLower().Substring(0, 5) == "spawn")
            {
                Debug.Log("Spawn Item");
            }
        }
        inputs.text = "";
        inputs.ActivateInputField();
    }
}

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Windows;

public class DebugConsole : MonoBehaviour
{
    [SerializeField] GameObject Console;
    [SerializeField] InputField inputs;
    [SerializeField] TMPro.TMP_Text TextArea;
    private bool noClipToggle = false;
    private bool godToggle = false;

    private void Start()
    {
        Console.SetActive(false);
        InputEvents.MoveStarted.AddListener(ToggleConsole());
    }


    public UnityAction ToggleConsole()
    {
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
            return null;
        }
        else
        {
            Debug.Log("No Console Exists");
            return null;
        }
    }

    public void CallFunction()
    {
        string Command = inputs.text;
        TextArea.text = "\n" + Command;
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
        else if (Command.ToLower().Substring(0,5) == "spawn")
        {
            Debug.Log("Spawn Item");
        }
        inputs.text = "";
    }
}

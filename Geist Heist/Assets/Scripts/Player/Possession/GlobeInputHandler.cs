/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles inputs for the globe end scene
 */

using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobeInputHandler : IInputHandler
{
    [Header ("Required Variables")]

    [Tooltip("Camera for the ending swinging.")]
    [SerializeField] private CinemachineCamera globeSwingCamera;
    [Tooltip("Camera for the ending rolling before hallway.")]
    [SerializeField] private CinemachineCamera globeRollCamera;
    [Tooltip("Camera for the ending rolling through the hallway.")]
    [SerializeField] private CinemachineCamera globeHallwayCamera;

    //will likely change with later UI assets
    [Tooltip("UI for the button pressing minigame.")]
    [SerializeField] private TMP_Text buttonPressText;
    [Tooltip("Scene that contains the ending cutscene video.")]
    [SerializeField, Scene] private string endCutsceneScene = "Main Menu";

    [Tooltip("Canvas prefab that has the FadeToBlack script.")]
    [SerializeField] private GameObject fadeToWhite;


    [Header("Design Variables")]
    [Tooltip("Amount of button presses required for the ending minigame.")]
    public int endingButtonPresses = 10;
    [HideInInspector] public int currentButtonPresses = 0;

    private Coroutine endCoroutine;
    [HideInInspector] public bool EndingActive = false;
    [Foldout("Achievements"), SerializeField] bool hasAchievement;
    [Foldout("Achievements"), SerializeField] AchievementManager.eAchievements WhatAcheivement;


    private Animator animator => GetComponent<Animator>();
    private SceneTransitionInteractable sceneTransitionInteractable => GetComponent<SceneTransitionInteractable>();

    private void Start()
    {
        
    }
    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        IncreaseCameraPriority(globeSwingCamera, 1);
        if (hasAchievement)
        {
            AchievementManager.Instance.UnlockAchievement(WhatAcheivement);
        }

        if (endCoroutine == null)
        {
            sceneTransitionInteractable.enabled = false;
            EndingActive = true;
            endCoroutine = StartCoroutine(ButtonPressMinigame());
        }
    }

    public override void OnPossessionEnded()
    {
    }

    public override bool IsDetectable() { return false; }


    #region action
    public override void OnActionStarted()
    {
        if (EndingActive)
        {
            animator.SetBool("EndingStarted", true);
            currentButtonPresses++;
            endingButtonPresses--;
        }
    }

    public override void WhileActionHeld(float secondsHeld)
    {
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
    }

    public override void OnActionCanceled(float secondsHeld)
    {
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
    }

    public override void WhileInteractHeld(float secondsHeld)
    { }

    public override void OnInteractCanceled(float secondsHeld)
    {
    }
    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld(float secondsHeld)
    {
    }

    public override void WhileMoveNotHeld()
    {
    }
    public override void OnMoveCanceled(float secondsHeld) { }


    #endregion

    #region Other
    public IEnumerator ButtonPressMinigame()
    {
        buttonPressText.enabled = true;
        int buttonPresses = endingButtonPresses;

        while (EndingActive)
        {
            //UI update
            if (currentButtonPresses > 0 && currentButtonPresses < 11)
            {
                buttonPressText.text = currentButtonPresses.ToString() + " / " + buttonPresses.ToString();
            }

            if (endingButtonPresses <= 0)
            {
                //animation will be adjusted here later
                animator.SetBool("EndRoll", true);
                EndingActive = false;

                IncreaseCameraPriority(globeRollCamera, 1);
                DecreaseCameraPriority(globeSwingCamera);
            }
            yield return null;
        }
    }

    private void IncreaseCameraPriority(CinemachineCamera camera, int value)
    {
        if (camera.Priority == 0)
        {
            camera.Priority += value;
        }
    }

    private void DecreaseCameraPriority(CinemachineCamera camera)
    {
        if (camera.Priority != 0)
        {
            camera.Priority--;
        }
    }

    public void SwitchToHallwayCamera()
    {
        IncreaseCameraPriority(globeHallwayCamera, 2);
        DecreaseCameraPriority(globeRollCamera);
    }

    public void FadeToWhite()
    {
        FadeToBlack ftb = Instantiate(fadeToWhite).GetComponent<FadeToBlack>();
        ftb.Initialize(LoadEndScene);
    }

    public void LoadEndScene()
    {
        StopAllCoroutines();
        LevelManager.Instance.ChangeScene(endCutsceneScene);
    }
    #endregion
}

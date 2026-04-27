/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles inputs for the globe end scene
 */

using NaughtyAttributes;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    [Tooltip("UI image for the button pressing minigame.")]
    [SerializeField] private Image buttonPressUI;


    [Tooltip("Rect Transform for the button press UI.")]
    [SerializeField] private RectTransform buttonTransform;


    [Tooltip("Keyboard button sprite.")]
    [SerializeField] private Sprite keyboardButton;
    [Tooltip("Controller button sprite.")]
    [SerializeField] private Sprite controllerButton;

    //[SerializeField] private List<Sprite> explosionSprites;

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


    private Animator animator => GetComponent<Animator>();
    private SceneTransitionInteractable sceneTransitionInteractable => GetComponent<SceneTransitionInteractable>();
    private Coroutine buttonPressAnimation;

    private void Start()
    {
        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);
    }
    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        IncreaseCameraPriority(globeSwingCamera, 1);

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

            StaticUtilities.StopAndStartCoroutine(ref buttonPressAnimation, ExpandPressButton());
            SetFogBubbleAmount((float)currentButtonPresses / endingButtonPresses);
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
        buttonPressAnimation = 
            buttonPressAnimation
            .Then(ShrinkPressButton());
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
        buttonPressUI.enabled = true;

        while (EndingActive)
        {
            if (currentButtonPresses >= endingButtonPresses)
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

    protected void OnControllerChanged()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        bool controller = InputEvents.Instance.IsGamepadActive();

        if (controller)
        {
            buttonPressUI.sprite = controllerButton;
        }
        else
        {
            buttonPressUI.sprite = keyboardButton;
        }
    }


    private IEnumerator ExpandPressButton()
    {
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: Vector3.one, endScale: new Vector3(1.25f, 1.25f, 1), seconds: 0.1f);
    }
    private IEnumerator ShrinkPressButton()
    {
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: new Vector3(1.25f, 1.25f, 1), endScale: Vector3.one, seconds: 0.1f);
    }

    private IEnumerator PressButtonAnimation()
    {
        // expand
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: Vector3.one, endScale: new Vector3(1.25f, 1.25f, 1), seconds: 0.1f);
        // shrink
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: new Vector3(1.25f, 1.25f, 1), endScale: Vector3.one, seconds: 0.1f);
    }
    #endregion

    #region Fog Bubbles

    void SetFogBubbleAmount(float t)
    {
        Debug.Log(t);
    }

    #endregion
}

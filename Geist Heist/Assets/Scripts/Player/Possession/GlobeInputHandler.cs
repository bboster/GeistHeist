/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles inputs for the globe end scene
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobeInputHandler : IInputHandler
{
    [Header ("Required Variables")]
    [Tooltip("Camera for the ending scene.")]
    [SerializeField] private CinemachineCamera globeCamera;
    //will likely change with later UI assets
    [Tooltip("UI for the button pressing minigame.")]
    [SerializeField] private TMP_Text buttonPressText;
    [Tooltip("Scene that contains the ending cutscene video.")]
    [SerializeField, Scene] private string endCutsceneScene = "Main Menu";


    [Header("Design Variables")]
    [Tooltip("Amount of button presses required for the ending minigame.")]
    public int endingButtonPresses = 10;
    [HideInInspector] public int currentButtonPresses = 0;

    private Coroutine endCoroutine;
    [HideInInspector] public bool EndingActive = false;


    private Animator animator => GetComponent<Animator>();
    private SceneTransitionActionable sceneTransitionInteractable => GetComponent<SceneTransitionActionable>();

    private void Start()
    {
        
    }
    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        if (globeCamera.Priority == 0)
        {
            globeCamera.Priority++;
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


    #region action
    public override void OnActionStarted()
    {
        if (EndingActive)
        {
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
                animator.SetBool("EndingStarted", true);
                EndingActive = false;
            }

            yield return null;
        }
    }

    public void LoadEndScene()
    {
        StopAllCoroutines();
        LevelManager.Instance.ChangeScene(endCutsceneScene);
    }
    #endregion
}

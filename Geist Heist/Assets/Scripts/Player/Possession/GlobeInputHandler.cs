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
using FMODUnity;
using FMOD.Studio;

public class GlobeInputHandler : IInputHandler
{
    [Header ("Required Variables")]


    [Tooltip("Camera that watches the guards.")]
    [SerializeField] private CinemachineCamera toGuardCamera;
    [Tooltip("Camera for the ending swinging.")]
    [SerializeField] private CinemachineCamera globeSwingCamera;
    [Tooltip("Camera for the ending rolling before hallway.")]
    [SerializeField] private CinemachineCamera globeRollCamera;
    [Tooltip("Camera for the ending rolling through the hallway.")]
    [SerializeField] private CinemachineCamera globeHallwayCamera;
    [Tooltip("UI image for the button pressing minigame.")]
    [SerializeField] private Image buttonPressUI;
    [SerializeField, Required]
    private Canvas overlayCanvas;
    [Tooltip("Rect Transform for the button press UI.")]
    [SerializeField] private RectTransform buttonTransform;

    [Header("Button sprites")]
    [Tooltip("Keyboard button sprite.")]
    [SerializeField] private Sprite keyboardButton;
    [Tooltip("Controller button sprite.")]
    [SerializeField] private Sprite controllerButton;


    [Header("Scene Transition")]
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

    [Foldout("Explosions"), SerializeField] private GameObject explosionContainer;
    [Foldout("Explosions"), SerializeField] private Transform explosionParent;
    [Foldout("Explosions"), SerializeField] private List<Sprite> explosionSprites;

    #region Fubbles
    [Foldout("Fog Bubbles"), SerializeField] private int fubblesPerButtonPress =3 ;
    [Foldout("Fog Bubbles"), SerializeField] private float maxFogBubbleEmissionRate;
    [Foldout("Fog Bubbles"), SerializeField] private float minFogBubbleStartSpeed = 1;
    [Foldout("Fog Bubbles"), SerializeField] private float maxFogBubbleStartSpeed = 3.5f;
    [Foldout("Fog Bubbles"), SerializeField] private Camera LeftFogBubblesPrefab;
    [Foldout("Fog Bubbles"), SerializeField] private Camera RightFogBubblesPrefab;
    [Foldout("Fog Bubbles"), SerializeField] private RawImage leftFogBubbleOutputImage;
    [Foldout("Fog Bubbles"), SerializeField] private RawImage rightFogBubbleOutputImage;
    [Foldout("Fog Bubbles"), SerializeField] private Material leftFogBubbleMaterial;
    [Foldout("Fog Bubbles"), SerializeField] private Material rightFogBubbleMaterial;
    [Foldout("Fog Bubbles"), SerializeField] private RenderTexture leftFogRenderTexture;
    [Foldout("Fog Bubbles"), SerializeField] private RenderTexture rightFogRenderTexture;
    #endregion

    private Camera leftFogBubblesInstance, rightFogBubblesInstance;
    private ParticleSystem[] leftParticleSystems;
    private ParticleSystem[] rightParticleSystems;
    ParticleSystemForceField fogForceField;

    private Animator animator;
    private SceneTransitionInteractable sceneTransitionInteractable;
    private Coroutine buttonPressAnimation;
    private Coroutine buttonRotationAnimation;

    private EventInstance orchHit;

    private void Start()
    {
        animator = GetComponent<Animator>();
        sceneTransitionInteractable = GetComponent<SceneTransitionInteractable>();
        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerChanged);

        // YES, fog bubbles wont be needed most of the time,
        // BUT its an expensive operation that would cause a fps spike at the ending (not good!)
        InitializeFogBubbles();

        overlayCanvas.gameObject.SetActive(false);

        orchHit = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GlobeClick);

        leftFogBubblesInstance.gameObject.SetActive(false);
        rightFogBubblesInstance.gameObject.SetActive(false);
    }
    public override void WhilePossessingUpdate()
    {
    }

    public override void OnPossessionStart()
    {
        //start cinematic
        toGuardCamera.gameObject.SetActive(true);
        IncreaseCameraPriority(toGuardCamera, 1);
        EndingDoorController.Instance.DoorAnimator.SetBool("ENDSCENESTARTED", true);

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

        overlayCanvas.gameObject.SetActive(true);

        leftFogBubblesInstance.gameObject.SetActive(true);
        rightFogBubblesInstance.gameObject.SetActive(true);

        // hide the hud
        StaticUtilities.FadeToHidden(PlayerHUDManager.Instance.GetComponent<CanvasGroup>(), seconds: 0.5f);
    }

    public override void OnPossessionEnded()
    {
        Destroy(leftFogBubblesInstance.gameObject);
        Destroy(rightFogBubblesInstance.gameObject);
    }

    public override bool IsDetectable() { return false; }


    #region action
    public override void OnActionStarted()
    {
        if (EndingActive && toGuardCamera.Priority <= 0)
        {
            animator.SetBool("EndingStarted", true);
            currentButtonPresses++;
            orchHit.start();
            orchHit.setParameterByName("GlobeRamp", currentButtonPresses);

            StaticUtilities.StopAndStartCoroutine(ref buttonPressAnimation, ExpandPressButton());

            SetFogBubbleAmount((float)currentButtonPresses / endingButtonPresses);

            #region Explosions
            //new explosion, parent
            GameObject explosion = Instantiate(explosionContainer, Vector3.zero, Quaternion.identity, explosionParent);

            //position, behind button
            RectTransform rt = explosion.GetComponent<RectTransform>();

            float x = UnityEngine.Random.Range(-100, 100);
            float y = UnityEngine.Random.Range(-100, 100);

            rt.anchoredPosition = new Vector3(x, y, 0);
            explosion.transform.SetAsFirstSibling();

            //swap in sprite
            Image explosionImage = explosion.GetComponent<Image>();
            explosionImage.sprite = explosionSprites[UnityEngine.Random.Range(0, explosionSprites.Count - 1)];
            explosionImage.SetNativeSize();

            //animation
            StaticUtilities.AnimateScale(rt, Vector3.zero, Vector3.one, 0.2f);
            int rotation = UnityEngine.Random.Range(30, -30);
            StaticUtilities.AnimateRotation(rt, new Vector3(0, 0, rotation), 0.2f).Then(() => StaticUtilities.AnimateScale(rt, Vector3.zero, 0.15f));
            #endregion
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

        while (EndingActive)
        {
            if (toGuardCamera.Priority <= 0)
            {
                buttonPressUI.enabled = true;
            }

            if (currentButtonPresses >= endingButtonPresses)
            {
                //animation will be adjusted here later
                animator.SetBool("EndRoll", true);
                EndingActive = false;

                StaticUtilities.FadeToHidden(buttonPressUI.GetComponent<CanvasGroup>(), unscaledTime: true, seconds: 0.5f);

                globeRollCamera.gameObject.SetActive(true);
                IncreaseCameraPriority(globeRollCamera, 1);
                DecreaseCameraPriority(globeSwingCamera);
                globeSwingCamera.gameObject.SetActive(false);
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
        globeHallwayCamera.gameObject.SetActive(true);
        IncreaseCameraPriority(globeHallwayCamera, 2);
        DecreaseCameraPriority(globeRollCamera);
        globeRollCamera.gameObject.SetActive(false);
    }
    public void SwitchToSwingingCamera()
    {
        globeSwingCamera.gameObject.SetActive(true);
        IncreaseCameraPriority(globeSwingCamera, 2);
        DecreaseCameraPriority(toGuardCamera);
        toGuardCamera.gameObject.SetActive(false);
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
        float rotation = UnityEngine.Random.Range(-30f, 30f);

        // scale
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: Vector3.one, endScale: new Vector3(1.25f, 1.25f, 1), seconds: 0.1f)
            // rotate
            .And(StaticUtilities.AnimateRotation(buttonTransform, endRotation: new Vector3(0, 0, rotation), seconds: 0.1f));
    }
    private IEnumerator ShrinkPressButton()
    {
        // scale
        yield return StaticUtilities.AnimateScale(buttonTransform, startScale: new Vector3(1.25f, 1.25f, 1), endScale: Vector3.one, seconds: 0.1f)
            // rotate
            .And(StaticUtilities.AnimateRotation(buttonTransform, Quaternion.identity, seconds: 0.1f));
    }

    public void KnockOverGuards()
    {
        EndingGuardController.Instance.KnockOverGuards();
    }

    #endregion

    #region Fog Bubbles

    void InitializeFogBubbles(bool disableAfterInit = true)
    {
        leftFogBubblesInstance = Instantiate(LeftFogBubblesPrefab);
        rightFogBubblesInstance = Instantiate(RightFogBubblesPrefab);

        fogForceField = leftFogBubblesInstance.GetComponentInChildren<ParticleSystemForceField>();
        fogForceField.gameObject.SetActive(false);

        // initialize left render texture
        leftFogRenderTexture = new RenderTexture(3840, 2160, leftFogRenderTexture.depth, leftFogRenderTexture.format);
        leftFogRenderTexture.Create();
        leftFogBubblesInstance.targetTexture = leftFogRenderTexture;
        var leftFogMaterialCopy = Instantiate(leftFogBubbleMaterial);
        leftFogMaterialCopy.SetTexture("_Render_Texture", leftFogRenderTexture);
        leftFogBubbleOutputImage.material = leftFogMaterialCopy;

        // initialize right render texture
        rightFogRenderTexture = new RenderTexture(3840, 2160, rightFogRenderTexture.depth, rightFogRenderTexture.format);
        rightFogBubblesInstance.targetTexture = rightFogRenderTexture;
        var rightFogMaterialCopy = Instantiate(rightFogBubbleMaterial);
        rightFogMaterialCopy.SetTexture("_Render_Texture", rightFogRenderTexture);
        rightFogBubbleOutputImage.material = rightFogMaterialCopy;

        // initialize particles
        leftParticleSystems = leftFogBubblesInstance.GetComponentsInChildren<ParticleSystem>();
        rightParticleSystems = rightFogBubblesInstance.GetComponentsInChildren<ParticleSystem>();
        SetFogBubbleAmount(0);

        if (disableAfterInit)
        {
            //leftFogBubblesInstance.gameObject.SetActive(false);
            //rightFogBubblesInstance.gameObject.SetActive(false);
        }
    }

    void SetFogBubbleAmount(float t)
    {
        if(t>= 1)
        {
            fogForceField.gameObject.SetActive(true);
        }

        float speed = Mathf.Lerp(minFogBubbleStartSpeed, maxFogBubbleStartSpeed, t);
        float rate = t >= 1 ? 0 : Mathf.Lerp(0, maxFogBubbleEmissionRate, t);

        leftParticleSystems.ForEach(ps => {
            SetParticleSystem(ps, rate, speed);

            if(t > 0)
                ps.Emit(fubblesPerButtonPress);
            });
        rightParticleSystems.ForEach(ps => { 
            SetParticleSystem(ps, rate, speed);

            if (t > 0)
                ps.Emit(fubblesPerButtonPress);
        });
    }

    void SetParticleSystem(ParticleSystem ps, float emissionRate, float speed)
    {
        var emission = ps.emission;
        emission.rateOverTime = emissionRate;
        emission.enabled = emissionRate > 0;

        var main = ps.main;
        main.simulationSpeed = speed;
    }

    #endregion
}

/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 02/10/26
 * Last Edited: 02/10/26
 * Summary: Loads into hub scene after intro cutscene is finished playing
 * TO DO: Swap out the skip text message with controller support system
 */
using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroCutsceneController : MonoBehaviour
{
    private bool skippable = false;
    private VideoPlayer player;

    [SerializeField, Scene] private string hubScene;
    [SerializeField, Required] private GameObject loadingScreenPrefab;
    [SerializeField, Required] private GameObject loadingCardPrefab;
    [SerializeField] private TextMeshProUGUI skipText; //THIS NEEDS TO BE SWAPPED OUT WITH CONTROLLER ICONS
    [SerializeField] private float skipTextActiveTime;

    [SerializeField] private string ControllerText;
    [SerializeField] private string KeyboardText;

    [SerializeField] private RawImage outputImage;

    private RenderTexture renderTexture;
    private EventInstance introVl;

    private void Awake()
    {
        if(hubScene == null || hubScene.Length == 0)
        {
            Debug.LogError("No scene specified for transition on" + gameObject.name);
            return;
        }

        renderTexture = new RenderTexture(3840, 2160, 1, RenderTextureFormat.ARGB32);
        renderTexture.Create();

        player = GetComponent<VideoPlayer>();
        player.renderMode = VideoRenderMode.RenderTexture;
        player.targetTexture = renderTexture;


        outputImage.texture = renderTexture;

        player.Prepare();
        StartCoroutine(PrepareWait());

        player.loopPointReached += LoadHub;

        StaticUtilities.HideCursor();
    }

    private void Start()
    {
        InputEvents.PauseStarted.AddListener(SkipCutscene);
        InputEvents.InteractStarted.AddListener(SkipCutscene);

        InputEvents.Instance.OnControllerChanged.AddListener(OnControllerUpdated);
    }

    private IEnumerator PrepareWait()
    {
        while(!player.isPrepared)
        {
            yield return new WaitForEndOfFrame();
        }

        player.Play();
        introVl = RuntimeManager.CreateInstance(FMODEvents.Instance.VLIntro);
        introVl.start();
    }

    /// <summary>
    /// Loads the player into the hub using the specified loading screen and loading card
    /// </summary>
    /// <param name="player"></param>
    private void LoadHub()
    {
        if (loadingScreenPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            LevelManager.Instance.ChangeScene(hubScene);
            return;
        }
        var levelTransition = Instantiate(loadingScreenPrefab).GetComponent<LevelTransitionScreen>();
        levelTransition.StartTransition(hubScene, loadingCardPrefab);
    }

    /// <summary>
    /// Loads the player into the hub using the specified loading screen and loading card
    /// </summary>
    /// <param name="player"></param>
    private void LoadHub(VideoPlayer player)
    {
        AchievementManager.instance.UnlockAchievement(AchievementManager.eAchievements.FinishGame);

        if (loadingScreenPrefab == null)
        {
            Debug.LogError("No transition card set on " + gameObject.name);
            LevelManager.Instance.ChangeScene(hubScene);
            return;
        }

        InputEvents.PauseStarted.RemoveListener(SkipCutscene);
        InputEvents.InteractStarted.RemoveListener(SkipCutscene);

        InputEvents.Instance.OnControllerChanged.RemoveListener(OnControllerUpdated);

        var levelTransition = Instantiate(loadingScreenPrefab).GetComponent<LevelTransitionScreen>();
        levelTransition.StartTransition(hubScene, loadingCardPrefab);
    }

    /// <summary>
    /// Skips the cutscene
    /// </summary>
    private void SkipCutscene()
    {
        Debug.Log("skipping!");

        if (skippable)
            LoadHub();

        skippable = true;
        skipText.gameObject.SetActive(true);
        StartCoroutine(SkipTextTimer());
    }

    /// <summary>
    /// Controls how long the text allowing the cutscene to be skipped is visible for
    /// </summary>
    /// <returns></returns>
    private IEnumerator SkipTextTimer()
    {
        yield return new WaitForSeconds(skipTextActiveTime);

        skippable = false;
        skipText.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        player.loopPointReached -= LoadHub;

        introVl.stop(STOP_MODE.ALLOWFADEOUT);
    }

    private void OnControllerUpdated()
    {
        Debug.Log("controller updated "+ InputEvents.Instance.IsGamepadActive().ToString());
        skipText.text = InputEvents.Instance.IsGamepadActive() ? ControllerText : KeyboardText;
    }
}

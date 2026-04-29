/*
 * Contributors: Toby, Josh
 * Creation Date: 10/20/25
 * Last Modified: 4/29/2026
 * 
 * Brief Description: Resusable & modular UI popup for confirming the users choice.
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelConfirmationPopup : ConfirmationPopup
{
    [SerializeField, Required] private CanvasGroup loadingText;
    [SerializeField, Required] private Animator animator;
    [SerializeField, Required] private Transform parent;

    private bool loadingAnimationFinished = false;
    private bool closeSequenceStarted = false;
    private CanvasGroup parentCanvasGroup;

    public override void OpenConfirmationPopup(string text = null, UnityAction OnConfirmationButtonClicked = null, UnityAction OnCancelButtonClicked = null, float fadeSeconds = -1, bool closeMenuOnConfirm = true, bool freezeTime = true)
    {
        base.OpenConfirmationPopup(text, OnConfirmationButtonClicked, OnCancelButtonClicked, fadeSeconds, closeMenuOnConfirm, freezeTime);

        confirmButton?.onClick.RemoveAllListeners();
        confirmButton?.onClick.AddListener(OnConfirmButtonClicked);

        loadingText.alpha = 0;

        loadingAnimationFinished = false;
        closeSequenceStarted = false;
        parentCanvasGroup = parent.gameObject.GetOrAddComponent<CanvasGroup>();
        parentCanvasGroup.alpha = 1f;
        parentCanvasGroup.interactable = true;
        parentCanvasGroup.blocksRaycasts = true;

        DontDestroyOnLoad(parent.gameObject);
    }

    protected override void OnConfirmButtonClicked()
    {
        Time.timeScale = 1;

        animator.SetTrigger("Loading");

        cancelButton.enabled = false;
        confirmButton.interactable = false;
        confirmButton.SetColors(disabledColor: Color.white); // so the player cant tell i just disabled it lolz
        confirmButton.enabled = false;

        SceneManager.sceneLoaded -= OnSceneLoadedAfterConfirmation;
        SceneManager.sceneLoaded += OnSceneLoadedAfterConfirmation;
    }

    // Called from the animation clip that shows the "Loading..." text
    public void OnLoadingAnimationFinished()
    {
        // this loads the scene
        onConfirmationButtonClicked();

        Debug.Log("Level finished loading");
        loadingAnimationFinished = true;
        AnyConfirmationMenuOpen = false;
    }

    IEnumerator CloseLevelConfirmation()
    {
        Debug.Log("closing level confirmation screen");
        yield return new WaitForSecondsRealtime(2);

        // wait for little loading animation to finish
        while (!loadingAnimationFinished)
            yield return null;

        if (parentCanvasGroup == null && parent != null)
            parentCanvasGroup = parent.GetComponent<CanvasGroup>();

        CanvasGroup groupToFade = parentCanvasGroup != null ? parentCanvasGroup : canvasGroup;
        yield return StaticUtilities.FadeToHidden(groupToFade, lastFadeSecondsUsed);

        if (parentCanvasGroup != null)
        {
            parentCanvasGroup.interactable = false;
            parentCanvasGroup.blocksRaycasts = false;
        }

        HideConfirmationPopup();
    }

    private void OnSceneLoadedAfterConfirmation(Scene _, LoadSceneMode __)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedAfterConfirmation;

        if (this == null || closeSequenceStarted)
            return;

        closeSequenceStarted = true;
        StartCoroutine(CloseLevelConfirmation());
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoadedAfterConfirmation;
    }

    protected override void AfterFadeToHidden()
    {
        AnyConfirmationMenuOpen = false;
        Destroy(parent.gameObject);
    }
}

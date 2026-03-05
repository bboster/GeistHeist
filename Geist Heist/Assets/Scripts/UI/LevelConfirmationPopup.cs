/*
 * Contributors: Toby
 * Creation Date: 10/20/25
 * Last Modified: 11/5/25
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

    public override void OpenConfirmationPopup(string text = null, UnityAction OnConfirmationButtonClicked = null, UnityAction OnCancelButtonClicked = null, float fadeSeconds = -1, bool closeMenuOnConfirm = true, bool freezeTime = true)
    {
        base.OpenConfirmationPopup(text, OnConfirmationButtonClicked, OnCancelButtonClicked, fadeSeconds, closeMenuOnConfirm, freezeTime);

        confirmButton?.onClick.RemoveAllListeners();
        confirmButton?.onClick.AddListener(OnConfirmButtonClicked);

        loadingText.alpha = 0;

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

        SceneManager.sceneLoaded += (_,_) => { StartCoroutine(CloseLevelConfirmation()); };
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

        yield return StaticUtilities.FadeToHidden(canvasGroup, lastFadeSecondsUsed);

        HideConfirmationPopup();
    }

    protected override void AfterFadeToHidden()
    {
        AnyConfirmationMenuOpen = false;
        Destroy(parent.gameObject);
    }
}

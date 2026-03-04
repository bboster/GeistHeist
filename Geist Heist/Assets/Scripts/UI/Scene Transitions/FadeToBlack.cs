/*
 * Contributors: Sky
 * Creation Date: 2/26/26
 * Last Modified: 2/26/26
 * 
 * Brief Description: Reference for a fade to black, calls the coroutine from the static utilities class
 */
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class FadeToBlack : Singleton<FadeToBlack>
{
    private Coroutine fadeCoroutine;
    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float secondsForFade = 1;
    public void Initialize(UnityAction action)
    {
        DontDestroyOnLoad(gameObject);

        if (fadeCoroutine == null)
        {
            fadeCanvasGroup.alpha = 0;
            fadeCoroutine = StartCoroutine(FadeAnimation(action));
        }
    }

    private IEnumerator FadeAnimation(UnityAction action)
    {
        yield return StaticUtilities.FadeToVisible(fadeCanvasGroup, secondsForFade);
        action();
        yield return StaticUtilities.FadeToHidden(fadeCanvasGroup, secondsForFade);
        Destroy(gameObject);
        fadeCoroutine = null;
    }


}

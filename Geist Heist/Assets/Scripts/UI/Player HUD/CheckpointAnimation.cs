/*
 * Contributors: Toby
 * Creation Date: 11/13/2025
 * Last Modified: 11/13/2025
 * 
 * Brief Description: Simple animation that appears when the player hits a checkpoint
 */

using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class CheckpointAnimation : MonoBehaviour
{
    [SerializeField] private float fadeSeconds = 0.3f;
    [SerializeField] private float persistSeconds = 1.75f;
    [SerializeField] private float secondsBetweenDots = 0.5f;
    [SerializeField] private string baseSavingText = "Checkpoint reached";

    [SerializeField, Required] private CanvasGroup savingGroup;
    [SerializeField, Required] private TMP_Text savingText;

    private Coroutine checkpointAnimationCoroutine;
    private Coroutine ellipsesAnimationCoroutine;
    private int currentEllipses = 1;

    private const int MAX_ELLIPSES = 3; // ...
    private const int MIN_ELLIPSES = 1; // .

    private void Start()
    {
        savingGroup.alpha = 0;
    }

    public void OpenCheckpointAnimation()
    {
        Debug.Log("animation");
        StaticUtilities.StopAndStartCoroutine(ref checkpointAnimationCoroutine, CheckpointFadeAnimation());
        StaticUtilities.StopAndStartCoroutine(ref ellipsesAnimationCoroutine, EllipsesAnimation());
    }

    private IEnumerator CheckpointFadeAnimation()
    {
        float t= savingGroup.alpha; // start with current alpha for edge case where player goes to two checkpoints really quickly
        while (t < 1)
        {
            // because alpha is also 0->1, no lerp is needed 
            t += Time.unscaledDeltaTime / fadeSeconds;

            // avoids looking weird if jumping between checkpoints
            if(t> savingGroup.alpha)
                savingGroup.alpha = t;

            yield return null;
        }

        // wait a sec
        savingGroup.alpha = 1;
        yield return new WaitForSecondsRealtime(persistSeconds);

        float timeStarted = Time.unscaledTime;
        t = 0;
        while (t < 1)
        {
            // different t calculation lol
            t = (Time.unscaledTime - timeStarted) / fadeSeconds;
            savingGroup.alpha = 1-t;

            yield return null;
        }
        savingGroup.alpha = 0;
    }

    private IEnumerator EllipsesAnimation()
    {
        do
        {
            // increase ellipses amount, loop around
            currentEllipses++;
            if (currentEllipses > MAX_ELLIPSES) 
                currentEllipses = MIN_ELLIPSES;

            string ellipses = "";
            for (int _ = 0; _ < currentEllipses; _++)
                ellipses += ".";

            savingText.text = baseSavingText + ellipses;
            Debug.Log(baseSavingText + ellipses);
            Debug.Log(currentEllipses);

            yield return new WaitForSecondsRealtime(secondsBetweenDots);
        }
        //while (checkpointAnimationCoroutine != null);
        while (savingGroup.alpha > 0);




    }
}

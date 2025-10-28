/*
 * Contributors: Toby
 * Creation Date: 10/27/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Brief animation that plays between levels
 * Animation should be handled with an animation component childed to this
 */

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class LevelTransitionCard : MonoBehaviour
{
    [SerializeField] private float fadeInSeconds = 0.5f;
    [SerializeField] private float waitingSeconds = 4;
    [SerializeField] private float fadeOutSeconds = 0.5f;

    [SerializeField, Required] CanvasGroup group;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartTransition(int sceneToLoad)
    {
        DontDestroyOnLoad(this);
        StartCoroutine(TitleCardAnimation(sceneToLoad));
    }

    private IEnumerator TitleCardAnimation(int sceneToLoad)
    {
        yield return FadeIn();

        GameManager.Instance.NextLevel(sceneToLoad);

        yield return new WaitForSeconds(fadeInSeconds);

        yield return FadeOut();

        Destroy(this.gameObject);
    }

    private IEnumerator FadeIn()
    {
        float startTime = Time.time;
        float time;
        do
        {
            time = Time.time - startTime;
            float t = time / fadeInSeconds;

            group.alpha = t;

            yield return null;
        }
        while (time < fadeInSeconds);
    }

    private IEnumerator FadeOut()
    {
        float startTime = Time.time;
        float time;
        do
        {
            time = Time.time - startTime;
            float t = time / fadeInSeconds;

            group.alpha =  1- t;

            yield return null;
        }
        while (time < fadeInSeconds);
    }
}

/*
 * Contributors: Toby
 * Creation Date: 10/27/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Brief animation that plays between levels. 
 * The manager that handles animations is called the SCREEN.
 * Animation CARD should be handled with an animation component childed to this
 */

using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransitionScreen : MonoBehaviour
{
    [SerializeField] private float fadeInSeconds = 0.5f;
    [SerializeField] private float waitingSeconds = 4;
    [SerializeField] private float fadeOutSeconds = 0.5f;

    /*[SerializeField, Required]*/ CanvasGroup group;
    private string _sceneToLoad;
    private GameObject animationObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartTransition(string sceneToLoad, GameObject animationPrefab)
    {
        if(group == null)
            group = GetComponent<CanvasGroup>();

        DontDestroyOnLoad(this);
        _sceneToLoad = sceneToLoad;
        if (animationPrefab != null)
            animationObject = Instantiate(animationPrefab, this.transform);
        else
            Debug.LogError("No level transition card set");

        StartCoroutine(TitleCardFadeAnimation());
    }

    private IEnumerator TitleCardFadeAnimation()
    {
        // Note that the actual animation will most likely be handled in an animation controller

        Debug.Log("Playing card fade animation. Press any key to skip");

        yield return FadeIn();

        GameManager.Instance.NextLevel(_sceneToLoad);

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

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
            SceneManager.LoadScene(_sceneToLoad);
            Destroy(this.gameObject);
        }
    }
#endif
}

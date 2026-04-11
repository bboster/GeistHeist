/*
 * Contributors: Toby
 * Creation Date: 10/27/25
 * Last Modified: 10/28/25
 * 
 * Brief Description: Brief animation that plays between levels. 
 * The manager that handles animations is called the SCREEN.
 * Animation CARD should be handled with an animation component childed to this
 */

using FMODUnity;
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

    private Coroutine fadeCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    /*public void StartTransition(string sceneToLoad, GameObject animationPrefab)
    {
        StartTransition(SceneManager.GetSceneByName(sceneToLoad).buildIndex, animationPrefab);
    }*/

    public void StartTransition(string sceneToLoad, GameObject animationPrefab)
    {
        Debug.Log("starting transition to " + sceneToLoad);
        if (group == null)
            group = GetComponent<CanvasGroup>();

        _sceneToLoad = sceneToLoad;
        if (animationPrefab == null)
            Debug.LogError("No level transition card set");
        else
            Instantiate(animationPrefab, this.transform);

        SceneManager.sceneLoaded += OnSceneLoaded;

        DontDestroyOnLoad(this);
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        group.alpha = 0;
        yield return StaticUtilities.FadeOpacity(group, 0, 1, fadeInSeconds);

        RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
        SceneManager.LoadScene(_sceneToLoad);
    }

    void OnSceneLoaded(Scene s, LoadSceneMode lsm)
    {
        // this happens sometimes
        if (this == null)
            return;

        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSecondsRealtime(waitingSeconds);

        yield return StaticUtilities.FadeToHidden(group, fadeOutSeconds);

        Destroy(this.gameObject);
    }
}

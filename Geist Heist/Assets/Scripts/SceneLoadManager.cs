/*
 * Contributors:  Jacob
 * Creation Date: 4/7/2026
 * Last Modified: 4/7/2026
 * 
 * Brief Description: Executes a sequence to properly unload and load scenes
 */

using FMOD.Studio;
using FMODUnity;
using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : DontDestroyOnLoadSingleton<SceneLoadManager>
{
    public static Action OnLoadStarted;
    public static Action OnLoadCompleted;

    [ReadOnly] public bool PlayCreditsQueued;

    public void LoadScene(string sceneToLoad)
    {
        StartCoroutine(LoadSceneSequence(sceneToLoad));
    }

    public void LoadScene(int sceneToLoad)
    {
        StartCoroutine(LoadSceneSequence(sceneToLoad));
    }

    /// <summary>
    /// Unloads and loads a scene with a proper sequence
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneSequence(string sceneToLoad)
    {
        OnLoadStarted?.Invoke();

        RuntimeManager.GetBus("Bus:/").stopAllEvents(STOP_MODE.ALLOWFADEOUT); //This should stop all sounds on the master bus from playing
        
        GC.Collect(); //Garbage collection

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        GC.Collect();

        SceneManager.LoadScene(sceneToLoad);

        OnLoadCompleted?.Invoke();

        SettingsProfile.ReadSavedSettings();

        yield return true;
    }

    /// <summary>
    /// Unloads and loads a scene with a proper sequence
    /// </summary>
    /// <param name="sceneToLoad"></param>
    /// <returns></returns>
    private IEnumerator LoadSceneSequence(int sceneToLoad)
    {
        OnLoadStarted?.Invoke();

        RuntimeManager.GetBus("Bus:/").stopAllEvents(STOP_MODE.ALLOWFADEOUT); //This should stop all sounds on the master bus from playing

        GC.Collect(); //Garbage collection

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        GC.Collect();

        SceneManager.LoadScene(sceneToLoad);

        OnLoadCompleted?.Invoke();

        yield return true;
    }
}

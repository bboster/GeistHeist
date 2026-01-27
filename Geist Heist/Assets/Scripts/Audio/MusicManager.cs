
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : Singleton<MusicManager>
{
    private EventInstance levelBGM;
    private EventInstance hubBGM;
    private EventInstance globeBGM;
    private EventInstance menuBGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.MenuBGM);
        globeBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.GlobeBGM);
        hubBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.HubBGM);
        levelBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.instance.LevelBGM);

        StopAll();

        /*
         * Hard-coding the scene names in the code is really scuffed tbh.
         * Could we make variables for each of the scene names or something?
         * -Toby
         */

        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Main Menu")))
        {
            menuBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Actual Hub Scene")))
        {
            hubBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Globe")))
        {
            globeBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("FINAL Parlour Room")))
        {
            levelBGM.start();
        }
    }

    void OnDestroy()
    {
        StopAll();
    }

    void StopAll()
    {
        levelBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        hubBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        globeBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

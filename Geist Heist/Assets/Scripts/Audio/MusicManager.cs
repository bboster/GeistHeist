
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : MonoBehaviour
{
    private EventInstance levelBGM;
    private EventInstance hubBGM;
    private EventInstance globeBGM;
    private EventInstance menuBGM;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuBGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.MenuBGM);
        globeBGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.GlobeBGM);
        hubBGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.HubBGM);
        levelBGM = AudioManager.instance.CreateEventInstance(FMODEvents.instance.LevelBGM);

        StopAll();
        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Main Menu")))
        {
            menuBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("HubGreybox")))
        {
            hubBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Globe")))
        {
            globeBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName("Level 1 Art Exhibit")))
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

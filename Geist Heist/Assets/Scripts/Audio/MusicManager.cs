
using UnityEngine;
using UnityEngine.SceneManagement;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField, NaughtyAttributes.Scene] private string hubName;
    [SerializeField] private string[] levelNames;
    [SerializeField, NaughtyAttributes.Scene] private string globeName;
    [SerializeField, NaughtyAttributes.Scene] private string menuName;
    
    private EventInstance levelBGM;
    private EventInstance hubBGM;
    private EventInstance globeBGM;
    private EventInstance menuBGM;

    protected override void Awake()
    {
        //SceneManager.sceneLoaded += StartMusic;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {

        globeBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GlobeBGM);
        hubBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.HubBGM);
        levelBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.LevelBGM);



        //the following if-else block could be changed to a Switch statement -Josh
        StopAll();
    }

    private void Start()
    {
        Debug.Log(hubName);
        StartMusic(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    private void StartMusic(Scene s, LoadSceneMode m)
    {
        Debug.Log(hubName);


        if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(menuName)))
        {
            menuBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.MenuBGM);
            menuBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(hubName)))
        {
            hubBGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(globeName)))
        {
            globeBGM.start();
        }
        else
        {
            levelBGM.start();
        }
    }

    void OnDestroy()
    {
        StopAll();

        SceneManager.sceneLoaded -= StartMusic;
    }

    void StopAll()
    {
        levelBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        hubBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        globeBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

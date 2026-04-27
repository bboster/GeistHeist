
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using FMODUnity;
using FMOD.Studio;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField, NaughtyAttributes.Scene] private string hubName;
    [SerializeField, NaughtyAttributes.Scene] private string tutorialName;
    [SerializeField, NaughtyAttributes.Scene] private string wing1Name;
    [SerializeField, NaughtyAttributes.Scene] private string wing2Name;
    [SerializeField, NaughtyAttributes.Scene] private string wing3Name;
    [SerializeField, NaughtyAttributes.Scene] private string wing4Name;
    [SerializeField, NaughtyAttributes.Scene] private string wing5Name;
    [SerializeField, NaughtyAttributes.Scene] private string globeName;
    [SerializeField, NaughtyAttributes.Scene] private string menuName;
    
    //private EventInstance levelBGM;
    private EventInstance hubBGM;
    private EventInstance globeBGM;
    private EventInstance menuBGM;
    //private EventInstance tutorialBGM;
    private EventInstance wing1BGM;
    private EventInstance wing2BGM;
    private EventInstance wing3BGM;

    protected override void Awake()
    {
        base.Awake();
        //SceneManager.sceneLoaded += StartMusic;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Initialize()
    {

        globeBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.GlobeBGM);
        hubBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.HubBGM);
        //tutorialBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.TutorialBGM);
        //levelBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.LevelBGM);
        menuBGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.MenuBGM);
        wing1BGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.Wing1BGM);
        wing2BGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.Wing2BGM);
        wing3BGM = AudioManager.Instance.CreateEventInstance(FMODEvents.Instance.Wing3BGM);


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
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(wing1Name)))
        {
            wing1BGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(wing2Name)))
        {
            wing1BGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(wing3Name)))
        {
            wing2BGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(wing4Name)))
        {
            wing2BGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(wing5Name)))
        {
            wing3BGM.start();
        }
        else if (SceneManager.GetActiveScene().Equals(SceneManager.GetSceneByName(tutorialName)))
        {
            //tutorialBGM.start();
        }
    }

    public void MusicSwitch(bool hiding, float val)
    {
        if (hiding)
        {
            StartCoroutine(MusicToHidden(val));
        }
        else
        {
            StartCoroutine(MusicToNormal(val));
        }
    }

    public IEnumerator MusicToHidden(float val)
    {
        val += 0.05f;
        PlayerManager.Instance.UpdateMusicSwitch(val);
        hubBGM.setParameterByName("Hiding", val);
        //tutorialBGM.setParameterByName("Hiding", val);
        wing1BGM.setParameterByName("Hiding", val);
        wing2BGM.setParameterByName("Hiding", val);
        wing3BGM.setParameterByName("Hiding", val);
        yield return new WaitForSecondsRealtime(0.03f);
        if (val < 1)
        {
            StartCoroutine(MusicToHidden(val));
        }
    }

    public IEnumerator MusicToNormal(float val)
    {
        val -= 0.05f;
        PlayerManager.Instance.UpdateMusicSwitch(val);
        hubBGM.setParameterByName("Hiding", val);
        //tutorialBGM.setParameterByName("Hiding", val);
        wing1BGM.setParameterByName("Hiding", val);
        wing2BGM.setParameterByName("Hiding", val);
        wing3BGM.setParameterByName("Hiding", val);
        yield return new WaitForSecondsRealtime(0.03f);
        if (val > 0)
        {
            StartCoroutine(MusicToNormal(val));
        }
    }

    void OnDestroy()
    {
        StopAll();

        SceneManager.sceneLoaded -= StartMusic;
    }

    void StopAll()
    {
        //levelBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        hubBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        //tutorialBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        globeBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuBGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        wing1BGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        wing2BGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        wing3BGM.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}

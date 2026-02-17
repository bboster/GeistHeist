/*
 * Contributors: Joe C, Toby
 * Creation Date: ?
 * Last Modified: 11/15/2025
 * 
 * Brief Description: 
 */

using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : Singleton<AudioManager>
{
    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;
    private Bus vocalsBus;
    
    private float getPausedVolumeMultiplier => GameManager.Instance == null ? 1 :       // timescale is 1 if no GameManager (this happens in main menu)
                                   (GameManager.Instance.IsPaused ? 0 : 1); // actual calculation if gamemanger is in scene

    //Sets AudioManager Instance in the scene
    protected void Start()
    {
        masterBus = RuntimeManager.GetBus("bus:/");
        validateBus(masterBus);
        musicBus = RuntimeManager.GetBus("bus:/Music");
        validateBus(musicBus);
        sfxBus = RuntimeManager.GetBus("bus:/SoundEffects");
        validateBus(sfxBus);
        vocalsBus = RuntimeManager.GetBus("bus:/Vocals");
        validateBus(vocalsBus);
    }

    public void Initialize()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.OnPauseChanged.AddListener(UpdateAllVolumes);

        UpdateAllVolumes();
    }

    #region Volume Update Handling

    public void UpdateAllVolumes()
    {
        UpdateMasterVolume();
        UpdateMusicVolume();
        UpdateSFXVolume();
        UpdateVocalsVolume();
    }
    public void UpdateMasterVolume()
    {
        masterBus.setVolume(SettingsProfile.MasterVolumeScaled * getPausedVolumeMultiplier);
    }

    public void UpdateMusicVolume()
    {
        musicBus.setVolume(SettingsProfile.MusicVolumeScaled * getPausedVolumeMultiplier);
    }
    public void UpdateSFXVolume()
    {
        sfxBus.setVolume(SettingsProfile.SFXVolumeScaled * getPausedVolumeMultiplier);
    }
    public void UpdateVocalsVolume()
    {
        vocalsBus.setVolume(SettingsProfile.VocalsVolumeScaled * getPausedVolumeMultiplier);
    }

    #endregion

    //Plays a non-looping event WITHOUT 3d Attributes
    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    //Plays a non-looping event WITH 3d Attributes (has a spacializer)
    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        return eventInstance;
    }

    //Sets EventInstance e's 3d attributes to a gameObject's transform + rigidbody
    //Should be called in e's update function
    public static void SetEventParameters(ref EventInstance e, Transform t, Rigidbody r)
    {
        RuntimeManager.AttachInstanceToGameObject(e, t.gameObject, r);
    }

    //Starts a looping sound effect from an ALREADY EXISTING INSTANCE
    //Music will have it's own script and functions for transitions
    //Not needed? Just play start function in each script
    /*public void StartLoopingSFX(EventInstance sound)
    {
        sound.start();
    }*/

    //Stops a looping sound effect
    //When fadeOut is true, allow for sound to finish
    //When fadeOut is false, stop sound immediately
    public void StopSFX(EventInstance sound, bool fadeOut)
    {
        if (fadeOut)
        {
            sound.stop(STOP_MODE.ALLOWFADEOUT);
        }
        else
        {
            sound.stop(STOP_MODE.IMMEDIATE);
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null) GameManager.Instance.OnPauseChanged.RemoveListener(UpdateAllVolumes);

        //TODO
        //ADD A FADE EFFECT ON EVERY SOUND TO MAKE IT FADE OUT OVER A HALF SECOND INSTEAD OF CUTTING THE SHORT
        //UNLESS MUSIC HAS SPECIAL TRANSITIONS BETWEEN SCENES, THEY SHOULD FOLLOW THE SAME RULE AS ABOVE
    }

    private void validateBus(Bus bus)
    {
        if (!bus.isValid())
        {
            Debug.LogError("Bus is not valid");
        }
    }
}

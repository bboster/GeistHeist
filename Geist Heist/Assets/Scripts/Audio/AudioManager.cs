using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : Singleton<AudioManager>
{
    public static AudioManager instance { get; private set; }

    //Sets AudioManager instance in the scene
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance != null)
        {
            Debug.Log("There is more than one AudioManager in the scene");
        }
        instance = this;
    }

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
    public void SetEventParameters(EventInstance e, Transform t, Rigidbody r)
    {
        e.set3DAttributes(RuntimeUtils.To3DAttributes(t, r));
    }

    private void OnDestroy()
    {
        //TODO
        //ADD A FADE EFFECT ON EVERY SOUND TO MAKE IT FADE OUT OVER A HALF SECOND INSTEAD OF CUTTING THE SHORT
        //UNLESS MUSIC HAS SPECIAL TRANSITIONS BETWEEN SCENES, THEY SHOULD FOLLOW THE SAME RULE AS ABOVE
    }
}

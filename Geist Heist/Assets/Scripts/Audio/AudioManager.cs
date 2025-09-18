using FMODUnity;
using UnityEngine;
using FMOD.Studio;

public class AudioManager : Singleton<AudioManager>
{
    public static AudioManager instance { get; private set; }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        if (instance != null)
        {
            Debug.Log("There is more than one AudioManager in the scene");
        }
        instance = this;
    }

    public void PlayOneShot(EventReference sound)
    {
        RuntimeManager.PlayOneShot(sound);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        //eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(gameObject.GetComponent<Transform>(), gameObject.GetComponent<Rigidbody>()));
        return eventInstance;
    }

    public void SetEventParameters(Transform t, Rigidbody r)
    {

    }

    private void OnDestroy()
    {
        //TODO
        //ADD A FADE EFFECT ON EVERY SOUND TO MAKE IT FADE OUT OVER A HALF SECOND INSTEAD OF CUTTING THE SHORT
        //UNLESS MUSIC HAS SPECIAL TRANSITIONS BETWEEN SCENES, THEY SHOULD FOLLOW THE SAME RULE AS ABOVE
    }
}

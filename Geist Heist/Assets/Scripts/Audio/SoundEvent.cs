using FMODUnity;

[System.Serializable]
public class SoundEvent
{
    public string key;

    public EventReference soundEvent;

    public string subtitle;

    public override string ToString()
    {
        return "KEY: " + key + " EVENT: ";
    }

    public EventReference GetReference() {
        return soundEvent;
    }
}
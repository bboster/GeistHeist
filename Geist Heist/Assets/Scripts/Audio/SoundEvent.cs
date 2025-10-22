using FMODUnity;

[System.Serializable]
public class SoundEvent
{
    public string key;

    public EventReference soundEvent;

    public override string ToString()
    {
        return "KEY: " + key + " EVENT: ";
    }
}
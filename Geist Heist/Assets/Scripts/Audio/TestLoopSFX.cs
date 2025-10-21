using FMOD.Studio;
using UnityEngine;

public class TestLoopSFX : MonoBehaviour
{
    private EventInstance TestSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        TestSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.TestA);
        //Function below just DOES NOT work right now?
        AudioManager.SetEventParameters(ref TestSFX, this.GetComponent<Transform>(), this.GetComponent<Rigidbody>());
        TestSFX.start();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        //AudioManager.SetEventParameters(TestSFX, transform, rigidbody);
    }
}

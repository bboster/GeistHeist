using FMOD.Studio;
using UnityEngine;

public class TestLoopSFX : MonoBehaviour
{
    EventInstance TestSFX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        TestSFX = AudioManager.instance.CreateEventInstance(FMODEvents.instance.EnemyBank.Find(x => x.Equals("TestA")).GetReference());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TestSFX.start();
        //AudioManager.SetEventParameters(TestSFX, transform, rigidbody);
    }
}

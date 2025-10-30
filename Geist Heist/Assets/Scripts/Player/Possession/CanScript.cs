using System.Collections;
using UnityEngine;

public class CanScript : MonoBehaviour
{
    bool firstTime = true;

    [SerializeField] private GameObject soundStimulus;

    private void OnCollisionEnter(Collision collision)
    {
        if (firstTime)
        {
            Instantiate(soundStimulus, transform.position, Quaternion.identity);
            Debug.Log("Stimulus");
            firstTime = false;
        }
    }
}

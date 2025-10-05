using UnityEngine;

public class CanScript : MonoBehaviour
{
    bool firstTime = true;
    private void OnCollisionEnter(Collision collision)
    {
        if (firstTime)
        {
            //JACOB ADD STIMULUS HERE PLEASE PLEASE PLEASE
            Debug.Log("Stimulus");
            firstTime = false;
        }
    }
}

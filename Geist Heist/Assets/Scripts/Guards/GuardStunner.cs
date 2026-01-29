using UnityEngine;
using GuardUtilities;

public class GuardStunner : MonoBehaviour
{
    [Tooltip("How much velocity this object must have before it can stun a guard")]
    [SerializeField] private float stunVelocityThreshold;
    [SerializeField] private bool isCar;
    bool canstun = true;

    /// <summary>
    /// Triggers stunned state on guard when the object collides with the guard.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out GuardController gc) && canstun)
        {
            Vector3 impactVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;

            if (impactVelocity.magnitude > stunVelocityThreshold)
            {
                gc.ChangeBehavior(GuardStates.concussed);
                if (isCar)
                {
                    AudioManager.Instance.PlayOneShot(FMODEvents.instance.CarBump, collision.transform.position);
                }
            }
        }
        else
        {
            if (!isCar)
            {
                canstun = false;
            }
        }
    }
}

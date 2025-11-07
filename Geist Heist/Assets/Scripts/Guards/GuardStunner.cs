using UnityEngine;
using GuardUtilities;

public class GuardStunner : MonoBehaviour
{
    [Tooltip("How much velocity this object must have before it can stun a guard")]
    [SerializeField] private float stunVelocityThreshold;

    /// <summary>
    /// Triggers stunned state on guard when the object collides with the guard.
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.TryGetComponent(out GuardController gc))
        {
            Vector3 impactVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;

            if(impactVelocity.magnitude > stunVelocityThreshold)
                gc.ChangeBehavior(GuardStates.concussed);
        }
    }
}

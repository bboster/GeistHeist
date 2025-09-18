using UnityEngine;

public class VisionCone : MonoBehaviour
{
    [Tooltip("The index of the behavior to activate when the player is seen. WILL REPLACE WITH BETTER SYSTEM WHEN I THINK OF ONE")]
    [SerializeField] private int behaviorIndex;
    [Tooltip("The index of the behavior to activate when the enemy loses track of the player during a chase.")]
    [SerializeField] private int recoveryBehaviorIndex;

    private void OnTriggerEnter(Collider other)
    {
        //Detect when player enters trigger.

        //Trigger ChangeBehavior
    }

    private void OnTriggerExit(Collider other)
    {
        //Detect when player leaves trigger.

        //Trigger ChangeBehavior to return to base behavior. (Will swap to a recovery behavior once that is programmed)
    }
}

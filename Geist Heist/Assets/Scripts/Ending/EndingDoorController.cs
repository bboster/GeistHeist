using UnityEngine;
/*
 * Contributors: Sky
 * Creation Date: 4/30/26
 * Last Modified: 4/30/26
 * 
 * Brief Description: Ending controller for the door (used for animation events)
 */
public class EndingDoorController : Singleton<EndingDoorController>
{
    [Tooltip("Animator on the guard container.")]
    public Animator GuardAnimator;
    [HideInInspector] public Animator DoorAnimator;

    private void Start()
    {
        DoorAnimator = GetComponent<Animator>();
    }

    //Guards go out of door
    public void ActivateGuards()
    {
        GuardAnimator.SetBool("Looping", true);
    }
}

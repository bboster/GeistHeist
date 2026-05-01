using System.Collections.Generic;
using UnityEngine;
/*
 * Contributors: Sky
 * Creation Date: 4/30/26
 * Last Modified: 4/30/26
 * 
 * Brief Description: Ending controller for the guards (used for animation events)
 */
public class EndingGuardController : Singleton<EndingGuardController>
{
    [HideInInspector] public Animator GuardAnimator;

    [Tooltip("Animators on each individual guard.")]
    [SerializeField] private List<Animator> guardAnimators;


    [Tooltip("Globe - GlobeInputHandler.")]
    [SerializeField] private GlobeInputHandler globeIH;

    private void Start()
    {
        GuardAnimator = GetComponent<Animator>();
    }

    //wait to get hit
    public void GuardWait()
    {
        foreach (Animator guard in guardAnimators)
        {
            guard.SetBool("Looping", true);
        }
    }

    //transition cam to minigame area
    public void CamBackToHub()
    {
        globeIH.SwitchToSwingingCamera();
    }

    //knock over the guards
    public void KnockOverGuards()
    {
        GuardAnimator.SetBool("Death", true);

        foreach (Animator guard in guardAnimators)
        {
            guard.SetBool("Death", true);
        }
    }
}

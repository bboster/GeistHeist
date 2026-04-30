using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class EndingAnimationController : MonoBehaviour
{
    public Animator GuardAnimator;
    public Animator DoorAnimator;

    [SerializeField] private List<Animator> guardAnimators;

    public void ActivateGuards()
    {
        GuardAnimator.SetBool("Looping", true);
    }

    public void GuardWait()
    {
        foreach (Animator guard in guardAnimators)
        {
            guard.SetBool("Looping", true);
        }
    }
}

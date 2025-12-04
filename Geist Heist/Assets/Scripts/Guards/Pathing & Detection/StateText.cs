/*
 * Author: Jacob Bateman, Toby
 * Contributors:
 * Creation: 10/04/25
 * Last Edited: 10/16/25
 * Summary: Changes text for the temporary state visualizer.
 * Is billboarded to always face the player
 */

using GuardUtilities;
using UnityEngine;
using UnityEngine.UI;

public class StateText : IBillboardUI
{
    [SerializeField] private Image stateImg;

    [SerializeField] private Sprite idleImg;
    [SerializeField] private Sprite patrolImg;
    [SerializeField] private Sprite surprisedImg;
    [SerializeField] private Sprite chaseImg;
    [SerializeField] private Sprite attackImg;
    [SerializeField] private Sprite stunnedImg;
    [SerializeField] private Sprite searchImg;
    [SerializeField] private Sprite visionBreakImg;
    [SerializeField] private Sprite returnImg;

    public override void OnInitialize(GameObject sourceGameObject)
    {
        var guard = sourceGameObject.GetComponent<GuardController>();
        guard.OnBehaviorStarted.AddListener(ChangeText);

        if (guard != null && guard.currentBehavior != null ) 
            ChangeText(guard.currentBehavior.StateName);
    }

    /// <summary>
    /// Runs temporary state visualizer
    /// </summary>
    /// <param name="state"></param>
    public void ChangeText(GuardStates state)
    {
        switch(state)
        {
            case GuardStates.idle:
                stateImg.sprite = idleImg;
                break;
            case GuardStates.patrol:
                stateImg.sprite = patrolImg;
                break;
            case GuardStates.chase:
                stateImg.sprite = chaseImg;
                break;
            case GuardStates.attack:
                stateImg.sprite = attackImg;
                break;
            case GuardStates.surprised:
                stateImg.sprite = surprisedImg;
                break;
            case GuardStates.concussed:
                stateImg.sprite = stunnedImg;
                break;
            case GuardStates.search:
                stateImg.sprite = searchImg;
                break;
            case GuardStates.visionBreak:
                stateImg.sprite = visionBreakImg;
                break;
            case GuardStates.returnToPath:
                stateImg.sprite = returnImg;
                break;
        }
    }
}

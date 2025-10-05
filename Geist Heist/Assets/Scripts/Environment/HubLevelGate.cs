/*
 * Contributors: Toby
 * Creation: 10/3/25
 * Last Edited: 10/3/25
 * Summary: Disables/enables a gate based on if another level has been completed.
 * 
 * Note: This code is extremely similar to TetherHubDisplay.cs, and honestly could have been modularized with just one boolean.
 * I chose to make it two scripts in case of:
 *      1. the two objects end up having different behaviour (animations or vfx or something) 
 *      2. Simplicity of understanding what each script means on each object
 */

using NaughtyAttributes;
using UnityEngine;

public class HubLevelGate : MonoBehaviour
{
    [InfoBox("If 'Required Level' has been completed -> this object will be disabled.\n\nUse this for blocking off the player in the hub world")]
    [InfoBox("If a level is not appearing, make sure it is added to the build settings")]

    [SerializeField, Scene] private string RequiredLevel;


    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysHide;

    public void Start()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (DebugAlwaysHide)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive( !SaveDataManager.Instance.IsLevelCompleted(RequiredLevel));
    }
}

/*
 * Contributors: Toby
 * Creation:    10/3/2025
 * Last Edited:  3/3/2026
 * Summary: Disables/enables a gate based on if another level has been completed.
 * 
 * Note: This code is extremely similar to TetherHubDisplay.cs, and honestly could have been modularized with just one boolean.
 * I chose to make it two scripts in case of:
 *      1. the two objects end up having different behaviour (animations or vfx or something) 
 *      2. Simplicity of understanding what each script means on each object
 */

using NaughtyAttributes;
using UnityEngine;

/*
 * Review notes: i think this code is fine. but we need to reiterate to design to actually use it. 
 * This code was written specifically to avoid having multiple hub scenes, which is what we ended up doing >:/
 * -Toby
 */

public class HubLevelVisibilityToggle : MonoBehaviour
{
    [InfoBox("If 'Required Level' has been completed -> this object will be enabled or disabled.\n\nUse this for blocking off the player in the hub world")]
    [InfoBox("If a level is not appearing, make sure it is added to the build settings")]

    [SerializeField, Scene] private string RequiredLevel;
    [SerializeField] private HubVisibilityType Visibility;

    [Header("Debug")]
    [SerializeField, OnValueChanged(nameof(UpdateVisibility))] private bool DebugAlwaysApply;

    public void Start()
    {
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        if (DebugAlwaysApply)
        {
            gameObject.SetActive(false);
            return;
        }

        if(Visibility == HubVisibilityType.EnableIfLevelCompleted)
        {
            gameObject.SetActive(SaveDataManager.Instance.IsLevelCompleted(RequiredLevel));
        }
        else
        {
            gameObject.SetActive( ! SaveDataManager.Instance.IsLevelCompleted(RequiredLevel));
        }
        
    }

    private enum HubVisibilityType
    {
        EnableIfLevelCompleted,
        DisableIfLevelCompleted,
    }
}

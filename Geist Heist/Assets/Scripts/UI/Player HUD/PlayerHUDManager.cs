/*
 * Contributors: Toby
 * Creation Date: 11/13/2025
 * Last Modified:  2/10/2026
 * 
 * Brief Description: Stores public refrences to each player HUD component
 */

using NaughtyAttributes;
using UnityEngine;

public class PlayerHUDManager : Singleton<PlayerHUDManager>
{
    [Required] public CheckpointAnimation CheckpointAnimationRef;

    [SerializeField, Required] private KeyUIManager keyUI;
    [SerializeField, Required] private PossessableToolbar possessableToolbar;

    public void Initialize()
    {
        keyUI.Initialize();
        possessableToolbar.Initialize();

        GetComponent<DialogueUIManager>().Initialize(); 
    }
}

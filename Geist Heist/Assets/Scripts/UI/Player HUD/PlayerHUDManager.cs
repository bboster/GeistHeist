/*
 * Contributors: Toby
 * Creation Date: 11/13/2025
 * Last Modified: 11/13/2025
 * 
 * Brief Description: Stores public refrences to each player HUD component
 */

using NaughtyAttributes;

public class PlayerHUDManager : Singleton<PlayerHUDManager>
{
    [Required] public CheckpointAnimation CheckpointAnimationRef;
}

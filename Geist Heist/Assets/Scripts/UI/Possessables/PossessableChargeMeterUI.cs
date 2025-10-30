/*
 * Contributors: Toby
 * Creation Date: 10/27/2025
 * Last Modified: 10/27/2025
 * 
 * Brief Description: Shows charge amount for various possessables.
 * Different possessables have different charge UI behaviour.
 */

using UnityEngine;

public abstract class PossessableChargeMeterUI : MonoBehaviour
{
    public abstract void OnPossessionStarted();
    public abstract void UpdateCharge(float heldTime, float timeForMaxCharge);
}

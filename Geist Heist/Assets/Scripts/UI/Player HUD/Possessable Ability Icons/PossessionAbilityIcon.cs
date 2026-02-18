/*
 * Contributors:  Toby
 * Creation Date: 2/12/2026
 * Last Modified: 2/12/2026
 * 
 * Brief Description: Interface for each unique image/icon that every possessable has.  
 */

using UnityEngine;

public abstract class PossessionAbilityIcon : MonoBehaviour
{
    public abstract void OnPossessionStarted(PossessableObject possessable);
}

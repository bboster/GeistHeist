/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles possession and interactibility for the globe
 */
using UnityEngine;

public class GlobePossessableObject : PossessableObject, IInteractable
{
    [SerializeField] private bool debugAlwaysPossessable;
    bool IInteractable.IsInteractable()
    {
        if (SaveDataManager.Instance.AllLevelsCompleted() || (Application.isEditor && debugAlwaysPossessable))
        {
            return true;
        }
        return false;
    }
}

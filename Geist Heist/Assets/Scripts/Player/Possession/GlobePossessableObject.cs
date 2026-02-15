/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles possession and interactibility for the globe
 */
public class GlobePossessableObject : PossessableObject, IInteractable
{
    bool IInteractable.IsInteractable()
    {
        if (SaveDataManager.Instance.AllLevelsCompleted())
        {
            return true;
        }
        return false;
    }
}

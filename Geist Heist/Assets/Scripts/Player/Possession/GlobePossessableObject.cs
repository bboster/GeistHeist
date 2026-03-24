/*
 * Contributors: Sky
 * Creation Date: 2/12/26
 * Last Modified: 2/12/26
 * 
 * Brief Description: Handles possession and interactibility for the globe
 */
using NaughtyAttributes;
using NUnit.Framework;
using UnityEngine;

public class GlobePossessableObject : PossessableObject, IInteractable
{
    [SerializeField] private bool debugAlwaysPossessable;

    [SerializeField, Required] private GameObject interactBillboardUIPoint;

    void Start()
    {
        if (SaveDataManager.Instance.AllLevelsCompleted() == false && (Application.isEditor && debugAlwaysPossessable == false))
        {
            interactBillboardUIPoint.SetActive(false);
        }
        else
        {
            MeshRenderer[] meshRenderers = GetComponentsInChildren<MeshRenderer>();

            //make it add material later maybe? shader or better material?
            foreach (MeshRenderer meshRenderer in meshRenderers)
            {
                meshRenderer.material = VisibleUnPossessedMaterial;
            }
        }
    }

    bool IInteractable.IsInteractable()
    {
        if (SaveDataManager.Instance.AllLevelsCompleted() || (Application.isEditor && debugAlwaysPossessable))
        {
            return true;
        }
        return false;
    }
}

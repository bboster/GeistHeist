/*
 * Contributors: Toby
 * Creation: 10/9/25
 * Last Edited: 10/9/25
 * Summary: INITIALIZES a UI billboard ui object that 
 * - stays in a single world point, 
 * - remains a consistent size, 
 * - always faces the player
 */

using UnityEngine;
using NaughtyAttributes;

public class BillboardUIPoint : MonoBehaviour
{
    [Tooltip("If true, hides this ui element at start")]
    [SerializeField] private bool HideByDefault = true;
    [SerializeField, Required] private GameObject UIObjectPrefab;
    [SerializeField, Required] private GameObject SourceGameObject;

    [Header("Debug")]
    bool HideGizmos = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var uiGameobject = Instantiate(UIObjectPrefab);

        BillboardUIManager.Instance.RegisterAndInitializeBillboardUIPoint(this.transform, uiGameobject.GetComponent<IBillboardUI>(), SourceGameObject, HideByDefault);
    }

    private void OnDrawGizmos()
    {
        if (HideGizmos)
            return;

        Gizmos.DrawSphere(transform.position, 0.4f);
    }
}

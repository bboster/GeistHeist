/*
 * Contributors: Toby
 * Creation: 10/9/25
 * Last Edited: 10/16/25
 * Summary: INITIALIZES a UI billboard ui object that 
 * - stays in a single world point, 
 * - remains a consistent size, 
 * - always faces the player
 */

using UnityEngine;
using NaughtyAttributes;

public class BillboardUIPoint : MonoBehaviour
{
    [SerializeField, Required] private GameObject UIObjectPrefab;
    [SerializeField, Required] private GameObject SourceGameObject;

    [HideInInspector] public IBillboardUI billboardUI;

    [Header("Debug")]
    bool HideGizmos = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        billboardUI = Instantiate(UIObjectPrefab).GetComponent<IBillboardUI>();
        BillboardUIManager.Instance.RegisterAndInitializeBillboardUIPoint(this.transform, billboardUI, SourceGameObject);
    }

    private void OnDrawGizmos()
    {
        if (HideGizmos)
            return;

        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }
}
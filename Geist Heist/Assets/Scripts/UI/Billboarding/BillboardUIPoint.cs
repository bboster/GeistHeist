/*
 * Contributors: Toby
 * Creation: 10/9/2025
 * Last Edited: 12/2/2025
 * 
 * Summary: INITIALIZES a UI billboard ui object that 
 * - stays in a single world point, 
 * - remains a consistent size, 
 * - always faces the player
 */

using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

public class BillboardUIPoint : MonoBehaviour
{
    [SerializeField, Required] public GameObject UIObjectPrefab;
    [SerializeField] private GameObject SourceGameObject;

    [HideInInspector] public IBillboardUI billboardUI;

    [Header("Debug")]
    bool HideGizmos = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // this is expected behavior is this is an Onomatopoeia point
        if (UIObjectPrefab == null)
        {
            Debug.LogError("Prefab null");
            return;
        }

        billboardUI = Instantiate(UIObjectPrefab).GetComponent<IBillboardUI>();
        BillboardUIManager.Instance.RegisterAndInitializeBillboardUIPoint(this.transform, billboardUI, SourceGameObject); 
    }

    private void OnDrawGizmos()
    {
        if (HideGizmos)
            return;

        Gizmos.DrawWireSphere(transform.position, 0.4f);
    }


    public void DestroySelf()
    {
        Destroy(billboardUI.gameObject);
        Destroy(this.gameObject);
    }
}
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

public class BillboardUIPoint : MonoBehaviour
{
    [SerializeField] private GameObject UIObjectPrefab;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var uiGameobject = Instantiate(UIObjectPrefab);

        BillboardUIManager.Instance.RegisterBillboardUIPoint(this, )
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

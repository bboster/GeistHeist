/*
 * Contributors: Toby,
 * Creation Date: 11/4/25
 * Last Modified: 11/4/25
 * 
 * Brief Description: Rotates a gameobject over time, forever
 */

using NaughtyAttributes;
using UnityEngine;

public class RotateOverTime : MonoBehaviour
{
    [SerializeField] private bool x;
    [SerializeField, ShowIf(nameof(x)), Label("Degrees Per Second")] private float xRotationSpeed = 0;

    [SerializeField] private bool y;
    [SerializeField, ShowIf(nameof(y)), Label("Degrees Per Second")] private float yRotationSpeed = 0;

    [SerializeField] private bool z;
    [SerializeField, ShowIf(nameof(z)), Label("Degrees Per Second")] private float zRotationSpeed = 0;

    // Update is called once per frame
    void Update()
    {
        Vector3 delta = new Vector3(
                x ? Time.deltaTime * xRotationSpeed : 0,
                y ? Time.deltaTime * yRotationSpeed : 0,
                z ? Time.deltaTime * zRotationSpeed : 0
            );

        transform.eulerAngles = transform.eulerAngles + delta;
    }
}

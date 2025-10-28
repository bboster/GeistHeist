/*
 * Contributors: Toby
 * Creation Date: 10/27/2025
 * Last Modified: 10/27/2025
 * 
 * Brief Description: Shows charge amount for car.
 * Uses a rotating needle to replicate a speedometer.
 * 
 */

using NaughtyAttributes;
using UnityEngine;

public class ToyCarSpeedometerUI : PossessableChargeMeterUI
{

    [Header("Angle Settings")]
    [Tooltip("Angle of the ticker at the left")]
    [SerializeField] private float minAngle = 90;
    [Tooltip("Angle of the ticker at the right")]
    [SerializeField] private float maxAngle = -70;
    [SerializeField] private float shakeStrength = 15;
    [SerializeField] private float shakeSpeed = 3;

    [Header("Opacity Settings")]
    [SerializeField] private bool HideWhenNotHeld = true;
    [SerializeField, ShowIf(nameof(HideWhenNotHeld))] private float SecondsToShow = 0.15f;
    [SerializeField, ShowIf(nameof(HideWhenNotHeld))] private float SecondsToHide = 0.3f;

    [Header("Components")]
    [SerializeField, Required] private RectTransform pointerTransform;
    [SerializeField, Required] private CanvasGroup speedometerGroup;

    private float currentOpacity=0;
    private float lastHeldTime;

    public override void OnPossessionStarted()
    {
        currentOpacity = 0;
    }

    public override void UpdateCharge(float heldTime, float timeForMaxCharge)
    {
        currentOpacity = GetOpacity(heldTime);
        speedometerGroup.alpha = currentOpacity;

        float t = heldTime / timeForMaxCharge;
        t = Mathf.Clamp(t, 0, 1);

        // basic lerp
        float z_angle = Mathf.LerpAngle(minAngle, maxAngle, t);

        // add shaky effect for extra time held
        if(heldTime >= timeForMaxCharge)
        {
            float t_extra = (heldTime - timeForMaxCharge) * shakeSpeed;

            // ping pong 0 <-> 1
            t_extra = Mathf.PingPong(t_extra, 1);

            // Add shakey affect
            z_angle += Mathf.Lerp(-shakeStrength, shakeStrength, t_extra);
        }
        
        // Smooth
        z_angle = Mathf.LerpAngle(pointerTransform.eulerAngles.z, z_angle, Time.deltaTime * 10);

        // Apply angle
        pointerTransform.eulerAngles = pointerTransform.eulerAngles.WithZ(z_angle);

        lastHeldTime = heldTime;
    }

    private float GetOpacity(float heldTime)
    {
        // Change opacity if applicable
        if (!HideWhenNotHeld)
        {
            return 1;
        }

        float a;

        if(heldTime <= 0)
        {
            return 0;
        }
        // if charge is decreasing
        else if (heldTime < lastHeldTime)
        {
            a = currentOpacity - (Time.deltaTime / SecondsToHide);
        }
        // if charge is increasing
        else
        {
            a = heldTime / SecondsToShow;
        }

        a = Mathf.Clamp(a, 0, 1);
        return Mathf.Lerp(currentOpacity, a, Time.deltaTime * 10);
    }
}

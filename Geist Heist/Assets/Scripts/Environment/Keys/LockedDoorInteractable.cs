/*
 * Contributors:Josh
 * Creation:2/2/2026
 * Last Edited: 02/04/2026
 * Summary: Locked door that implements the project's IInteractable.
 * Designer picks the hinge corner in the inspector (this is where the hinge is).
 * Door computes pivot from renderers based on the selected hinge corner.
 * Door swings deterministically: front hinges rotate by -openAngle, back hinges by +openAngle.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using FMODUnity;

public class LockedDoorInteractable : MonoBehaviour, IActionable
{
    public enum HingeCorner
    {
        BackLeft,
        FrontLeft,
        BackRight,
        FrontRight,
    }

    [Header("Door")]
    [SerializeField] private KeyType requiredKey = KeyType.Circle;
    [SerializeField] private string checkpointStateIdOverride;
    
    [Tooltip("Which corner (local to the door) the HINGE actually sits on")]
    [SerializeField] private HingeCorner hingeCorner = HingeCorner.BackLeft;        

    [SerializeField, Min(0f)] private float autoPivotInset = 0f; // small inward offset from bounds edge toward center
    [SerializeField, Min(0f)] private float openAngle = 90f;
    [SerializeField, Min(0.01f)] private float openSeconds = 0.6f;

    [Header("Debug (Editor only)")]
    [SerializeField] private bool ShowDebugGizmos = true;

    // runtime
    private Vector3? _computedPivotWorld = null;
    private bool _isOpen;
    private string _cachedCheckpointStateId;

    // IInteractable
    public void Action() => TryOpen();

    public bool IsActionable()
    {
        if (_isOpen) return false;
        
        return true;
    }

    public void TryOpen()
    {
        if (_isOpen) return;

        if (KeyManager.Instance != null && KeyManager.Instance.HasKey(requiredKey))
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.DoorOpen, transform.position);

            _isOpen = true;
            StartCoroutine(OpenDoorRoutine());
        }
        else
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.DoorLocked, transform.position);

            Debug.Log($"Door locked. Required key: {requiredKey}");
        }
    }

    private IEnumerator OpenDoorRoutine()
    {
        Vector3 pivotWorld = ResolvePivotWorld();

        Vector3 axis = transform.up;
        float angle = openAngle;
        float chosenSign = GetOpenSign();

        // Animate rotation around pivot
        float duration = Mathf.Max(0.0001f, openSeconds);
        float elapsed = 0f;
        float rotated = 0f;
        while (elapsed < duration)
        {
            float dt = Time.deltaTime;
            elapsed += dt;
            float step = (dt / duration) * angle;
            transform.RotateAround(pivotWorld, axis, chosenSign * step);
            rotated += step;
            yield return null;
        }

        // snap remaining if small numeric drift
        float remaining = angle - rotated;
        if (Mathf.Abs(remaining) > 0.0005f)
            transform.RotateAround(pivotWorld, axis, chosenSign * remaining);
    }

    public void RestoreCheckpointStateIfNeeded()
    {
        if (_isOpen)
            return;

        if (LevelManager.Instance == null)
            return;

        if (!LevelManager.Instance.IsDoorOpened(GetCheckpointStateId()))
            return;

        _isOpen = true;
        OpenDoorInstantly();
    }

    public bool TryGetOpenDoorStateId(out string doorStateId)
    {
        doorStateId = null;

        if (!_isOpen)
            return false;

        doorStateId = GetCheckpointStateId();
        return !string.IsNullOrEmpty(doorStateId);
    }

    private void OpenDoorInstantly()
    {
        Vector3 pivotWorld = ResolvePivotWorld();
        transform.RotateAround(pivotWorld, transform.up, GetOpenSign() * openAngle);
    }

    private Vector3 ResolvePivotWorld()
    {
        if (!_computedPivotWorld.HasValue)
        {
            if (TryComputePivotFromRenderers(out var computed))
                _computedPivotWorld = computed;
            else
                _computedPivotWorld = transform.position;
        }

        return _computedPivotWorld.Value;
    }

    private float GetOpenSign()
    {
        // front hinges rotate toward -angle, back hinges rotate toward +angle
        return (hingeCorner == HingeCorner.FrontLeft || hingeCorner == HingeCorner.FrontRight) ? -1f : +1f;
    }

    private string GetCheckpointStateId()
    {
        if (!string.IsNullOrWhiteSpace(checkpointStateIdOverride))
            return checkpointStateIdOverride;

        if (!string.IsNullOrEmpty(_cachedCheckpointStateId))
            return _cachedCheckpointStateId;

        _cachedCheckpointStateId = BuildHierarchyStateId();
        return _cachedCheckpointStateId;
    }

    private string BuildHierarchyStateId()
    {
        List<string> segments = new();
        Transform current = transform;

        while (current != null)
        {
            segments.Add($"{current.name}[{current.GetSiblingIndex()}]");
            current = current.parent;
        }

        segments.Reverse();
        return $"{gameObject.scene.buildIndex}:{string.Join("/", segments)}";
    }

    // Compute pivot from child renderers using the inspector-selected hingeCorner (hinge sits at this corner).
    private bool TryComputePivotFromRenderers(out Vector3 pivotWorld)
    {
        pivotWorld = Vector3.zero;
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return false;

        bool first = true;
        Vector3 min = Vector3.zero;
        Vector3 max = Vector3.zero;

        foreach (var r in renderers)
        {
            Bounds b = r.bounds;
            Vector3[] corners = new Vector3[8]
            {
                new Vector3(b.min.x, b.min.y, b.min.z),
                new Vector3(b.min.x, b.min.y, b.max.z),
                new Vector3(b.min.x, b.max.y, b.min.z),
                new Vector3(b.min.x, b.max.y, b.max.z),
                new Vector3(b.max.x, b.min.y, b.min.z),
                new Vector3(b.max.x, b.min.y, b.max.z),
                new Vector3(b.max.x, b.max.y, b.min.z),
                new Vector3(b.max.x, b.max.y, b.max.z)
            };

            foreach (var worldCorner in corners)
            {
                Vector3 local = transform.InverseTransformPoint(worldCorner);
                if (first) { min = local; max = local; first = false; }
                else { min = Vector3.Min(min, local); max = Vector3.Max(max, local); }
            }
        }

        // hingeCorner corresponds directly to hinge location:
        float xLocal = (hingeCorner == HingeCorner.BackLeft || hingeCorner == HingeCorner.FrontLeft) ? min.x : max.x;
        float zLocal = (hingeCorner == HingeCorner.BackLeft || hingeCorner == HingeCorner.BackRight) ? min.z : max.z;
        float yLocal = (min.y + max.y) * 0.5f;

        // inset toward center (compute center and move from edge toward center)
        float centerX = (min.x + max.x) * 0.5f;
        float dirX = Mathf.Sign(centerX - xLocal); // direction from edge toward center
        float pivotX = xLocal + dirX * autoPivotInset;

        float centerZ = (min.z + max.z) * 0.5f;
        float dirZ = Mathf.Sign(centerZ - zLocal);
        float pivotZ = zLocal + dirZ * autoPivotInset;

        Vector3 pivotLocal = new Vector3(pivotX, yLocal, pivotZ);
        pivotWorld = transform.TransformPoint(pivotLocal);
        return true;
    }

    // Compute a point on the door face opposite the hinge corner (in world space)
    private Vector3 ComputeOppositeFaceSampleWorld()
    {
        if (ComputeLocalBounds(out var localMin, out var localMax))
        {
            float centerY = (localMin.y + localMax.y) * 0.5f;
            // opposite corner of hinge
            HingeCorner opposite = GetOppositeCorner(hingeCorner);

            float sampleX = (opposite == HingeCorner.BackLeft || opposite == HingeCorner.FrontLeft) ? localMin.x : localMax.x;
            float sampleZ = (opposite == HingeCorner.BackLeft|| opposite == HingeCorner.BackRight ) ? localMin.z : localMax.z;

            Vector3 sampleLocal = new Vector3(sampleX, centerY, sampleZ);
            return transform.TransformPoint(sampleLocal);
        }

        return transform.position;
    }   

    // Compute local bounds (min/max in door-local space). Returns false if no renderers.
    private bool ComputeLocalBounds(out Vector3 localMin, out Vector3 localMax)
    {
        localMin = Vector3.zero;
        localMax = Vector3.zero;
        var renderers = GetComponentsInChildren<Renderer>();
        if (renderers == null || renderers.Length == 0) return false;

        bool first = true;
        foreach (var r in renderers)
        {
            Bounds b = r.bounds;
            Vector3[] corners = new Vector3[8]
            {
                new Vector3(b.min.x, b.min.y, b.min.z),
                new Vector3(b.min.x, b.min.y, b.max.z),
                new Vector3(b.min.x, b.max.y, b.min.z),
                new Vector3(b.min.x, b.max.y, b.max.z),
                new Vector3(b.max.x, b.min.y, b.min.z),
                new Vector3(b.max.x, b.min.y, b.max.z),
                new Vector3(b.max.x, b.max.y, b.min.z),
                new Vector3(b.max.x, b.max.y, b.max.z)
            };

            foreach (var worldCorner in corners)
            {
                Vector3 local = transform.InverseTransformPoint(worldCorner);
                if (first) { localMin = local; localMax = local; first = false; }
                else { localMin = Vector3.Min(localMin, local); localMax = Vector3.Max(localMax, local); }
            }
        }
        return true;
    }

    private static HingeCorner GetOppositeCorner(HingeCorner c)
    {
        switch (c)
        {
            case HingeCorner.BackLeft:  return HingeCorner.BackRight;
            case HingeCorner.FrontLeft: return HingeCorner.FrontRight;
            case HingeCorner.BackRight: return HingeCorner.BackLeft;
            case HingeCorner.FrontRight: return HingeCorner.FrontLeft;
            default: return c;
        }
    }

    // utility: rotate point around pivot by angle degrees about axis
    private static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 axis, float angleDegrees)
    {
        return Quaternion.AngleAxis(angleDegrees, axis) * (point - pivot) + pivot;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (openSeconds < 0.01f) openSeconds = 0.01f;
        if (openAngle < 0f  ) openAngle = 0f;
    }

    private void OnDrawGizmosSelected()
    {
        if (!ShowDebugGizmos) return;

        // compute pivot (computed)
        Vector3 pivotWorld;
        if (!_computedPivotWorld.HasValue)
        {
            if (!TryComputePivotFromRenderers(out var computed))
                pivotWorld = transform.position;
            else
                pivotWorld = computed;
        }
        else pivotWorld = _computedPivotWorld.Value;

        // draw pivot
        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(pivotWorld, 0.03f);
        UnityEditor.Handles.Label(pivotWorld + Vector3.up * 0.05f, "Pivot");

        // draw sample point & candidate positions
        Vector3 sample = ComputeOppositeFaceSampleWorld();
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(sample, 0.03f);
        UnityEditor.Handles.Label(sample + Vector3.up * 0.05f, "Sample(face)");

        Vector3 previewAngle;

        if (hingeCorner == HingeCorner.BackLeft || hingeCorner == HingeCorner.FrontRight)
        {
            previewAngle = RotatePointAroundPivot(sample, pivotWorld, transform.up, +openAngle);
            Gizmos.color = Color.green;
            UnityEditor.Handles.DrawLine(sample, previewAngle);
            Gizmos.DrawSphere(previewAngle, 0.02f);
            UnityEditor.Handles.Label(previewAngle + Vector3.up * 0.03f, "angle");
        }

        if (hingeCorner == HingeCorner.FrontLeft || hingeCorner == HingeCorner.BackRight)   
        {
            previewAngle = RotatePointAroundPivot(sample, pivotWorld, transform.up, -openAngle);
            Gizmos.color = Color.red;
            UnityEditor.Handles.DrawLine(sample, previewAngle);
            Gizmos.DrawSphere(previewAngle, 0.02f);
            UnityEditor.Handles.Label(previewAngle + Vector3.up * 0.03f, "-angle");
        }

        // draw player position used for test
        Vector3 playerPos = PlayerManager.Instance != null && PlayerManager.Instance.CurrentObject != null
            ? PlayerManager.Instance.CurrentObject.transform.position
            : (Camera.main != null ? Camera.main.transform.position : pivotWorld + transform.forward);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(playerPos, 0.04f);
        UnityEditor.Handles.Label(playerPos + Vector3.up * 0.05f, "Player");

        // draw door-local axes for orientation check
        Vector3 origin = transform.position;
        float axisLen = 0.5f;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, origin + transform.right * axisLen); // local X
        UnityEditor.Handles.Label(origin + transform.right * axisLen, "Local X");
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(origin, origin + transform.forward * axisLen); // local Z
        UnityEditor.Handles.Label(origin + transform.forward * axisLen, "Local Z");
    }
#endif
}

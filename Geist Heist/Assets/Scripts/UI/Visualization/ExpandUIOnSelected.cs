/*
 * Contributors: Toby
 * Creation Date: 4/7/2026
 * Last Modified: 4/7/2026
 * 
 * Brief Description: When controller selects this ui element, it expands to set scale.
 */


using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class ExpandUIOnSelected : MonoBehaviour, ISelectHandler, IDeselectHandler

{
    [SerializeField] private Vector3 scale = new Vector3(1.25f, 1.25f, 1.25f);
    [SerializeField] private float smoothSeconds = 0.1f;
    [SerializeField] private bool controllerOnly = true;
    [Required, SerializeField] private RectTransform elementToScale;

    private Coroutine coroutine;
    private Vector3 baseScale;

    void Start()
    {
        if (elementToScale == null) elementToScale = GetComponent<RectTransform>();

        baseScale = elementToScale.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (controllerOnly && InputEvents.Instance.IsGamepadActive() == false)
            return;

        coroutine = StaticUtilities.AnimateScale(elementToScale, scale, smoothSeconds, unscaledTime: true, coroutine);
    }


    public void OnDeselect(BaseEventData eventData)
    {
        coroutine = StaticUtilities.AnimateScale(elementToScale, baseScale, smoothSeconds, unscaledTime: true, coroutine);
    }
}

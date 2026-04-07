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
    [Required, SerializeField] private RectTransform elementToScale;

    private Coroutine coroutine;
    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        coroutine = StaticUtilities.AnimateScale(elementToScale, scale, smoothSeconds, unscaledTime: true, coroutine);
    }


    public void OnDeselect(BaseEventData eventData)
    {
        coroutine = StaticUtilities.AnimateScale(elementToScale, baseScale, smoothSeconds, unscaledTime: true, coroutine);
    }
}

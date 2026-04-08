using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float ImageFadeAwaySeconds = 0.5f;

    [Header("Look and move tutorial")]
    [SerializeField, Required] private Image MovementImage;
    [SerializeField, Required] private Sprite KeyboardMovementSprite;
    [SerializeField, Required] private Sprite ControllerMovementSprite;
    [SerializeField, Required] private float MovementTutorialDelaySeconds = 1f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TutorialAnimationSequence()
    {

    }

    private void OnControllerChanged()
    {
        if (InputEvents.Instance.IsGamepadActive())
        {
            MovementImage.sprite = ControllerMovementSprite;
        }
        else
        {
            MovementImage.sprite = KeyboardMovementSprite;
        }
    }
}

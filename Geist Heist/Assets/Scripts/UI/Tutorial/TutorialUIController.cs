using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUIController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float ImageFadeAwaySeconds = 0.5f;

    [Header("Look and move tutorial")]
    [SerializeField, Required] private CanvasGroup MovementCanvasGroup;
    [SerializeField, Required] private Image MovementImage;
    [SerializeField, Required] private Sprite KeyboardMovementSprite;
    [SerializeField, Required] private Sprite ControllerMovementSprite;
    [SerializeField, Required] private float MovementTutorialDelaySeconds = 3f;

    [Header("Possess tutorial")]
    [SerializeField, Required] private CanvasGroup PossessCanvasGroup;
    [SerializeField, Required] private Image PossessImage;
    [SerializeField, Required] private Sprite KeyboardPossessSprite;
    [SerializeField, Required] private Sprite ControllerPossessSprite;
    [SerializeField, Required] private float PossessTutorialDelaySeconds = 4f;

    [Header("Guard tutorial")]
    [SerializeField, Required] private CanvasGroup GuardCanvasGroup;
    [SerializeField, Required] private Image GuardImage;
    [SerializeField, Required] private Sprite KeyboardGuardSprite;
    [SerializeField, Required] private Sprite ControllerGuardSprite;
    [SerializeField, Required] private float GuartWaitSeconds = 6f;

    private bool playerMovedEver = false;
    private bool playerPossessedEver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InputEvents.MoveStarted.AddListener(OnMoveStarted);
        PlayerManager.OnPossessionObjectChanged.AddListener(OnPlayerPossessionObjectChanged);
        StartCoroutine(TutorialAnimationSequence());
    }

    void OnMoveStarted() => playerMovedEver = true;

    void OnPlayerPossessionObjectChanged(PossessableObject possessable)
    {
        if (possessable == PlayerManager.Instance.PlayerGhostObject)
            return;

        playerPossessedEver = true;
    }

    private IEnumerator TutorialAnimationSequence()
    {
        // === MOVEMENT TUTORIAL ===

        yield return StaticUtilities.FadeOpacity(MovementCanvasGroup, 0, 1, ImageFadeAwaySeconds, unscaledTime: false);

        // wait for them to move!
        while (playerMovedEver == false) yield return null;

        yield return StaticUtilities.FadeOpacity(MovementCanvasGroup, 1, 0, ImageFadeAwaySeconds, unscaledTime: false);
        yield return new WaitForSeconds(MovementTutorialDelaySeconds);

        // === POSSESS TUTORIAL ===

        yield return StaticUtilities.FadeOpacity(PossessCanvasGroup, 0, 1, ImageFadeAwaySeconds, unscaledTime: false);

        // wait for them to possess!
        while (playerPossessedEver == false) yield return null;

        yield return StaticUtilities.FadeOpacity(PossessCanvasGroup, 1, 0, ImageFadeAwaySeconds, unscaledTime: false);
        yield return new WaitForSeconds(PossessTutorialDelaySeconds);

        // === GUARD TUTORIAL ===

        yield return StaticUtilities.FadeOpacity(GuardCanvasGroup, 0, 1, ImageFadeAwaySeconds, unscaledTime: false);

        // wait around
        yield return new WaitForSeconds(GuartWaitSeconds);

        yield return StaticUtilities.FadeOpacity(GuardCanvasGroup, 1, 0, ImageFadeAwaySeconds, unscaledTime: false);

    }

    private void OnControllerChanged()
    {
        if (InputEvents.Instance.IsGamepadActive())
        {
            MovementImage.sprite = ControllerMovementSprite;
            PossessImage.sprite = ControllerPossessSprite;
        }
        else
        {
            MovementImage.sprite = KeyboardMovementSprite;
            MovementImage.sprite = ControllerPossessSprite;
        }
    }
}

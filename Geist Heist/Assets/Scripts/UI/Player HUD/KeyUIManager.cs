/*
 * Contributors: Toby S
 * Creation:     2/10/26
 * Last Edited:  2/10/26
 * 
 * Summary:  Hides if there are no keys in the scene.
 * Displays all keys in scene, greys them out if they are uncollected;
 */

using NaughtyAttributes;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KeyUIManager : MonoBehaviour
{
    [SerializeField] private Color keyUncollectedColor = Color.gray;
    [SerializeField, Required] private Image keyIcon;
    [SerializeField] private List<Image> keyImages;

    private Dictionary<KeyType, Image> keyImagePairs = new();

    KeyItem[] keysInScene;

    [Header("Key Icon Shake Animation")]
    [SerializeField] private float keyShakeAngle = 15;
    [SerializeField] private int keyIconNumberOfShakes = 3;
    [SerializeField] private float keyIconSecondsPerShake = 0.3f;

    [Header("Expand Key Animation")]
    [SerializeField] private float shakeCollectedKeyShakeAngle = 10f;
    [SerializeField] private float keyCollectedSecondsPerShake = 0.1f;
    [SerializeField] private int keyCollectedNumberOfShakes = 3;
    [SerializeField] private float expandedKeyScale = 1.25f;
    [SerializeField] private float expandSeconds = 0.5f;
    [SerializeField] private float retractSeconds = 0.3f;

    public void Initialize()
    {
        // hide this ui if no keys here
        keysInScene = FindObjectsByType<KeyItem>(FindObjectsSortMode.None);
        if(keysInScene.IsNullOrEmpty())
        {
            this.gameObject.SetActive(false);
            return;
        }

        // account for duplicate keys
        var uniqueKeysInScene = keysInScene
            .Select(k => k.keyType)
            .Distinct()
            .OrderBy(k => KeyManager.Instance.KeyUIIcons.FindIndex(kui => kui.Key == k))
            .ToList();


        if (uniqueKeysInScene.Count() > keyImages.Count) {
            Debug.LogError($"There are more keys ({uniqueKeysInScene.Count()} in scene than usable images in the key UI ({keyImages.Count})");
            return;
        }

        // Create keyType / image pairs
        for(int i=0; i< uniqueKeysInScene.Count(); i++)
        {
            keyImagePairs.Add(uniqueKeysInScene[i], keyImages[i]);
            Sprite sprite = KeyManager.Instance.GetKeySprite(uniqueKeysInScene[i]);
            if(sprite == null)
            {
                // hopefully designers listen because this WILL be a problem.
                Debug.LogError($"No key UI icon has been defined for keytype: {uniqueKeysInScene[i]}. Please define it in the KeyManager prefab.");
            }
            keyImages[i].sprite = sprite;
            keyImages[i].color = keyUncollectedColor;
        }

        // Hide all of the remaining keys
        for(int i= uniqueKeysInScene.Count(); i< keyImages.Count(); i++)
        {
            keyImages[i].gameObject.SetActive(false);
        }

        KeyManager.Instance.OnKeyCollected += OnKeyCollected;
    }

    private void OnKeyCollected(KeyType keyCollected)
    {
        var image = keyImagePairs[keyCollected];
        image.color = Color.white;

        StartCoroutine(ShakeAnimation(keyIcon.transform, keyShakeAngle, keyIconNumberOfShakes, keyIconSecondsPerShake));

        // expand and shake the keys image
        StartCoroutine(ExpandNewKeyAnimation(image.transform));
        StartCoroutine(ShakeAnimation(image.transform, shakeCollectedKeyShakeAngle, keyCollectedNumberOfShakes, keyCollectedSecondsPerShake));
    }

    #region Animations
    private IEnumerator ShakeAnimation(Transform uiElement, float shakeAngle, int numberOfShakes, float secondsPerShake)
    {
        // half-turn towards angle
        yield return StaticUtilities.AnimateRotation(uiElement, new Vector3(0, 0, shakeAngle), secondsPerShake / 2);

        for(int i=0; i< numberOfShakes - 1; i++)
        {
            float shakeDirection = i % 2 == 0 ? -1 : 1;
            float angle = shakeAngle * shakeDirection;

            yield return StaticUtilities.AnimateRotation(uiElement, new Vector3(0, 0, angle), secondsPerShake);
        }

        // return to original rotation
        yield return StaticUtilities.AnimateRotation(uiElement, Quaternion.identity, secondsPerShake / 2);

        /*for (int i = 0; i < numberOfShakes; i++)
        {
            float shakeDirection = i % 2 == 0 ? 1 : -1;

            float timeStarted = Time.time;
            float timeElapsed = 0;
            while(timeElapsed < secondsPerShake)
            {
                timeElapsed = Time.time - timeStarted;
                float t = timeElapsed / secondsPerShake;

                float angle = shakeAngle * shakeDirection;
                float z = Mathf.Lerp(-angle, angle, t);
                uiElement.transform.eulerAngles = new Vector3(0, 0, z);
                yield return null;

            }
        }*/
    }

    private IEnumerator ExpandNewKeyAnimation(Transform uiElement)
    {
        yield return StaticUtilities.AnimateScale(uiElement.transform, Vector3.one * expandedKeyScale, expandSeconds);

        yield return StaticUtilities.AnimateScale(uiElement.transform, Vector3.one, retractSeconds);
    }

    #endregion
}

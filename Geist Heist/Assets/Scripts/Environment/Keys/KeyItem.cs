/*
 * Contributors:Josh, Toby
 * Creation:2/1/2026
 * Last Edited: 2/1/2026
 * Summary: Allows player to collect keys in the environment. Upon walking into the key,
 * It plays the same collection animation as OptionalCollectible.
 * Key is added to KeyManager as soon as collection starts so it is usable during the pickup animation.
 * When animation finishes, the GameObject is destroyed.
 * TODO: // Simple enum for keys. Extend with the symbols used in UI.
 */

using System.Collections;
using UnityEngine;
using NaughtyAttributes;
[RequireComponent(typeof(Collider))]
public class KeyItem : MonoBehaviour
{
    [SerializeField] public KeyType keyType;
    [SerializeField] private GameObject collectionParticlePrefab;

    private bool _collected;
    private Collider childCollider;
    private ParticleSystem particleSystem;

    private void Awake()
    {
        childCollider = GetComponent<Collider>();
        if (childCollider != null) childCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_collected) return;

        // Use existing game convention to detect player possession object
        if (other.transform.GetComponent<PossessableObject>() == null) return;

        _collected = true;
        if (childCollider != null) childCollider.enabled = false;

        RegisterKey();

        // optional particle
        if (collectionParticlePrefab != null)
            StaticUtilities.PlayAndDestroyParticle(collectionParticlePrefab, transform.position);

        // start collection animation
        StartCoroutine(CollectAnimation());
    }

    //NOTE THAT ANIMATION IS TEMPORARY AND SHOULD BE REPLACED LATER
    //Animations reused from OptionalCollectible; Thanks toby!

    #region Animation 

    // hardcoding these values because this animation SHOULD be temporary

    private float spinningSpeed = 2;
    private float spinningSeconds = 3;
    private float backflipHeight = 5;
    private float goToPlayerSeconds = 0.5f;

    /// <summary>
    /// The exact same backflip animation as midwest goodbye, just sideways
    /// PLEAASE dont keep this in the final game
    /// </summary>
    /// <returns></returns>
    private IEnumerator CollectAnimation()
    {
        //TODO: change animation to anything else

        Vector3 startPos = transform.position;
        Vector3 startEulers = transform.eulerAngles;
        Vector3 startScale = transform.localScale;

        if (particleSystem != null)
        {
            /* Note: the particle system is automatically destroyed when it is done playing.
             * This code is fine and harmless, but it would be better to change the duration of the original particle.
             */
            //particleSystem.Stop(false);
        }

        // Sideflips
        float timeStarted = Time.time;
        float t = 0;
        while (t < 1)
        {
            t = (Time.time - timeStarted) / spinningSeconds;

            // if this code doesnt make sense to you then u shouldve paid more attention in ur trig class
            float y = Mathf.Sin(Mathf.PI * t / 2) * backflipHeight;
            var pos = startPos + new Vector3(0, y, 0);

            var rot = startEulers + new Vector3(0, t * spinningSpeed * 360, 0);

            transform.position = pos;
            transform.eulerAngles = rot;

            yield return null;
        }

        Debug.Log("Confetti explosion goes here???"); //TODO:

        // Go to her...
        timeStarted = Time.time;
        t = 0;

        startPos = transform.position;
        var startRotation = transform.rotation;
        while (t < 1)
        {
            t = (Time.time - timeStarted) / goToPlayerSeconds;

            // if this code doesnt make sense to you then u shouldve paid more attention in ur trig class

            Vector3 playerPos = PlayerManager.Instance.CurrentObject.transform.position;

            if (startPos == null || playerPos == null)
            {
                break;
            }

            var pos = Vector3.Lerp(startPos, playerPos, t * t); // t * t so it gets faster (plug x^2 into desmos and look at 0-1 to see the effect for yourself! it will be mind boggling!!!!!)
            var directionToPlayer = (transform.position - playerPos).normalized;
            var targetRot = Quaternion.LookRotation(directionToPlayer + Vector3.down);
            var rot = Quaternion.Lerp(startRotation, targetRot, t * 3);
            var scale = Vector3.Lerp(startScale, Vector3.zero, t);

            transform.position = pos;
            transform.rotation = rot;
            transform.localScale = scale;

            yield return null;
        }

        AfterCollectAnimationFinished();
    }

    private void AfterCollectAnimationFinished()
    {
        Destroy(this.gameObject);
    }

    private void RegisterKey()
    {
        if (KeyManager.Instance != null)
            KeyManager.Instance.AddKey(keyType);
        else
            Debug.LogWarning("KeyItem: KeyInventory not found in scene. Add KeyInventory component to a scene object (e.g. PlayerManager).");
    }
    #endregion
}

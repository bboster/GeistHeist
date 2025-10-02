/*
 * Contributors: Toby
 * Creation: 9/30/25
 * Last Edited: 9/30/25
 * Summary: Collectable object. saves to player data.
 * 
 * TODO: make a vfx/shader/material for if player is replaying level, and this collectable has already been collected
 */

using NaughtyAttributes;
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor.Compilation;
using System.Threading.Tasks;
#endif

public class OptionalCollectable : MonoBehaviour
{
    [SerializeField] private Collectable ThisCollectable;
    [SerializeField] private GameObject CollectionParticlePrefab;
    
    private void OnTriggerEnter(Collider other)
    {
        // TODO: animation (?)

        StaticUtilities.PlayAndDestroyParticle(CollectionParticlePrefab, transform.position);
        SaveDataManager.Instance.MarkCollectableAsCollected(ThisCollectable);
        //Destroy(this.gameObject);
        StartCoroutine(CollectAnimation());
    }

    private void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }


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
        //StaticUtilities.PlayAndDestroyParticle(collectedParticlesPrefab, rectTransform.WorldPosition());
        Destroy(this.gameObject);
    }

    #endregion


    #region Debug Tools 
#if UNITY_EDITOR
    private string filePath => Application.dataPath + "/Scripts/Environment/OptionalCollectable.cs";

    [Button("Add new collectable as enum")]
    private async void RegisterAsEnum()
    {
        // code that rewrites the script itself. lol

        var currentEnums = Enum.GetNames(typeof(Collectable));
        string name = new string(
            this.gameObject.name
            .Replace(" ","_")
            .Where(c => !" $.,;'()!@#$%^&*()/\\\"[]-".Contains(c))
            .ToArray()
            );
        Debug.Log("Saving enum as " + name);

        if (currentEnums.Contains(name))
        {
            Debug.Log("Enum already exists");
            return;
        }

        // Sorry the path is hardcoded idk how to fix that
        string currentContent = File.ReadAllText(filePath);

        int index = currentContent.LastIndexOf("}");
        string newContent = currentContent.Insert(index, $"\t{name},\n");

        File.WriteAllText(filePath, newContent);

        // Force code to recompile
        CompilationPipeline.RequestScriptCompilation();

        Debug.Log("Script recompilation requested.");

        do
        {
            await Task.Delay(3000);
        }
        while (EditorApplication.isCompiling);
        await Task.Delay(3000);

        ThisCollectable = (Collectable)((int)currentEnums.Count() + 1);

        Debug.Log("Saved enum as " + name);

    }

    [Button("Delete this enum")]
    private async void DeleteThisEnum()
    {
        if(ThisCollectable == 0) {
            Debug.Log("Cant delete default enum");
            return;
        }

        string currentContent = File.ReadAllText(filePath);

        int index = currentContent.LastIndexOf("}");
        string newContent = currentContent.Replace($"{ThisCollectable.ToString()},", "");

        File.WriteAllText(filePath, newContent);

        ThisCollectable = 0;

        // Force code to recompile
        CompilationPipeline.RequestScriptCompilation();

        Debug.Log("Script recompilation requested.");

        do
        {
            await Task.Delay(3000);
        }
        while (EditorApplication.isCompiling);

        await Task.Delay(3000);

        Debug.Log("done recompiling");

    }
#endif
#endregion
}


// do not move this enum from being at the BOTTOM of THIS script
// This enum writes itself using debug tools
public enum Collectable
{
    None,
	Test_Collectable,
}
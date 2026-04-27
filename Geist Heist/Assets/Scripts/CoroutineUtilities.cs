/*
 * Contributors:  Toby
 * Creation Date: Feburary 2026
 * Last Modified: Feburary 2026
 * 
 * Static utilities class used to chain couroutines together.
 * Many of these functions are untested because I made this script on a whim.
 * This was made for a different project but its getting repurposed for GH.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class CoroutineUtilities 
{
    /*public static Coroutine StartCoroutine(IEnumerator coroutine)
    {
        return CoroutineRunner.StartCoroutine(coroutine);
    }*/

    /// <summary>
    /// Starts all coroutines at the same time. waits for them all to be done.
    /// </summary>
    /// <param name="coroutines">Coroutines to play at the same time</param>
    /// <returns>Awaitable coroutine that becomes null when all coroutines are done playing</returns>
    public static Coroutine PlayCoroutines(ICollection<IEnumerator> coroutines) // alternative name: WaitForCoroutines
    {
        return CoroutineRunner.StartCoroutine(StartAndWaitForCoroutines(coroutines));
    }

    /// <summary>
    /// Plays coroutines one after another.
    /// </summary>
    /// <param name="coroutineSequence">Coroutines to play in order</param>
    /// <returns>Awaitable coroutine that becomes null when all coroutineSequence are done playing</returns>
    public static Coroutine StartSequence(ICollection<IEnumerator> coroutineSequence)
    {
        return CoroutineRunner.StartCoroutine(CreateCoroutineSequence(coroutineSequence));
    }

    /// <summary>
    /// Plays coroutine at the same time as coroutine. Waits for both coroutines to be finished.
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="coroutine">Coroutine to start</param>
    /// <returns>Awaitable coroutine that becomes null when all coroutines are done playing</returns>
    public static Coroutine And(this Coroutine currentCoroutine, IEnumerator coroutine)
    {
        return CoroutineRunner.StartCoroutine(StartAndWaitForCoroutines(currentCoroutine, coroutine));
    }

    /// <summary>
    /// Plays coroutines at the same time as coroutine. Waits for both coroutines to be finished.
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="coroutines">Coroutines to start</param>
    /// <returns>Awaitable coroutine that becomes null when all coroutines are done playing</returns>
    public static Coroutine And(this Coroutine currentCoroutine, ICollection<IEnumerator> coroutines)
    {
        return CoroutineRunner.StartCoroutine(StartAndWaitForCoroutines(currentCoroutine, coroutines));
    }

    /// <summary>
    /// Plays coroutine at the same time as coroutine. Waits for both coroutines to be finished.
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="coroutine">Coroutine to start</param>
    /// <returns>Awaitable coroutine that becomes null when all coroutines are done playing</returns>
    public static Coroutine And(this Coroutine currentCoroutine, Coroutine coroutine)
    {
        return CoroutineRunner.StartCoroutine(StartAndWaitForCoroutines(currentCoroutine, coroutine));
    }

    /// <summary>
    /// Starts coroutine after coroutines is completed
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="coroutine">Coroutine to start</param>
    /// <returns>Awaitable coroutine that becomes null when previous coroutine and new coroutine are done playing</returns>
    public static Coroutine Then(this Coroutine currentCoroutine, IEnumerator coroutine)
    {
        return CoroutineRunner.StartCoroutine(CombineToSequence(currentCoroutine, coroutine));
    }

    /// <summary>
    /// Starts coroutines at the same time after currentCoroutine is completed
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="coroutines">Coroutines to start at the same time</param>
    /// <returns>Awaitable coroutine that becomes null when previous coroutine and new coroutines are done playing</returns>
    public static Coroutine Then(this Coroutine currentCoroutine, ICollection<IEnumerator> coroutines)
    {
        return CoroutineRunner.StartCoroutine(CombineToSequence(currentCoroutine, coroutines));
    }

    /// <summary>
    /// Starts coroutine after coroutines is completed
    /// </summary>
    /// <param name="currentCoroutine">Coroutine already started</param>
    /// <param name="action">Action to execute after</param>
    /// <returns>Awaitable coroutine that becomes null when previous coroutine and action are done executing</returns>
    public static Coroutine Then(this Coroutine currentCoroutine, UnityAction action)
    {
        return CoroutineRunner.StartCoroutine(ExecuteActionAfter(currentCoroutine, action));
    }

    public static Coroutine ThenStartSequence(this Coroutine currentCoroutine, ICollection<IEnumerator> coroutineSequence)
    {
        return CoroutineRunner.StartCoroutine(
                CombineToSequence(currentCoroutine, CreateCoroutineSequence(coroutineSequence))
            );
    }

    #region Private IEnumerators

    private static IEnumerator StartAndWaitForCoroutines(ICollection<IEnumerator> coroutines)
    {
        Stack<Coroutine> coroutinesPlaying = new Stack<Coroutine>();

        foreach (IEnumerator coroutine in coroutines)
        {
            var coroutineInstance = CoroutineRunner.StartCoroutine(coroutine);
            coroutinesPlaying.Push(coroutineInstance);
        }

        yield return WaitForCoroutinesToFinish(coroutinesPlaying);
    }

    private static IEnumerator StartAndWaitForCoroutines(Coroutine existingCoroutine, IEnumerator newCoroutine)
    {
        Stack<Coroutine> coroutinesPlaying = new Stack<Coroutine>();
        coroutinesPlaying.Push(existingCoroutine);

        var coroutineInstance = CoroutineRunner.StartCoroutine(newCoroutine);
        coroutinesPlaying.Push(coroutineInstance);

        yield return WaitForCoroutinesToFinish(coroutinesPlaying);
    }

    private static IEnumerator StartAndWaitForCoroutines(Coroutine existingCoroutine, Coroutine coroutine)
    {
        Stack<Coroutine> coroutinesPlaying = new Stack<Coroutine>();
        coroutinesPlaying.Push(existingCoroutine);

        coroutinesPlaying.Push(coroutine);

        yield return WaitForCoroutinesToFinish(coroutinesPlaying);
    }

    private static IEnumerator StartAndWaitForCoroutines(Coroutine existingCoroutine, ICollection<IEnumerator> coroutines)
    {
        Stack<Coroutine> coroutinesPlaying = new Stack<Coroutine>();
        coroutinesPlaying.Push(existingCoroutine);

        foreach (IEnumerator coroutine in coroutines)
        {
            var coroutineInstance = CoroutineRunner.StartCoroutine(coroutine);
            coroutinesPlaying.Push(coroutineInstance);
        }

        yield return WaitForCoroutinesToFinish(coroutinesPlaying);
    }

    private static IEnumerator CombineToSequence(Coroutine existingCoroutine, IEnumerator newCoroutine)
    {
        // wait for existingCoroutine to finish
        yield return existingCoroutine;

        yield return newCoroutine;
    }

    private static IEnumerator CombineToSequence(Coroutine existingCoroutine, ICollection<IEnumerator> newConcurrentCoroutines)
    {
        // wait for existingCoroutine to finish
        yield return existingCoroutine;

        yield return PlayCoroutines(newConcurrentCoroutines);
    }

    private static IEnumerator ExecuteActionAfter(Coroutine existingCoroutine, UnityAction action)
    {
        // wait for existingCoroutine to finish
        yield return existingCoroutine;

        if(action != null) action();
    }

    /// <summary>
    /// Play coroutineSequence one after another
    /// </summary>
    private static IEnumerator CreateCoroutineSequence(ICollection<IEnumerator> coroutineSequence)
    {
        foreach(IEnumerator coroutine in coroutineSequence)
        {
            // wait for coroutine to finish
            yield return coroutine; 
        }
    }

    private static IEnumerator WaitForCoroutinesToFinish(Stack<Coroutine> coroutinesPlaying)
    {
        while (coroutinesPlaying.Count > 0)
        {
            // only check the top coroutine. if ANY coroutine is running, then the status of the other ones doesnt matter.
            var topCoroutine = coroutinesPlaying.Pop();


            // this is safe and probably works
            /*if (topCoroutine == null)
                coroutinesPlaying.Pop();
            else
                yield return null;*/

            // but this way is cooler
            if (topCoroutine != null)
                yield return topCoroutine;
            coroutinesPlaying.Pop();
        }
    }

    #endregion

    #region Coroutine Runner Initialization
    public static StaticUtilitiesCoroutineRunner CoroutineRunner => GetCoroutineRunner();
    private static StaticUtilitiesCoroutineRunner _coroutineRunner;

    private static StaticUtilitiesCoroutineRunner GetCoroutineRunner()
    {
        // if no coroutine runner in scene, make one
        if (_coroutineRunner == null)
        {
            var coroutineGameobject = new GameObject();
            GameObject.DontDestroyOnLoad(coroutineGameobject);
            coroutineGameobject.name = "Static Utilities Coroutine Runner";
            _coroutineRunner = coroutineGameobject.AddComponent<StaticUtilitiesCoroutineRunner>();
        }

        return _coroutineRunner;
    }
    #endregion
}


public class StaticUtilitiesCoroutineRunner : MonoBehaviour
{
    // doesnt need to do anything besides exist
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}

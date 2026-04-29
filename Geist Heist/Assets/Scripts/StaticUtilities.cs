/*
 * Contributors: Toby S, Sky B, Cade Naylor, Jay Embry
 * Creation Date: ???
 * Last Modified: 1/27/2026
 * 
 * Brief Description: General use utility functions that can be
 * applied to any project. 
 * This script has been used in projects past and it will be used
 * in projects future.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public static class StaticUtilities
{
    private static StaticUtilitiesCoroutineRunner CoroutineRunner => CoroutineUtilities.CoroutineRunner;

    #region Gameplay

    /// <summary>
    /// Most commonly used to transform player input (WASD) to 3D input, relative to the camera
    /// </summary>
    /// <param name="inputDirection">2D player input (WASD)</param>
    /// <param name="referencePoint">Usually the camera</param>
    /// <returns>Transformed Input Direction</returns>
    public static Vector3 TransformInputDirection(Vector2 inputDirection, Transform referencePoint)
    {
        return 
            ( referencePoint.forward * inputDirection.y 
            + referencePoint.right * inputDirection.x)
            .normalized;
    }


    #endregion

    #region Components

    // Stole ts from the internet
    public static T CopyComponent<T>(this T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        
        Component copy = destination.GetOrAddComponent<T>();

        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }

        // Get all properties
        PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        foreach (PropertyInfo property in properties)
        {
            if (property.CanWrite)
            {
                property.SetValue(copy, property.GetValue(original));
            }
        }

        return copy as T;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component
    {
        component = gameObject.GetComponentInChildren<T>();
        return component != null;
    }

    #endregion

    #region VFX

    /// <summary>
    /// Instiates a particle system, and destroys it after its done playing.
    /// If a particle is set to loop, it will play forever
    /// </summary>
    public static GameObject PlayAndDestroyParticle(GameObject particleSystemPrefab, Vector3 position, Vector3? scale=null, Quaternion? rotation=null)
    {
        if(particleSystemPrefab == null) return null;
        scale = scale ?? Vector3.one;
        rotation = rotation ?? Quaternion.identity;

        // Destroy
        var ps = particleSystemPrefab.GetComponentInChildren<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("Tried to spawn a particle, but no ParticleSystem was attached");
            return null;
        }

        // Build
        var particleGameObject = GameObject.Instantiate(particleSystemPrefab, position, rotation.Value);
        if(!ps.main.playOnAwake)
            ps.Play();


        // Destroy
        if (!ps.main.loop)
        {
            float timeToDestroy = Mathf.Max(ps.main.startLifetime.constantMax, ps.main.duration);
            GameObject.Destroy(particleGameObject, timeToDestroy);
        }
        return particleGameObject;
    }

    #endregion

    #region Animations

    /// <summary>
    /// Smooth transform's current scale to endScale;
    /// </summary>
    public static Coroutine AnimateScale(Transform transform, Vector3 endScale, float seconds,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(AnimateScaleCoroutine(transform, transform.localScale, endScale, seconds, unscaledTime, currentCoroutineToCancel));
    }

    /// <summary>
    /// Smooths the transforms scale from startScale to endScale;
    /// </summary>
    public static Coroutine AnimateScale(Transform transform, Vector3 startScale, Vector3 endScale, float seconds,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(AnimateScaleCoroutine(transform, startScale, endScale, seconds, unscaledTime, currentCoroutineToCancel));
    }

    private static IEnumerator AnimateScaleCoroutine(Transform transform, Vector3 startScale, Vector3 endScale, float seconds,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        float startTime = unscaledTime ? Time.unscaledTime : Time.time;
        float time = startTime;
        while (time - startTime < seconds)
        {
            time = unscaledTime ? Time.unscaledTime : Time.time;
            float t = (time - startTime) / seconds;

            if (transform == null)
                yield break;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);

            yield return null;
        }
        // apply one more time just in case.
        transform.localScale = endScale;
    }

    /// <summary>
    /// Smooth current rotation towards endEulerAngles;
    /// </summary>
    public static Coroutine AnimateRotation(Transform transform, Vector3 endRotation, float seconds,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(
            AnimateRotationCoroutine(transform, transform.rotation, Quaternion.Euler(endRotation), seconds, 
                                     unscaledTime, currentCoroutineToCancel)
        );
    }

    /// <summary>
    /// Smooth current rotation towards endRotation;
    /// </summary>
    public static Coroutine AnimateRotation(Transform transform, Quaternion endRotation, float seconds,
        Quaternion? startRotation = null,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        startRotation = startRotation ?? transform.rotation;

        return CoroutineRunner.StartCoroutine(
            AnimateRotationCoroutine(transform, startRotation.Value, endRotation, seconds,
                                     unscaledTime, currentCoroutineToCancel)
        );
    }

    private static IEnumerator AnimateRotationCoroutine(Transform transform, Quaternion startRotation, Quaternion endRotation, float seconds,
        bool unscaledTime = true, Coroutine currentCoroutineToCancel = null)
    {
        float startTime = unscaledTime ? Time.unscaledTime : Time.time;
        float time = startTime;
        while (time - startTime < seconds)
        {
            time = unscaledTime ? Time.unscaledTime : Time.time;
            float t = (time - startTime) / seconds;

            transform.localRotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }
        // apply one more time just in case.
        transform.localRotation = endRotation;
    }

    #endregion

    #region Coroutines

    public static void StopAndStartCoroutine(ref Coroutine coroutineInstance, IEnumerator coroutineToPlay)
    {
        if (coroutineInstance != null)
            CoroutineRunner.StopCoroutine(coroutineInstance);

        coroutineInstance = CoroutineRunner.StartCoroutine(coroutineToPlay);
    }

    public static void StartCoroutineIfNotPlaying(ref Coroutine coroutineInstance, IEnumerator coroutineToPlay)
    {
        if (coroutineInstance == null)
            coroutineInstance = CoroutineRunner.StartCoroutine(coroutineToPlay);
    }

    #endregion

    #region UI

    public static void ToggleCanvasGroup(CanvasGroup canvasgroup, bool enabled, float? alpha = null, bool? ignoreParentGroups = null)
    {
        if (enabled)
            EnableCanvasGroup(canvasgroup, alpha: alpha, ignoreParentGroups: ignoreParentGroups);
        else
            DisableCanvasGroup(canvasgroup, ignoreParentGroups: ignoreParentGroups);
    }
    public static void EnableCanvasGroup(CanvasGroup canvasgroup, float? alpha = null, bool interactable = true, bool blocksRaycasts = true, bool? ignoreParentGroups = null)
    {
        canvasgroup.alpha = alpha ?? 1;
        canvasgroup.interactable = interactable;
        canvasgroup.blocksRaycasts = blocksRaycasts;
        canvasgroup.ignoreParentGroups = ignoreParentGroups ?? canvasgroup.ignoreParentGroups;
    }

    public static void DisableCanvasGroup(CanvasGroup canvasgroup, float? alpha = null, bool? ignoreParentGroups = null)
    {
        canvasgroup.alpha = alpha ?? 0;
        canvasgroup.interactable = false;
        canvasgroup.blocksRaycasts = false;
        canvasgroup.ignoreParentGroups = ignoreParentGroups ?? canvasgroup.ignoreParentGroups;
    }

    public static void ShowCursor()
    {
        UnityEngine.Cursor.visible = true;
        // Free mouse if editor, locked to window if in a build. For the sake of debugging because oh my god
        UnityEngine.Cursor.lockState = Application.isEditor ? CursorLockMode.None : CursorLockMode.Confined;
    }

    public static void HideCursor()
    {
        UnityEngine.Cursor.visible = false;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
    }

    public static Coroutine FadeToVisible(CanvasGroup group, float seconds, bool unscaledTime = true,
        UnityAction afterFadeCallback = null, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(FadeOpacityCoroutine(group, start_a: group.alpha, target_a: 1, seconds: seconds, unscaledTime: unscaledTime,
            afterFadeCallback: afterFadeCallback));
    }

    public static Coroutine FadeToHidden(CanvasGroup group, float seconds, bool unscaledTime = true,
        UnityAction afterFadeCallback = null, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(FadeOpacityCoroutine(group, start_a: group.alpha, target_a: 0, seconds: seconds, 
            afterFadeCallback: afterFadeCallback, unscaledTime: unscaledTime));
    }

    public static Coroutine FadeOpacity(CanvasGroup group, float start_a, float target_a, float seconds,
        bool unscaledTime = true, UnityAction afterFadeCallback = null, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        return CoroutineRunner.StartCoroutine(FadeOpacityCoroutine(group, start_a: start_a, target_a: target_a, seconds: seconds, 
            afterFadeCallback: afterFadeCallback, unscaledTime: unscaledTime));
    }

    public static Coroutine FadeOpacityBySpeed(CanvasGroup group, float start_a, float end_a, float alpha_perSecond,
       bool unscaledTime = true, UnityAction afterFadeCallback = null, Coroutine currentCoroutineToCancel = null)
    {
        if (currentCoroutineToCancel != null)
            CoroutineRunner.StopCoroutine(currentCoroutineToCancel);

        float diff = Mathf.Abs(end_a - start_a);
        float seconds = diff / alpha_perSecond;

        return CoroutineRunner.StartCoroutine(FadeOpacityCoroutine(group, start_a: start_a, target_a: end_a, seconds: seconds, afterFadeCallback: afterFadeCallback, unscaledTime: unscaledTime));
    }

    private static IEnumerator FadeOpacityCoroutine(CanvasGroup group, float start_a, float target_a, float seconds, UnityAction afterFadeCallback = null, 
        bool unscaledTime = true)
    {
        float startTime = unscaledTime ? Time.unscaledTime : Time.time;
        float time = startTime;
        while (time - startTime < seconds)
        {
            if (group == null)
                yield break;

            time = unscaledTime ? Time.unscaledTime : Time.time;
            float t = (time - startTime) / seconds;

            group.alpha = Mathf.Lerp(start_a, target_a, t);

            yield return null;
        }
        if(group == null)
            yield break;    

        // apply one more time just in case.
        group.alpha = target_a;

        if (afterFadeCallback != null)
            afterFadeCallback();
    }


    /// <summary>
    /// Sets the colors of a selectable ui component.
    /// All color parameters are optional, so only set the ones you need to update.
    /// </summary>
    /// <param name="uiComponent"></param>
    public static void SetColors(this Selectable uiComponent,
        Color? normalColor = null, Color? highlightedColor = null, Color? pressedColor = null, Color? selectedColor = null, Color? disabledColor = null)
    {
        var colors = uiComponent.colors;
        colors.normalColor = normalColor ?? colors.normalColor;
        colors.highlightedColor = highlightedColor ?? colors.highlightedColor;
        colors.selectedColor = selectedColor ?? colors.selectedColor;
        colors.pressedColor = pressedColor ?? colors.pressedColor;
        colors.disabledColor = disabledColor ?? colors.disabledColor;
        uiComponent.colors = colors;
    }

    public static Vector3 WorldPosition(this RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);
        return corners.Average();
    }

    #endregion

    #region Transform

    /// <summary>
    /// Rotates the transform so the forward vector points AWAY from worldPosition
    /// </summary>
    public static void LookAway(this Transform transform, Vector3 worldPosition)
    {
        // Weird equation but it does indeed make it look away
        transform.LookAt(2 * transform.position - worldPosition);
    }

    /// <summary>
    /// Rotates the transform so the forward vector points AWAY from target
    /// </summary>
    public static void LookAway(this Transform transform, Transform target)
    {
        // Calls the other function
        transform.LookAway(target.position);
    }

    public static void ScaleOverTime(Transform transform, Vector3 targetScale, float seconds, bool unscaledTime = true)
    {
        ScaleOverTime(transform, transform.localScale, targetScale, seconds);
    }

    public static void ScaleOverTime(Transform transform, Vector3 startScale, Vector3 targetScale, float seconds, bool unscaledTime = true)
    {
        CoroutineRunner.StartCoroutine(ScaleOverTimeCoroutine(transform, startScale, targetScale, seconds, unscaledTime));
    }

    private static IEnumerator ScaleOverTimeCoroutine(Transform transform, Vector3 startScale, Vector3 targetScale, float seconds, bool unscaledTime)
    {
        float startTime = unscaledTime ? Time.unscaledTime : Time.time;
        float time = startTime;
        while (time - startTime < seconds)
        {
            time = unscaledTime ? Time.unscaledTime : Time.time;
            float t = (time - startTime) / seconds;

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }
        // apply one more time just in case.
        transform.localScale = targetScale;
    }

    #endregion

    #region Vectors

    public static Vector3 Average(this Vector3[] vectors)
    {
        Vector3 total = Vector3.zero;
        foreach (var v in vectors)
        {
            total += v;
        }
        return total / vectors.Length;
    }

    /// <summary>
    /// Get the smallest value in a vector
    /// </summary>
    public static float Min(this Vector3 vector)
    {
        return Mathf.Min(Mathf.Min(vector.x, vector.y), vector.z);
    }

    /// <summary>
    /// Get the largest value in a vector
    /// </summary>
    public static float Max(this Vector3 vector)
    {
        return Mathf.Max(Mathf.Max(vector.x, vector.y), vector.z);
    }

    /// <summary>
    /// Return vector with x value changed
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 WithX(this Vector3 vector, float x)
    {
        vector.x = x;
        return vector;
    }

    /// <summary>
    /// Return vector with y value changed
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 WithY(this Vector3 vector, float y)
    {
        vector.y = y;
        return vector;
    }

    /// <summary>
    /// Return vector with z value changed
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    public static Vector3 WithZ(this Vector3 vector, float z)
    {
        vector.z = z;
        return vector;
    }

    #endregion

    #region Math

    /// <summary>
    /// Returns the positive distance between a and b.
    /// </summary>
    public static float Difference(float a, float b)
    {
        return Mathf.Abs(a - b);
    }

    /// <summary>
    /// Returns inverse lerp (t), where t may be less than 0 or greater than 1
    /// </summary>
    public static float InverseLerpUnclamped(float a, float b, float value)
    {
        if (a != b)
        {
            return (value - a) / (b - a);
        }

        return 0f;
    }

    public static float InverseLerpAngle(float a, float b, float value)
    {
        // this is an AWFUL way to do this bro 
        while (a < 0 || b < 0 || value < 0)
        {
            a += 180;
            b += 180;
            value += 180;
        }

        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);
        value = Mathf.Repeat(value, 360);

        //Debug.Log($"a: {Mathf.Round(a)} b: {Mathf.Round(b)} value:{Mathf.Round(value)} t: {Mathf.Round(Mathf.InverseLerp(a, b, value) * 100) / 100}");

        return Mathf.InverseLerp(a, b, value);
    }

    public static float InverseLerpAngleUnclamped(float a, float b, float value)
    {
        // this is an AWFUL way to do this bro 
        while (a < 0 || b < 0 || value < 0)
        {
            a += 180;
            b += 180;
            value += 180;
        }

        a = Mathf.Repeat(a, 360);
        b = Mathf.Repeat(b, 360);
        value = Mathf.Repeat(value, 360);

        //Debug.Log($"a: {Mathf.Round(a)} b: {Mathf.Round(b)} value:{Mathf.Round(value)} t: {Mathf.Round(InverseLerpUnclamped(a, b, value) * 100) / 100}");

        return InverseLerpUnclamped(a, b, value);
    }

    /// <summary>
    /// Sin clamped between 0 and 1 (instead of -1 and 1)
    /// </summary>
    public static float Sin01(float x)
    {
        return (MathF.Sin(x) + 1) / 2;
    }

    /// <summary>
    /// Sin clamped between a and b (instead of -1 and 1)
    /// </summary>
    public static float SinRange(float x, float a, float b)
    {
        return Mathf.Lerp(a, b, Sin01(x));
    }

    /// <summary>
    /// Cos clamped between 0 and 1 (instead of -1 and 1)
    /// </summary>
    public static float Cos01(float x)
    {
        return (MathF.Cos(x) + 1) / 2;
    }

    /// <summary>
    /// Cos clamped between a and b (instead of -1 and 1)
    /// </summary>
    public static float CosRange(float x, float a, float b)
    {
        return Mathf.Lerp(a, b, Cos01(x));
    }

    public static float RoundToHundreth(float x)
    {
        return Mathf.Round(x * 100) / 100;
    }

    #endregion

    #region Lists

    /// <summary>
    /// Shuffles selected list
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    { //ty stack exchange <3
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

    /// <summary>
    /// Combines two arrays of any type
    /// </summary>
    /// <typeparam name="T">Variable type for arrays</typeparam>
    /// <param name="arr1">The first array</param>
    /// <param name="arr2">The second array</param>
    /// <returns>The combined array, with elements from array 1 first</returns>
    public static T[] AddArrays<T>(T[] arr1, T[] arr2)
    {
        int index = 0;
        T[] result = new T[arr1.Length + arr2.Length];
        for (int i = 0; i < arr1.Length - 1; i++)
        {
            result[index] = arr1[i];
            index++;
        }
        for (int i = 0; i < arr2.Length - 1; i++)
        {
            result[index] = arr2[i];
            index++;
        }
        return result;
    }

    /// <summary>
    /// Converts a list of any type into an array
    /// </summary>
    /// <typeparam name="T">The data type</typeparam>
    /// <param name="list">The list to be converted</param>
    /// <returns>The list in array form</returns>
    public static T[] ListToArray<T>(List<T> list)
    {
        T[] result = new T[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            result[i] = list.ElementAt(i);
        }
        return result;
    }

    public static bool IsNullOrEmpty<T>(this ICollection<T> array)
    {
        if (array == null) return true;
        if (array.Count == 0) return true;
        return false;
    }

    public static bool IsEmptyOrNull<T>(this string str)
    {
        if (str == null) return true;
        if (str.Length == 0) return true;
        return false;
    }



    #endregion

    #region Linq

    public static void ForEach<T>(this IEnumerable<T> source, UnityAction<T> action)
    {
        //source.ThrowIfNull("source");
        //action.ThrowIfNull("action");
        foreach (T element in source)
        {
            action(element);
        }
    }

    #endregion

    #region Dictionaries

    /// <summary>
    /// Returns first instance of a key that found.
    /// If duplicate values exist in the dictionary, an unpredictable key may be returned.
    /// </summary>
    public static T1 GetFirstKeyByValue<T1, T2>(this Dictionary<T1, T2> dictionary, T2 value)
    {
        return dictionary.Keys
            .Where(k => dictionary[k].Equals(value))
            .First();
    }

    public static void RemoveAllInstancesWithValue<T1, T2>(this Dictionary<T1, T2> dictionary, T2 value)
    {
        var keysToRemove = dictionary.Keys.Where(k => dictionary[k].Equals(value)).ToList();

        foreach (var key in keysToRemove)
        {
            dictionary.Remove(key);
        }
    }

    #endregion

    #region Stacks

    public static void PushMultiple<T>(this Stack<T> stack, IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            stack.Push(value);
        }
    }

    #endregion

    #region Strings

    /// <summary>
    /// Returns the first word of a sentence.
    /// ex: "This is a sentence" => "This"
    /// </summary>
    public static string FirstWord(string str, char sperationCharacter = ' ')
    {
        int index = str.IndexOf(sperationCharacter);
        return index == -1 ? str : str.Substring(0, index);
    }

    /// <summary>
    /// Returns the last word of a sentence.
    /// ex: "This is a sentence" => "sentence"
    /// </summary>
    public static string LasttWord(string str, char sperationCharacter = ' ')
    {
        int index = str.LastIndexOf(sperationCharacter);
        return index == -1 ? str : str.Substring(index, str.Length - index - 1);
    }

    #endregion

    #region Color

    public static string ToHex(this Color color)
    {
        return ColorUtility.ToHtmlStringRGB(color);
    }

    #endregion

    #region Scenes


    public static string BuildIndexToSceneName(int buildIndex)
    {
        string path = SceneUtility.GetScenePathByBuildIndex(buildIndex);
        return Path.GetFileNameWithoutExtension(path);
    }

    #endregion

    #region Debug

    /// <summary>
    /// (Editor only) Returns true if the user is selecting parent, or any of its children
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static bool Editor_SelectingSelfOrChild(Transform parent)
    {
#if UNITY_EDITOR

        var selected = UnityEditor.Selection.activeTransform;
        return selected != null && (selected == parent || selected.IsChildOf(parent));
#else
        return false;
#endif
    }

    /// <summary>
    /// (Editor only) Returns true if the user is selecting parent, or any of its children
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public static bool Editor_SelectingTransform(Transform transform)
    {
#if UNITY_EDITOR

        var selected = UnityEditor.Selection.activeTransform;
        return selected != null && selected == transform;
#else
        return false;
#endif
    }
    #endregion
}

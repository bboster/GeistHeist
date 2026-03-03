/*
 * Contributors: Toby
 * Creation Date: 3/3/2026
 * Last Modified: 3/3/2026
 * 
 * Brief Description: Data model that stores dialogue text and associated audio.
 */

using NaughtyAttributes;
using UnityEngine;

[System.Serializable]
public class DialogueTextData
{
    [ResizableTextArea]
    public string BodyText;

    public float SecondsDelayUntilNextDialogue = 0.1f;

    [Tooltip("How long to wait until destroying itself")]
    public float StayLength = 3;

    [Header("Audio")]
    public int audioLine;
}

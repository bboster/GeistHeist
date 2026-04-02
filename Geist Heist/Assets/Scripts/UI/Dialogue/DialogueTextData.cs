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

    public DialogueSpeaker dialogueSpeaker;

    public float SecondsDelayUntilNextDialogue = 0.1f;

    [Tooltip("How long to wait until destroying itself")]
    public float StayLength = 3;

    [Header("Audio")]
    public int audioLine = -1;

    private EColor debugColor => (dialogueSpeaker == DialogueSpeaker.Ollie) ? EColor.Blue : EColor.Orange;
}

public enum DialogueSpeaker
{
    PASystem,
    Ollie,
}

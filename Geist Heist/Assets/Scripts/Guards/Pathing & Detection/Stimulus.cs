/*
 * Author: Jacob Bateman
 * Contributors:
 * Creation: 10/02/25
 * Last Edited: 10/02/25
 * Summary: Base class for all stimuli
 */

using GuardUtilities;
using UnityEngine;
using NaughtyAttributes;

public abstract class Stimulus : MonoBehaviour
{

    public enum StimulusType { sight, sound }
    [Header("Stimulus Values")]
    [Tooltip("The type of sense this is")]
    [SerializeField] protected StimulusType stimulusType;


    [Tooltip("The priority the stimulus has compared to other stimuli and behaviors")]
    [SerializeField] protected Priority priority;

    [Tooltip("The state this stimulus should send the guard to")]
    [SerializeField] protected GuardStates stateToChangeTo;

    public abstract void TriggerStimulus();

    /// <summary>
    /// Returns the priority of the stimulus
    /// </summary>
    /// <returns></returns>
    public Priority GetPriority()
    {
        return priority;
    }

    /// <summary>
    /// Returns the type of the stimulus (vision, sound, etc...)
    /// </summary>
    /// <returns></returns>
    public StimulusType GetStimulusType()
    {
        return stimulusType;
    }
}

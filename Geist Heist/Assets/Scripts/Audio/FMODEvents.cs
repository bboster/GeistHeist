using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections.Generic;

//[CreateAssetMenu(fileName = "New Sound Event Bank", menuName = "Database/Create New Sound Event Bank"), System.Serializable]
public class FMODEvents : MonoBehaviour
{
    //One list of events for each Bank in FMOD Studio
    //This is to choose the only the necessary banks in a scene to help with loading, not all sound effects need to be loaded on the main menu
    public List<SoundEvent> BGMBank;
    public List<SoundEvent> EnemyBank;
    public List<SoundEvent> EnvironmentBank;
    public List<SoundEvent> InterfaceBank;
    public List<SoundEvent> PlayerBank;

    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("There is more than one FMODEvents in the scene");
        }
        instance = this;
    }
}

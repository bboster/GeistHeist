using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class GlobeEndActionable : MonoBehaviour, IActionable
{
    [SerializeField] private CinemachineCamera globeCam;
    [SerializeField] private int endingButtonPresses = 10;
    private Coroutine endCoroutine;

    public void Action()
    {
        Debug.Log("test");

        if (globeCam.Priority == 0)
        {
            globeCam.Priority++;
        }

        if (endCoroutine == null)
        {
            endCoroutine = StartCoroutine(ButtonPressMinigame());
        }
    }

    public IEnumerator ButtonPressMinigame()
    {
        while (true)
        {
            if (endingButtonPresses <= 0)
            {
                //initiate ending cutscene
            }

            yield return null;
        }
    }

    bool IActionable.IsActionable()
    {
        if (SaveDataManager.Instance.AllLevelsCompleted())
        {
            return true;
        }

        return false;
    }
    
}

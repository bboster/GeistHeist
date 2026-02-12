using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;

public class GlobeEndActionable : MonoBehaviour, IActionable
{
    [SerializeField] private CinemachineCamera globeCam;
    [SerializeField] private TMP_Text buttonPressText;
    public int endingButtonPresses = 10;
    private Coroutine endCoroutine;
    [HideInInspector] public bool EndingActive = false;
    private Animator animator => GetComponent<Animator>();
    [HideInInspector] public int currentButtonPresses = 0;

    public void Action()
    {
        Debug.Log("test");

        if (globeCam.Priority == 0)
        {
            globeCam.Priority++;
        }

        if (endCoroutine == null)
        {
            EndingActive = true;
            endCoroutine = StartCoroutine(ButtonPressMinigame());
        }
    }

    public IEnumerator ButtonPressMinigame()
    {
        buttonPressText.enabled = true;
        while (EndingActive)
        {
            if (currentButtonPresses > 0 && currentButtonPresses < 11)
            {
                buttonPressText.text = currentButtonPresses.ToString() + " / 10";
            }

            if (endingButtonPresses <= 0)
            {
                //initiate ending cutscene
                animator.SetBool("EndingStarted", true);
                EndingActive = false;
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

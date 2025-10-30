using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
/*
* Contributors: Brenden, Toby
* Creation Date: 10/1/25
* Last Modified: 10/29/25
* 
* Brief Description: Input Handler for the Vending Machine, handles movement and actions for the Vending Machine
*
* TODO: a lot of this code is copy pasted directly from ToyCar.cs
*/
[RequireComponent(typeof(PossessableObject))]
public class VendingObject : IInputHandler, IInteractable
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [SerializeField] private GameObject CanSpawnPoint;
    [SerializeField] private GameObject CanPrefab;

    private float currentStrength;

    /*[Dropdown("balancing")]*/[SerializeField] private float maxStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float minStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float strengthGrowthRate;
    /*[Dropdown("balancing")]*/[SerializeField] private float chargeLossRate;
    /*[Dropdown("balancing")]*/[SerializeField] private float tapStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private Vector3 launchDirection;
    /*[Dropdown("balancing")]*/[SerializeField] private bool Tap;

    [SerializeField] private PossessableChargeMeterUI chargeMeter;

    private PossessableObject possessableObject;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        possessableObject = GetComponent<PossessableObject>();
        if(chargeMeter == null)
            chargeMeter = GetComponentInChildren<PossessableChargeMeterUI>();   
    }

    public override void OnPossessionStart()
    {
        chargeMeter.OnPossessionStarted();
    }

    public override void OnPossessionEnded()
    {
    }

    public override void WhilePossessingUpdate()
    {
        chargeMeter.UpdateCharge(currentStrength, maxStrength);
        //Images.SetActive(false);
    }

    #region action
    public override void OnActionStarted()
    {
        Debug.Log("hiii");
        if (Tap)
        {
            GameObject temp;
            temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(launchDirection * tapStrength);
        }
    }

    public override void WhileActionHeld(float secondsHeld)
    {
        if (!Tap)
        {
            // Will be clamped later (dont clamp now for charge ui animations)
            currentStrength += strengthGrowthRate;
        }
    }

    public override void OnActionCanceled(float secondsHeld)
    {
        if (!Tap)
        {
            currentStrength = Mathf.Clamp(currentStrength, minStrength, maxStrength);

            GameObject temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            Vector3 tempLaunch = Vector3.Scale(launchDirection, CanSpawnPoint.transform.forward);
            tempLaunch.y = launchDirection.y;
            temp.GetComponent<Rigidbody>().AddForce(tempLaunch * currentStrength);
        }
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
        currentStrength = Mathf.Max(
            currentStrength - (Time.deltaTime * chargeLossRate),
            minStrength);
    }

    #endregion

    #region Interact
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf && possessableObject.CanUnPossess)
        {
            PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
        }
    }

    public override void WhileInteractHeld(float secondsHeld)
    { }
    

    public override void OnInteractCanceled(float secondsHeld)
    { }

    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld(float secondsHeld)
    {
    }

    public override void WhileMoveNotHeld()
    {

    }

    public override void OnMoveCanceled(float secondsHeld) { }

    #endregion

    void IInteractable.Interact()
    {
        //PlayerManager.Instance.PossessObject(GetComponent<PossessableObject>());
    }

   
}

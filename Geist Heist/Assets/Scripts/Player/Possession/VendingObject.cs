using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
/*
* Contributors: Brenden
* Creation Date: 10/1/25
* Last Modified: 10/1/25
* 
* Brief Description: Input Handler for the Vending Machine, handles movement and actions for the Vending Machine
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
    /*[Dropdown("balancing")]*/[SerializeField] private float tapStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private Vector3 launchDirection;
    /*[Dropdown("balancing")]*/[SerializeField] private bool Tap;

    [SerializeField] private Image ChargeUI;
    [SerializeField] private GameObject Images;

    private PossessableObject possessableObject;

    // Start is called once before the first execution of WhilePossessingUpdate after the MonoBehaviour is created
    void Start()
    {
        possessableObject = GetComponent<PossessableObject>();
    }

    public override void OnPossessionStart()
    {

    }

    public override void OnPossessionEnded()
    {

    }

    #region action
    public override void OnActionStarted()
    {
        if (Tap)
        {
            GameObject temp;
            temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(launchDirection * tapStrength);
        }
        else
        {
            currentStrength = minStrength;
            ChargeUI.fillAmount = (currentStrength - minStrength) / (maxStrength - minStrength);
            Images.SetActive(true);
        }
    }

    public override void WhileActionHeld()
    {
        if (!Tap)
        {
            currentStrength += strengthGrowthRate;
            if(currentStrength > maxStrength)
            {
                currentStrength = maxStrength;
            }
            ChargeUI.fillAmount = (currentStrength - minStrength) / (maxStrength - minStrength);
        }
    }

    public override void OnActionCanceled()
    {
        if (!Tap)
        {
            GameObject temp;
            temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            Vector3 tempLaunch = Vector3.Scale(launchDirection, CanSpawnPoint.transform.forward);
            tempLaunch.y = launchDirection.y;
            temp.GetComponent<Rigidbody>().AddForce(tempLaunch * currentStrength);
            Images.SetActive(false);
        }
    }

    #endregion

    #region Possess
    public override void OnInteractStarted()
    {
        if (thirdPersoncinemachineCamera.activeSelf)
        {
            PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
        }
    }

    public override void WhileInteractHeld()
    { }
    public override void WhileActionNotHeld()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnInteractCanceled()
    { }

    #endregion

    #region Move
    public override void OnMoveStarted()
    {

    }
    public override void WhileMoveHeld()
    {
    }

    public override void WhileMoveNotHeld()
    {

    }

    public override void OnMoveCanceled() { }

    #endregion

    void IInteractable.Interact()
    {
        PlayerManager.Instance.PossessObject(GetComponent<PossessableObject>());
    }

    
}

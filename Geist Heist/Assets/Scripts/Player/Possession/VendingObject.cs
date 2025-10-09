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
public class VendingObject : IInputHandler, IInteractable
{
    [SerializeField] private GameObject thirdPersoncinemachineCamera;
    [SerializeField] private GameObject CanSpawnPoint;
    [SerializeField] private GameObject CanPrefab;

    private float currentStrength;

    /*[Dropdown("balancing")]*/[SerializeField] private float MaxStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float MinStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float StrengthGrowthRate;
    /*[Dropdown("balancing")]*/[SerializeField] private float TapStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private Vector3 launchDirection;
    /*[Dropdown("balancing")]*/[SerializeField] private bool Tap;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public override void OnPossessionStarted()
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
            temp.GetComponent<Rigidbody>().AddForce(launchDirection * TapStrength);
        }
        else
        {
            currentStrength = MinStrength;
        }
    }

    public override void WhileActionHeld()
    {
        if (!Tap)
        {
            currentStrength += StrengthGrowthRate;
            if(currentStrength > MaxStrength)
            {
                currentStrength = MaxStrength;
            }
        }
    }

    public override void OnActionCanceled()
    {
        if (!Tap)
        {
            GameObject temp;
            temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(Vector3.Scale(launchDirection,transform.forward) * currentStrength);
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

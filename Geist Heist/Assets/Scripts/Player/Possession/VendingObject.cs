using NaughtyAttributes;
using System.Collections;
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
    //[SerializeField] private GameObject thirdPersoncinemachineCamera;
    [SerializeField] private Transform CanSpawnPoint;
    [SerializeField] private GameObject CanPrefab;

    private float currentStrength;

    /*[Dropdown("balancing")]*/[SerializeField] private float maxStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float minStrength;
    /*[Dropdown("balancing")]*/[SerializeField] private float strengthGrowthRate;
    /*[Dropdown("balancing")]*/[SerializeField] private float chargeLossRate;
    /*[Dropdown("balancing")]*/[SerializeField] private Vector3 launchDirection;
    /*[Dropdown("balancing")]*/[SerializeField] private bool Tap;
    [Tooltip("Force there to be time between can throws")]
    [SerializeField] private float delayBetweenThrows = 1f;
    [SerializeField, ShowIf(nameof(Tap))] private float tapStrength;

    [SerializeField] private float delayToUpdateChargeMeter = 0.25f;
    [Tooltip("How long it takes for the visible material to go back to possession material.")]
    [SerializeField] private float delayToUpdateMaterialVisibility = 0.5f;

    [SerializeField] private PossessableChargeMeterUI chargeMeter;

    private PossessableObject possessableObject;
    private bool hasThrownThisPossession;
    private Coroutine materialCountdownCoroutine;

    [SerializeField, Required] TrajectoryPredictor trajectoryPredictor;
    [SerializeField, Required] GameObject LineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    void Start()
    {
        possessableObject = GetComponent<PossessableObject>();
        if(chargeMeter == null)
            chargeMeter = GetComponentInChildren<PossessableChargeMeterUI>();   
    }

    public override void OnPossessionStart()
    {
        if (chargeMeter == null)
            chargeMeter = GetComponentInChildren<PossessableChargeMeterUI>();

        chargeMeter?.OnPossessionStarted();
        hasThrownThisPossession = false;
    }

    public override void OnPossessionEnded()
    {
        currentStrength = minStrength;
        hasThrownThisPossession = false;
        LineRenderer.SetActive(false);
    }

    public override void WhilePossessingUpdate()
    {
        chargeMeter.UpdateCharge(currentStrength, maxStrength);
        //Images.SetActive(false);
    }

    #region action
    public override void OnActionStarted()
    {
        if (Tap)
        {
            if (possessableObject.VisiblePossessionMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.VisiblePossessionMaterial;
            }

            GameObject temp;
            temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(launchDirection * tapStrength, ForceMode.Impulse);
            hasThrownThisPossession = true; 

        }
    }

    public override void WhileActionHeld(float secondsHeld)
    {
        if (secondsHeld < delayBetweenThrows && hasThrownThisPossession)
            return;

        LineRenderer.SetActive(true);
        if (!Tap)
        {
            // Will be clamped later (dont clamp now for charge ui animations)
            currentStrength += Time.deltaTime * strengthGrowthRate;
            Vector3 tempLaunch = Vector3.Scale(launchDirection, CanSpawnPoint.transform.forward);
            tempLaunch.y = launchDirection.y;
            trajectoryPredictor.PredictTrajectory(Mathf.Clamp(currentStrength, minStrength, maxStrength), CanPrefab.GetComponent<Rigidbody>().mass, CanSpawnPoint.transform.forward, CanSpawnPoint.transform.position, CanPrefab.GetComponent<Rigidbody>().linearDamping, .025f);
        }
    }

    public override void OnActionCanceled(float secondsHeld)
    {
        if (secondsHeld < delayBetweenThrows && hasThrownThisPossession)
            return;

        if (!Tap)
        {
            LineRenderer.SetActive(false);
            currentStrength = Mathf.Clamp(currentStrength, minStrength, maxStrength);

            GameObject temp = Instantiate(CanPrefab, CanSpawnPoint.transform.position, Quaternion.identity);
            temp.GetComponent<Rigidbody>().AddForce(CanSpawnPoint.transform.forward * currentStrength);
            hasThrownThisPossession = true;

            if (possessableObject.VisiblePossessionMaterial != null)
            {
                possessableObject.meshRenderer.material = possessableObject.VisiblePossessionMaterial;
            }
        }

        if (possessableObject.PossessedMaterial != null)
        {
            if (materialCountdownCoroutine == null)
            {
                materialCountdownCoroutine = StartCoroutine(MaterialReplaceCountdown());
            }
        }

        PossessableObject.OnActionPerformed?.Invoke();
    }

    public override void WhileActionNotHeld(float secondsNotHeld)
    {
        if (secondsNotHeld < delayToUpdateChargeMeter)
            return;

        currentStrength = Mathf.Max(
            currentStrength - (Time.deltaTime * chargeLossRate),
            minStrength);
    }

    #endregion

    #region Interact
    public override void OnInteractStarted()
    {
        if (materialCountdownCoroutine != null)
        {
            StopCoroutine(materialCountdownCoroutine);
            materialCountdownCoroutine = null;
        }

        PlayerManager.Instance.PossessGhost(gameObject.transform.GetComponent<PossessableObject>());
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

    public void OnDrawGizmos()
    {
        Gizmos.DrawRay(CanSpawnPoint.position, CanSpawnPoint.forward);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(CanSpawnPoint.position, 
            Vector3.Scale(launchDirection, CanSpawnPoint.transform.forward)
            .WithY(launchDirection.y)
        );
    }

    public void OnDrawGizmosSelected()
    {
        // ok guys i got distracted but i still wanna finish this l8r
        /*Gizmos.color = Color.green;
        Vector3 tempLaunch = Vector3.Scale(launchDirection, CanSpawnPoint.transform.forward)
            .WithY(launchDirection.y);
        Gizmos.DrawRay(CanSpawnPoint.position, tempLaunch * maxStrength);

        float angle = Mathf.Atan2(launchDirection.y, launchDirection.x);
        float prevy = 0;
        for(int i = 1; i< 10; i++)
        {
            float y = tempLaunch.y * Mathf.Sin(angle) - (0.5f * -Physics.gravity.y * Mathf.Pow(i, 2));

            Gizmos.DrawLine(
                CanSpawnPoint.position + new Vector3(tempLaunch.x * (i - 1f), prevy, tempLaunch.z * (i - 1f)),
                CanSpawnPoint.position + new Vector3(tempLaunch.x * i, y, tempLaunch.z * i)
                );
            prevy = y;
        } */
    }

    /// <summary>
    /// Counts down and replaces visible material of the vending machine with the possessed material
    /// </summary>
    /// <returns></returns>
    public IEnumerator MaterialReplaceCountdown()
    {
        while (true)
        {
            yield return new WaitForSeconds(delayToUpdateMaterialVisibility);
            possessableObject.meshRenderer.material = possessableObject.PossessedMaterial;
            materialCountdownCoroutine = null;
            break;
        }
        yield return null;
    }
}

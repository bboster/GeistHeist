/*
 * Contributors: Toby Schamberger
 * Creation Date: 4/14/2026
 * Last Modified: 4/14/2026
 * 
 * Brief Description: Tutorializes pressing left/right if the player has never pressed left / right on the car before.
 */

using NaughtyAttributes;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CarTutorialPossessaleCanvasOverlay : PossessableCanvasOverlay
{
    [Required, SerializeField] private CanvasGroup group;
    public override void Initialize()
    {
        if (SaveDataManager.Instance.HasPlayerMovedWithCar())
        {
            Debug.Log("Player has moved with car before: hiding tutorial");
            Destroy(this.gameObject);
            return;
        }
    }

    public override void WhilePossessedUpdate()
    {
        PossessableToolbar.Instance?.currentCanvasOverlay?.WhilePossessedUpdate();

        // if the player has now moved with the car
        if (SaveDataManager.Instance.HasPlayerMovedWithCar())
        {
            DeinitializeThenDestroy();
        }
    }

    public override Task ThisDeinitialize()
    {
        throw new System.NotImplementedException();
    }
}

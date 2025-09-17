/*
 * Contributors: Toby
 * Creation Date: 9/16/25
 * Last Modified: 9/16/25
 * 
 * Brief Description: On every possessable object, and the player for simplicity. 
 * Contains reference to input scripts and other stuff.
 *  
 *  TODO: this should probably need a reference to the camera and an anchor point (for 3rd person).
 *  Also a reference to the thing the camera is orbiting
 */



//change cameras by turning cameras on and off
//use triggers to detect when ghost is close enough, on button press
//not detectable, can't move
//press button to perform an action (spawn a cube or something)
using UnityEngine;

public class PossessableObject : MonoBehaviour
{
    public IInputHandler InputHandler;
    public ICameraInputHandler CameraInputHandler;

    //oncontrol()
    //possess()

    public void Possess()
    {
        SwitchCamera();
        //not detectable
        //disable movement
        //can do thing
    }

    public void SwitchCamera()
    {
        //switch from 1st to 3rd
    }

    public void ChangeMovement()
    {
        //disable movement
        //or change movement to specific object
    }

    public void PerformAction()
    {
        //on button push, do action if applicable
    }


}

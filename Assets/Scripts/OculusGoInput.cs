using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OculusGoInput : MonoBehaviour
{
    /// <summary>Delegate which handles single axis movement.</summary>
    public delegate void GoMoveEvent(float value);

    /// <summary>Event which handles movement.</summary>
    public event GoMoveEvent OnMove;

    /// <summary>Delegate which handles single axis rotation.</summary>
    public delegate void GoLookEvent(float value);

    /// <summary>Event which handles looking.</summary>
    public event GoLookEvent OnLook;

    /// <summary>Delegate which handles weapon firing.</summary>
    public delegate void GoFireEvent(bool active);

    /// <summary>Event which handles weapon firing.</summary>
    public event GoFireEvent OnFire;

    /// <summary>Delegate which handles weapon switching.</summary>
    public delegate void GoSwitchWeaponEvent(bool active);

    /// <summary>Event which handles weapon switching.</summary>
    public event GoSwitchWeaponEvent OnSwitchWeapon;

    /// <summary>Delegate which handles in-game pausing.</summary>
    public delegate void GoPauseEvent(bool active);

    /// <summary>Event which handles in-game pausing.</summary>
    public event GoPauseEvent OnPause;

    /// <summary>Gets the controler </summary>
    public GameObject controller;

    /// <summary>To set the object that the controller uses for tracking input </summary>
    public GameObject controllerPoint;

    /// <summary>Gets the mech </summary>
    public GameObject mech;

    /// <summary>To set the deadzone of the movement "Joystick" </summary>
    [Range(0.0f, 1.0f)]
    public float deadzone = 0.5f;

	
    private void Start()
    {
    }

    private void Update()
    {
        Matrix4x4 MechMat4 = mech.transform.worldToLocalMatrix;
        Matrix4x4 ControllerMat4 = controller.transform.localToWorldMatrix;

        Matrix4x4 CombineMat4 = MechMat4 * ControllerMat4;
		
		
		Vector3 setFloatingPos = controller.transform.position + new Vector3(0.0f, 1.0f, 0.0f);
		Vector3 controllerDirection = controllerPoint.transform.position - setFloatingPos;
		Vector4 controllerDirection4 = new Vector4(controllerDirection.x, controllerDirection.y, controllerDirection.z, 0);
		
		controllerDirection = MechMat4 * controllerDirection4;
		
		
		float inputY = Mathf.Clamp(controllerDirection.x, -1.0f, 1.0f);
        float inputX = Mathf.Clamp(controllerDirection.z, -1.0f, 1.0f);
		

        OnLook?.Invoke(GetControllerHorizontal(inputY));
        OnMove?.Invoke(GetControllerVertical(inputX));
        OnFire?.Invoke(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger));
        OnSwitchWeapon?.Invoke(OVRInput.Get(OVRInput.Button.PrimaryTouchpad));
    }

    private float GetControllerHorizontal(float angle)
    {
        if (Mathf.Abs(angle) < deadzone)
        {
            return 0.0f;
        }
        else
        {
            return angle;
        }
    }

    private float GetControllerVertical(float angle)
    {
        if (Mathf.Abs(angle) < deadzone)
        {
            return 0.0f;
        }
        else
        {
            return angle;
        }
    }

}

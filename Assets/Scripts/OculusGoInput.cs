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

    /// <summary>Gets the mech </summary>
    public GameObject mech;

    /// <summary>To set the deadzone of the movement "Joystick" </summary>
    [Range(0.0f, 1.0f)]
    public float deadzone = 0.5f;

    /// <summary>The min look range is to be edited to find a good median </summary>
    [Range(0.0f, 360.0f)]
    public float LookRangeMin = 65.0f;
    /// <summary>The max look range is to be edited to find a good median </summary>
    [Range(0.0f, 360.0f)]
    public float LookRangeMax = 320.0f;

    /// <summary>The min move range is to be edited to find a good median </summary>
    [Range(0.0f, 360.0f)]
    public float MoveRangeMin = 270.0f;
    /// <summary>The max move range is to be edited to find a good median </summary>
    [Range(0.0f, 360.0f)]
    public float MoveRangeMax = 359.0f;

	public GameObject controllerPoint;

    private bool linearMovement = false;

	public Text debugText;
	public Text debugText2;
	
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
		
		Debug.DrawLine(controllerPoint.transform.position, setFloatingPos);
		
		controllerDirection = MechMat4 * controllerDirection4;
		

        
		
		
		float inputY = controllerDirection.x; //ClampAngle(CombineMat4.rotation.eulerAngles.y, LookRangeMin, LookRangeMax) * -1.0f;
        float inputX = controllerDirection.y; //ClampAngle(CombineMat4.rotation.eulerAngles.x, MoveRangeMin, MoveRangeMax);
		
		debugText.text = inputY.ToString();
		debugText2.text = inputX.ToString();

        //if (inputY == 1.0f || inputY == -1.0f)
        //    inputY = 0.0f;
        //if (inputX == 1.0f || inputX == -1.0f)
        //    inputX = 0.0f;

        linearMovement = false;


        OnLook?.Invoke(GetControllerHorizontal(inputY));
        OnMove?.Invoke(GetControllerVertical(inputX));
        OnFire?.Invoke(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger));
        OnSwitchWeapon?.Invoke(OVRInput.Get(OVRInput.Button.PrimaryTouchpad));
    }

    private float MapToRange(float value, float oldMin, float oldMax, float newMin, float newMax)
    {
        // 60 - -60 = 120
        float oldRange = oldMax - oldMin;
        // 1 - -1 = 2
        float newRange = newMax - newMin;

        // (-90 - -60) / 120 = -0.25
        float fractionThrough = (value - oldMin) / oldRange;
        // -0.25 * 2 + -1
        return fractionThrough * newRange + newMin;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        return Mathf.Clamp(MapToRange(angle, min, max, -1, 1), -1.0f, 1.0f);
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

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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


    public TextMeshPro textA;
    public TextMeshPro textB;

    private Matrix4x4 ControllerMat4;
    private Matrix4x4 MechMat4;
    private Matrix4x4 CombineMat4;



    private void Start()
    {
        //OnPause += GameManager.Instance.TogglePause;
    }

    private void Update()
    {
        OnPause?.Invoke(Input.GetKeyDown(KeyCode.Escape));

        MechMat4 = mech.transform.worldToLocalMatrix;
        ControllerMat4 = controller.transform.localToWorldMatrix;

        CombineMat4 = MechMat4 * ControllerMat4;

        textA.text = CombineMat4.rotation.eulerAngles.x.ToString();
        textB.text = CombineMat4.rotation.eulerAngles.y.ToString();

        if (OVRInput.Get(OVRInput.Button.PrimaryTouchpad))
        {
            OnLook?.Invoke(GetControllerHorizontal());
            OnMove?.Invoke(GetControllerVertical());
        }

        OnFire?.Invoke(OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger));
        OnSwitchWeapon?.Invoke(OVRInput.Get(OVRInput.Button.Back));
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


    private float GetControllerHorizontal()
    {
        //float angle = controller.transform.localEulerAngles.y;
        float angle = CombineMat4.rotation.eulerAngles.y;


        float temp = Mathf.Clamp(MapToRange(angle, 65, 320, 1, -1), -1.0f, 1.0f);
        
        if (Mathf.Abs(temp) < deadzone)
        {
            return 0.0f;
        }
        else
        {
            return temp;
        }
    }

    private float GetControllerVertical()
    {
        //float angle = controller.transform.localEulerAngles.x;
        float angle = CombineMat4.rotation.eulerAngles.x;


        float temp = Mathf.Clamp(MapToRange(angle, 270, 359, -1, 1), -1.0f, 1.0f);

        if (Mathf.Abs(temp) < deadzone)
        {
            return 0.0f;
        }
        else
        {
            return temp;
        }
    }

}

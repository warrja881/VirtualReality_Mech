using System.Collections;
using System.Collections.Generic;
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

    private void Start()
    {
        //OnPause += GameManager.Instance.TogglePause;
    }

    private void Update()
    {
        OnPause?.Invoke(Input.GetKeyDown(KeyCode.Escape));

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
        float angle = controller.transform.eulerAngles.y;
        //float temp = Mathf.Clamp(MapToRange(controller.transform.rotation.y, -90, 90, -1, 1), -1.0f, 1.0f);
        if (angle > 270)
            angle -= 180;
        float temp = Mathf.Clamp(MapToRange(controller.transform.eulerAngles.y, 30, 270, -1, 1), -1.0f, 1.0f);
        //float temp = Mathf.Clamp(MapToRange(controllerTransform.eulerAngles.y, -90, 90, -1, 1), -1.0f, 1.0f);

        if (temp > -0.25f && temp < 0.25f)
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
        //float temp = Mathf.Clamp(MapToRange(controller.transform.rotation.x, -110, 0, -1, 1), -1.0f, 1.0f);
        float temp = Mathf.Clamp(MapToRange(controller.transform.eulerAngles.x, 0, 320, -1, 1), -1.0f, 1.0f);
        //float temp = Mathf.Clamp(MapToRange(controllerTransform.eulerAngles.x, -110, 0, -1, 1), -1.0f, 1.0f);

        if (temp > -0.25f && temp < 0.25f)
        {
            return 0.0f;
        }
        else
        {
            return temp;
        }
    }
}

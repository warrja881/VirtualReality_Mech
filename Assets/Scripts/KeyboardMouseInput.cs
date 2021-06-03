using UnityEngine;

public class KeyboardMouseInput : MonoBehaviour
{
    /// <summary>Delegate which handles single axis movement.</summary>
    public delegate void KeyboardMoveEvent(float value);

    /// <summary>Event which handles movement.</summary>
    public event KeyboardMoveEvent OnMove;

    /// <summary>Delegate which handles single axis rotation.</summary>
    public delegate void KeyboardLookEvent(float value);
    
    /// <summary>Event which handles looking.</summary>
    public event KeyboardLookEvent OnLook;

    /// <summary>Delegate which handles weapon firing.</summary>
    public delegate void KeyboardFireEvent(bool active);

    /// <summary>Event which handles weapon firing.</summary>
    public event KeyboardFireEvent OnFire;

    /// <summary>Delegate which handles weapon switching.</summary>
    public delegate void KeyboardSwitchWeaponEvent(bool active);

    /// <summary>Event which handles weapon switching.</summary>
    public event KeyboardSwitchWeaponEvent OnSwitchWeapon;

    private void Update()
    {
        OnLook?.Invoke(Input.GetAxis("Horizontal"));
        OnMove?.Invoke(Input.GetAxis("Vertical"));
        OnFire?.Invoke(Input.GetMouseButton(0));
        OnSwitchWeapon?.Invoke(Input.GetMouseButtonDown(1));
    }
}

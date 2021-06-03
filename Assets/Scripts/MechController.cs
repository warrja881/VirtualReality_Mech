using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The movement speed of the mech.")]
    private float m_MoveSpeed = 5.0f;

    [Tooltip("The speed of the mech's turning.")]
    [SerializeField]
    private float m_RotationSpeed = 1.0f;

    [Tooltip("The smoothness of the mech's turning.")]
    [SerializeField]
    private float m_RotationSmooth = 5.0f;

    [SerializeField]
    [Tooltip("The spinning barrel piece of the mech's chaingun.\n(NOTE: This is only temporary and will be moved into a chaingun script.)")]
    private Transform m_ChainGunBarrel;

    [SerializeField]
    [Tooltip("The maximum spin speed of the chaingun barrel when it is spooling.\n(NOTE: This is only temporary and will be moved into a chaingun script.)")]
    private float m_MaxBarrelSpeed = 15.0f;

    /// <summary>The chaingun's current rotation speed.</summary>
    private float m_CurrentBarrelSpeed = 0.0f;

    /// <summary>The mech's current rotation.</summary>
    private float m_MechRotation = 0.0f;

    /// <summary>The keyboard and mouse input handler.</summary>
    private KeyboardMouseInput m_KBMInput;

    ////////////////////////////////////////////////////////////
    //// THE FOLLOWING IS JANK AND WILL BE REMOVED LATER ON ////
    ////////////////////////////////////////////////////////////
    
    private enum Weapon { ChainGun = 1, RocketLauncher = 2 }

    [SerializeField]
    [Tooltip("The current selected weapon.\n(NOTE: This is jank af and will be removed later in favour of a better solution.)")]
    private Weapon m_CurrentWeapon = Weapon.ChainGun;

    private void Awake()
    {
        // If an input system has not been assign, attempt to
        // find one on the game object this script is attached to
        if (m_KBMInput == null)
            m_KBMInput = GetComponent<KeyboardMouseInput>();
    }

    private void Start()
    {
        // Ensure input component exists before assigning inputs
        // This prevents null reference exceptioon errors
        if (m_KBMInput != null)
        {
            m_KBMInput.OnFire += Fire;
            m_KBMInput.OnLook += Look;
            m_KBMInput.OnMove += Move;
        }
    }

    /// <summary>Moves the mech forward and back based on the input.</summary>
    /// <param name="value">The input value.</param>
    private void Move(float value)
    {
        transform.position += transform.forward * value * m_MoveSpeed * Time.deltaTime;
    }

    /// <summary>Rotates the mech left and right based on the input.</summary>
    /// <param name="value">The input value.</param>
    private void Look(float value)
    {
        m_MechRotation += value;
        Quaternion targetRotation = Quaternion.Euler(0.0f, m_MechRotation, 0.0f);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * m_RotationSmooth);
    }

    /// <summary>Switches between the chaingun and rocket launcher.</summary>
    private void SwitchWeapons()
    {
        if (m_CurrentWeapon == Weapon.ChainGun)
            m_CurrentWeapon = Weapon.RocketLauncher;
        else
            m_CurrentWeapon = Weapon.ChainGun;
    }

    /// <summary>Fires the mech's weapon </summary>
    /// <param name="input"></param>
    private void Fire(bool input)
    {
        // It's too much to explain here so trust when I say do not
        // move the spool check from the front of this if statement
        if (ChainGunSpooled(input) && m_CurrentWeapon == Weapon.ChainGun)
        {
            // Do shoot shoot
        }
    }

    /// <summary>Spools the chaingun to prepare it for firing.</summary>
    /// <param name="spool">Defines whether or not the chaingun should be spooling.</param>
    /// <returns>Whether chaingun is spooled and ready to be fired.</returns>
    private bool ChainGunSpooled(bool spool)
    {
        // Increase or decrease the spin speed of the barrel
        if (spool)
            m_CurrentBarrelSpeed += 10.0f * Time.deltaTime;
        else
            m_CurrentBarrelSpeed -= 10.0f * Time.deltaTime;

        // Clamp the speed of the barrel spin
        m_CurrentBarrelSpeed = Mathf.Clamp(m_CurrentBarrelSpeed, 0.0f, m_MaxBarrelSpeed);
        
        // Rotate the barrel
        m_ChainGunBarrel.Rotate(0.0f, 0.0f, m_CurrentBarrelSpeed);

        // Return spooled status
        return m_CurrentBarrelSpeed >= m_MaxBarrelSpeed;
    }
}

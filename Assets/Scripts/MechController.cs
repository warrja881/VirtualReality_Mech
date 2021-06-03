using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
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

    public Rigidbody Rigidbody { get => m_Rigidbody; }

    private Rigidbody m_Rigidbody;

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

        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Lock and hide cursor during runtime
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Ensure input component exists before assigning inputs
        // This prevents null reference exceptioon errors
        if (m_KBMInput != null)
        {
            m_KBMInput.OnFire += Fire;
            m_KBMInput.OnLook += Look;
            m_KBMInput.OnMove += Move;
            m_KBMInput.OnSwitchWeapon += SwitchWeapons;
        }
    }

    private void FixedUpdate()
    {
        if (m_Rigidbody.velocity.y != 0.0f)
            m_Rigidbody.velocity += Physics.gravity * 2.5f * Time.fixedDeltaTime;
    }

    /// <summary>Moves the mech forward and back based on the input.</summary>
    /// <param name="value">The input value.</param>
    private void Move(float value)
    {
        m_Rigidbody.position += transform.forward * value * m_MoveSpeed * Time.deltaTime;
    }

    /// <summary>Rotates the mech left and right based on the input.</summary>
    /// <param name="value">The input value.</param>
    private void Look(float value)
    {
        m_MechRotation += value * m_RotationSpeed;
        Quaternion targetRotation = Quaternion.Euler(0.0f, m_MechRotation, 0.0f);
        m_Rigidbody.rotation = Quaternion.Lerp(m_Rigidbody.rotation, targetRotation, Time.deltaTime * m_RotationSmooth);
    }

    /// <summary>Switches between the chaingun and rocket launcher.</summary>
    private void SwitchWeapons(bool input)
    {
        if (input)
        {
            if (m_CurrentWeapon == Weapon.ChainGun)
                m_CurrentWeapon = Weapon.RocketLauncher;
            else
                m_CurrentWeapon = Weapon.ChainGun;
        }
    }

    /// <summary>Fires the mech's weapon </summary>
    /// <param name="input"></param>
    private void Fire(bool input)
    {
        if (ChainGunSpooled(input))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitInfo))
            {
                ExplosiveBarrel barrel = null;
                hitInfo.transform.TryGetComponent(out barrel);

                barrel?.Explode();
            }
        }
    }

    /// <summary>Spools the chaingun to prepare it for firing.</summary>
    /// <param name="spool">Defines whether or not the chaingun should be spooling.</param>
    /// <returns>Whether chaingun is spooled and ready to be fired.</returns>
    private bool ChainGunSpooled(bool spool)
    {
        // Increase or decrease the spin speed of the barrel
        if (spool && m_CurrentWeapon == Weapon.ChainGun)
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

using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MechController : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The mech's chain gun.")]
    public ChainGun m_ChainGun;

    [SerializeField]
    [Tooltip("The mech's rocket launcher.")]
    public RocketLauncher m_RocketLauncher;

    [SerializeField]
    [Tooltip("The movement speed of the mech.")]
    private float m_MoveSpeed = 5.0f;

    [Tooltip("The speed of the mech's turning.")]
    [SerializeField]
    private float m_RotationSpeed = 1.0f;

    [Tooltip("The smoothness of the mech's turning.")]
    [SerializeField]
    private float m_RotationSmooth = 5.0f;

    /// <summary>The mech's current rotation.</summary>
    private float m_MechRotation = 0.0f;

    /// <summary>The keyboard and mouse input handler.</summary>
    private KeyboardMouseInput m_KBMInput;

    /// <summary>The Oculus Go input handler.</summary>
    private OculusGoInput m_GOInput;

    public Rigidbody Rigidbody { get => m_Rigidbody; }

    private Rigidbody m_Rigidbody;

    private enum Weapon { ChainGun = 1, RocketLauncher = 2 }

    [SerializeField]
    [ReadOnly]
    [Tooltip("The current selected weapon.")]
    private Weapon m_CurrentWeapon = Weapon.ChainGun;

    private void Awake()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            // If an input system has not been assign, attempt to
            // find one on the game object this script is attached to
            if (m_KBMInput == null)
                m_KBMInput = GetComponent<KeyboardMouseInput>();
        }
        else
        {
            if (m_GOInput == null)
                m_GOInput = GetComponent<OculusGoInput>();
        }

        m_Rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        // Lock and hide cursor during runtime
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_MechRotation = transform.localRotation.eulerAngles.y;

        // Ensure input component exists before assigning inputs
        // This prevents null reference exceptioon errors
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            if (m_KBMInput != null)
            {
                m_KBMInput.OnFire += Fire;
                m_KBMInput.OnLook += Look;
                m_KBMInput.OnMove += Move;
                m_KBMInput.OnSwitchWeapon += SwitchWeapons;
            }
        }
        else
        {
            if (m_GOInput != null)
            {
                m_GOInput.OnFire += Fire;
                m_GOInput.OnLook += Look;
                m_GOInput.OnMove += Move;
                m_GOInput.OnSwitchWeapon += SwitchWeapons;
            }
        }

        m_ChainGun?.Select();
        m_RocketLauncher?.Unselect();
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
        transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * m_RotationSmooth);
    }

    /// <summary>Switches between the chaingun and rocket launcher.</summary>
    private void SwitchWeapons(bool input)
    {
        if (input)
        {
            if (m_CurrentWeapon == Weapon.ChainGun)
            {
                m_CurrentWeapon = Weapon.RocketLauncher;
                m_RocketLauncher?.Select();
                m_ChainGun?.Unselect();
            }
            else
            {
                m_CurrentWeapon = Weapon.ChainGun;
                m_ChainGun?.Select();
                m_RocketLauncher?.Unselect();
            }
        }
    }

    /// <summary>Fires the mech's weapon.</summary>
    /// <param name="input"></param>
    private void Fire(bool input)
    {
        // Calls both weapon fire functions but only
        // the selected weapon will actually fire.
        m_ChainGun?.Fire(input);
        m_RocketLauncher?.Fire(input);
    }
}

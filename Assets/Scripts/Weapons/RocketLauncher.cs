using System.Collections;
using UnityEngine;

public class RocketLauncher : Weapon
{
    [Header("Rocket launcher")]

    [SerializeField]
    [Tooltip("This is usually where the rockets will appear.")]
    private Transform m_FiringPoint;

    [SerializeField]
    [Tooltip("The prafab which will be used as the rocket.")]
    private GameObject m_RocketPrefab;

    [SerializeField]
    [Tooltip("The current ammunition count.")]
    private int m_Ammunition;

    [SerializeField]
    [Tooltip("THe maximum amunition count. Setting this to 0 will allow infinite ammo.")]
    private int m_MaxAmmunition = 4;

    [SerializeField]
    [Tooltip("The time it takes to reload the launcher.")]
    private float m_ReloadTime = 1.5f;

    [SerializeField]
    [Tooltip("Defines much ammunition should be available before it is defined as low.")]
    [Range(0, 100)]
    private int m_LowAmmoPercentage = 25;

    [ReadOnly]
    [Tooltip("Defines whether or not the player is reloading.")]
    public bool m_Reloading = false;

    /// <summary>Used to determine how far along the reloading process is.</summary>
    private float m_CurrentReloadTime;

    private void Start()
    {
        m_Ammunition = m_MaxAmmunition;
        m_NextFiringTime = Time.time + (60.0f / m_RateOfFire);
    }

    /// <summary>Checks if the weapon's ammunition pool is nearing depletion.</summary>
    /// <returns>True if weapon ammunition is nearing depletion.</returns>
    public bool LowAmmunition() => m_MaxAmmunition / 100.0f * m_Ammunition <= m_LowAmmoPercentage;

    /// <summary>Checks if the weapon's ammunition pool has been depleted.</summary>
    /// <returns>True if weapon ammunition has been depleted.</returns>
    public bool Empty() => m_Ammunition == 0;

    public override void Fire(bool input)
    {
        // Don't fire if this weapon isn't selected
        if (m_Selected && input)
            FireProjectile();

        // PLacing this code here allows the weapon to reload in the background as
        // the Fire() function is called regardless of this weapon's selection state
        if (Empty())
        {
            if (!m_Reloading)
            {
                // Begin reloading
                StartCoroutine(Reload());
            }
        }
    }

    /// <summary>Handles the instantiation and launbcing of the rocket.</summary>
    private void FireProjectile()
    {
        // Ensure weapon is firing at the correct rate and has ammunition available
        if (ReadyToFire() && !Empty())
        {
            // TODO: Ensure that if the prefab does not contain
            //       the rocket script it will still be fired

            Transform objectFiredAt = GetObjectAhead(m_FiringPoint, out Vector3 firingDirection);
            Rocket rocket = InstantiateRocket();
            rocket.Launch(firingDirection);
            m_Ammunition--;
        }
    }

    private Rocket InstantiateRocket()
    {
        Instantiate(m_RocketPrefab, m_FiringPoint.position, m_FiringPoint.rotation).TryGetComponent(out Rocket rocket);
        return rocket;
    }

    /// <summary>Refills the weapon's ammunition pool.</summary>
    public void RefillAmmunition() => m_Ammunition = m_MaxAmmunition;

    /// <summary>Reloads the weapon after a delay.</summary>
    public IEnumerator Reload()
    {
        m_Reloading = true;

        // Wait
        yield return new WaitForSeconds(m_ReloadTime);

        // Top up ammo
        RefillAmmunition();
        
        m_Reloading = false;
    }
}

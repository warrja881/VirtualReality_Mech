using UnityEngine;

public class ChainGun : Weapon
{
    [Header("Chain Gun")]

    [SerializeField]
    [Tooltip("This is usually where the barrel ends and the muzzle flash would appear.")]
    private Transform m_FiringPoint;

    [SerializeField]
    [Tooltip("The warm up time of the chain gun before firing.")]
    private float m_SpoolTime;

    [SerializeField]
    [Tooltip("The bullet spread when firing the chain gun.")]
    private float m_Spread;

    [SerializeField]
    [Tooltip("The spinning barrel piece of the mech's chaingun.\n(NOTE: This is only temporary.)")]
    private Transform m_ChainGunBarrel;

    [SerializeField]
    [Tooltip("The maximum spin speed of the chaingun barrel when it is spooling.\n(NOTE: This is only temporary.)")]
    private float m_MaxBarrelSpeed = 15.0f;

    /// <summary>The chaingun's current rotation speed.</summary>
    private float m_CurrentBarrelSpeed = 0.0f;

    private void Start() => m_NextFiringTime = Time.time + (60.0f / m_RateOfFire);

    public override void Fire(bool input)
    {
        // Ensure gun is spooled before and the rate of fire is matched
        if (Spooled(input) && ReadyToFire())
        {
            // TODO: Ensure that if the prefab does not contain
            //       the rocket script it will still be fired

            Transform objectFiredAt = GetObjectAhead(m_FiringPoint, out Vector3 firingDirection);
            if (objectFiredAt != null)
            {
                objectFiredAt.TryGetComponent(out ExplosiveBarrel barrel);
                barrel?.TakeDamage(m_DamageOutput);
            }
        }
    }



    /// <summary>Spools the chaingun to prepare it for firing.</summary>
    /// <returns>Whether chaingun is spooled and ready to be fired.</returns>
    private bool Spooled(bool spool)
    {
        // Increase or decrease the spin speed of the barrel
        if (m_Selected && spool)
            m_CurrentBarrelSpeed += m_MaxBarrelSpeed / m_SpoolTime * Time.deltaTime;
        else
            m_CurrentBarrelSpeed -= m_MaxBarrelSpeed / m_SpoolTime * Time.deltaTime;
        
        // Clamp the speed of the barrel spin
        m_CurrentBarrelSpeed = Mathf.Clamp(m_CurrentBarrelSpeed, 0.0f, m_MaxBarrelSpeed);
        
        // Rotate the barrel
        m_ChainGunBarrel.Rotate(0.0f, 0.0f, m_CurrentBarrelSpeed);

        // Return spooled status
        return m_CurrentBarrelSpeed >= m_MaxBarrelSpeed;
    }
}

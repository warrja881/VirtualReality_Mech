using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [Header("General")]

    [SerializeField]
    [Tooltip("Reference to the player's position in the mech.\nNOTE: This should idealy be assigned with an object that is vertically alligned with the weapons")]
    protected Transform m_PlayerPosition;

    [SerializeField]
    [Tooltip("How many rounds are fired per minute.")]
    protected int m_RateOfFire;

    [SerializeField]
    [Tooltip("The amount of damage each round does to relevant objects.")]
    protected int m_DamageOutput;

    [SerializeField]
    [ReadOnly]
    [Tooltip("Whether the weapon is currently selected.")]
    protected bool m_Selected = false;

    /// <summary>Marks the weapon as selected.</summary>
    public void Select() => m_Selected = true;

    /// <summary>Marks the weapon as unselected.</summary>
    public void Unselect() => m_Selected = false;

    /// <summary>A value used to calculate when the weeapon can fire its next round.</summary>
    protected float m_NextFiringTime = 0.0f;

    /// <summary>Clamps the rate of fire to ensure the gun isn't a raycast lazer beam.</summary>
    /// <returns>True if the weapon is to be fired.</returns>
    protected bool ReadyToFire()
    {
        if (Time.time >= m_NextFiringTime)
        {
            m_NextFiringTime = Time.time + (60.0f / m_RateOfFire);
            return true;
        }

        return false;
    }

    protected Transform GetObjectAhead(Transform firingPoint, out Vector3 firingDirection)
    {
        firingDirection = firingPoint.forward;

        // Check for raycast hit
        if (Physics.Raycast(m_PlayerPosition.position, m_PlayerPosition.forward, out RaycastHit hitInfo))
        {
            // Cache the direction from the gun barrel and the hit
            firingDirection = (hitInfo.point - firingPoint.position).normalized;
            float dot = Vector3.Dot(firingPoint.forward, firingDirection);

            // Only return the object looked at if it's within a reasonable angle
            // This will prevent shooting a pillar the player may be hard up against
            if (dot >= 0.9f)
                return hitInfo.transform;
        }

        return null;
    }

    /// <summary>Fires the weapon.</summary>
    /// <param name="input">Defines whether the fire input is active.</param>
    public abstract void Fire(bool input);
}

using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The maximum health of the barrel.")]
    private int m_MaxHealth = 100;

    [SerializeField]
    [Tooltip("The current health of the barrel.")]
    private int m_CurrentHealth = 100;

    [SerializeField]
    [Tooltip("The range of the barrel's explosion.")]
    private float m_BlastRadius = 5.0f;

    [SerializeField]
    [Tooltip("The delay of the explosion when it is triggered by a neighbouring barrel.")]
    private float m_ChainExplosionDelay = 0.5f;

    [SerializeField]
    [Tooltip("The amount of damage the barrel deals to the mechs.")]
    private float m_ExplosionForce = 250.0f;

    [SerializeField]
    [Tooltip("The amount of damage the barrel deals to the mechs.")]
    private float m_Damage = 50.0f;

    public Rigidbody Rigidbody { get => m_Rigidbody; }
    private Rigidbody m_Rigidbody;

    /// <summary>Defines whether not the barrel is currently in the process of exploding.</summary>
    [HideInInspector]
    public bool m_Exploding = false;

    private void Start() => m_Rigidbody = GetComponent<Rigidbody>();

    public void Explode()
    {
        m_Exploding = true;

        // Finds player mech
        // TODO: Extend this to check all mechs including AI if implemented
        var mech = FindObjectOfType(typeof(MechController)) as MechController;

        // Find all surrounding barrels
        var surroundingBarrels = FindObjectsOfType(typeof(ExplosiveBarrel)).ToList();

        // Linq style foreach loop
        surroundingBarrels.ForEach(itr =>
        {
            // Cache current iterator as the barrel type
            var barrel = itr as ExplosiveBarrel;

            // Ensure no self reference
            if (barrel != this)
            {
                float distanceToOtherBarrel = Vector3.Distance(transform.position, barrel.transform.position);

                // Check if barrel is within blast radius
                if (Vector3.Distance(transform.position, barrel.transform.position) <= m_BlastRadius)
                {
                    // Add esplosion force to barrel
                    barrel.Rigidbody.AddExplosionForce(m_ExplosionForce * barrel.Rigidbody.mass / 2.0f, transform.position, m_BlastRadius);

                    // Detonate barrel
                    barrel.ExplodeWithDelay();
                }
            }
        });

        float distanceToMech = Vector3.Distance(transform.position, mech.transform.position);

        if (Vector3.Distance(transform.position, mech.transform.position) <= m_BlastRadius)
        {
            // Add esplosion force to mech
            mech.Rigidbody.AddExplosionForce(m_ExplosionForce * Rigidbody.mass, transform.position, m_BlastRadius);
        }

        // Destroy game object after explosion
        // NOTE: This is not efficient. It would be best to implement object pooling
        Destroy(gameObject);
    }

    public void ExplodeWithDelay()
    {
        if (!m_Exploding)
            StartCoroutine(DelayedExplosion());
    }

    private IEnumerator DelayedExplosion()
    {
        m_Exploding = true;

        yield return new WaitForSeconds(m_ChainExplosionDelay);
        
        Explode();
    }
}

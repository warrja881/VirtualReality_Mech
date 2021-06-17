using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class ExplosiveBarrel : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The particle system for the explosion.")]
    private ParticleSystem m_ExplosionParticles;
    public ParticleSystem ExplosionParticles { get => m_ExplosionParticles; }

    [SerializeField]
    private GameObject m_Model;
    public GameObject Model { get => m_Model; }

    [HideInInspector]
    public float ExplodeTime = 0.0f;

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
    [Tooltip("The amount of explosion force the barrel deals to its surroundings.")]
    private float m_ExplosionForce = 250.0f;

    [SerializeField]
    [Tooltip("The amount of damage the barrel deals to its surroundings.")]
    private float m_Damage = 50.0f;

    [SerializeField]
    [Tooltip("The distance which beyond barrels will not be visible or interactive.")]
    private float m_CullingDistance = 200.0f;

    public Rigidbody Rigidbody { get => m_Rigidbody; }
    private Rigidbody m_Rigidbody;

    private Collider m_Collider;

    /// <summary>Defines whether not the barrel is currently in the process of exploding.</summary>
    [HideInInspector]
    public bool m_Exploding = false;

    private MechController m_Mech;

    private void Start()
    {
        m_Collider = GetComponent<Collider>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Mech = FindObjectOfType<MechController>();

        GameManager.Instance?.m_ObjectHandler.RegisterBarrel(this);
    }

    public bool WithinViewRange()
    {
        return m_Mech != null && Vector3.Distance(transform.position, m_Mech.transform.position) < m_CullingDistance;
    }

    public void Explode()
    {
        if (!WithinViewRange()) return;

        m_Exploding = true;
        ExplodeTime = Time.time;

        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
        m_Rigidbody.isKinematic = true;
        m_Collider.enabled = false;
        m_Model.SetActive(false);
        m_ExplosionParticles.transform.parent.gameObject.SetActive(true);

        // Find all surrounding barrels
        List<ExplosiveBarrel> surroundingBarrels = new List<ExplosiveBarrel>();
        if (GameManager.Instance != null)
            surroundingBarrels = GameManager.Instance.m_ObjectHandler.BarrelCollection.ToList();
        else
            surroundingBarrels = ((ExplosiveBarrel[])FindObjectsOfType(typeof(ExplosiveBarrel))).ToList();

        // Linq style foreach loop
        surroundingBarrels.ForEach(barrel =>
        {
            // Ensure no self reference
            if (barrel != null && barrel != this)
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

        // Destroy game object after explosion
        if (GameManager.Instance != null)
            GameManager.Instance.m_ObjectHandler.DestroyObjects(this, m_ExplosionParticles);
        else
            Destroy(gameObject);
    }

    public void TakeDamage(int damage)
    {
        m_CurrentHealth -= damage;

        if (m_CurrentHealth <= 0) Explode();
    }

    public void ExplodeWithDelay()
    {
        if (!m_Exploding)
            StartCoroutine(DelayedExplosion());
    }

    private IEnumerator DelayedExplosion()
    {
        m_Exploding = true;

        yield return new WaitForSeconds(UnityEngine.Random.Range(m_ChainExplosionDelay / 2, m_ChainExplosionDelay * 2));
        
        Explode();
    }
}

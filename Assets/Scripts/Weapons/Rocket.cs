using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Rocket : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The particle system for the explosion.")]
    private GameObject m_ExplosionParticles;
    public GameObject ExplosionParticles { get => m_ExplosionParticles; }

    [SerializeField]
    [Tooltip("The particle system for the rocket trail.")]
    private ParticleSystem m_TrailParticles;

    [SerializeField]
    [Tooltip("The initial launch speed of the rocket.")]
    private float m_LaunchSpeed = 75.0f;

    [SerializeField]
    [Tooltip("The amount of explosion force the rocket deals to its surroundings.")]
    private float m_ExplosionForce = 250.0f;

    [SerializeField]
    [Tooltip("The blast radius of the rocket's explosion.")]
    private float m_BlastRadius = 5.0f;

    public void Launch(Vector3 direction)
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        rigidbody.useGravity = false;
        rigidbody.AddForce(direction * m_LaunchSpeed, ForceMode.Impulse);
    }

    private void Explode()
    {
        m_TrailParticles.Stop();
        m_ExplosionParticles.SetActive(true);
        //var explosionParticles = Instantiate(m_ExplosionParticlesPrefab, transform.position, Quaternion.identity);

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
        {
            GameManager.Instance.m_ObjectHandler.DestroyObjects(m_ExplosionParticles, this);
        }
        else
        {
            Destroy(m_ExplosionParticles);
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) => Explode();
}

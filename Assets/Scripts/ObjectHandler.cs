using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectHandler : MonoBehaviour
{
    /// <summary>List of all the objects to be destroyed</summary>
    public HashSet<ExplosiveBarrel> BarrelCollection { get; private set; } = new HashSet<ExplosiveBarrel>();

    private HashSet<Object> m_DestructionQueue = new HashSet<Object>();

    public void Update()
    {
        foreach (var barrel in BarrelCollection)
        {
            if (barrel == null || barrel.m_Exploding) continue;

            if (barrel.WithinViewRange())
                barrel.Model.SetActive(true);
            else
                barrel.Model.SetActive(false);
        }

        foreach (var item in m_DestructionQueue)
        {
            var barrel = item as ExplosiveBarrel;
            if (barrel != null && (barrel.ExplosionParticles == null || !barrel.ExplosionParticles.isPlaying))
            {
                DestroyBarrel(barrel);
                break;
            }

            var particleSystem = item as ParticleSystem;
            if (particleSystem != null && !particleSystem.isPlaying)
            {
                DestroyParticleSystem(particleSystem);
                break;
            }

            var rocket = item as Rocket;
            if (rocket != null && !rocket.ExplosionParticles.GetComponentInChildren<ParticleSystem>().isPlaying)
            {
                DestroyRocket(rocket);
                break;
            }
        }
    }

    public void RegisterBarrel(ExplosiveBarrel barrel)
    {
        BarrelCollection.Add(barrel);
    }

    private void DestroyBarrel(ExplosiveBarrel barrel)
    {
        m_DestructionQueue.Remove(barrel);
        BarrelCollection.Remove(barrel);
        Destroy(barrel.gameObject);
    }

    private void DestroyRocket(Rocket barrel)
    {
        m_DestructionQueue.Remove(barrel);
        Destroy(barrel.gameObject);
    }

    private void DestroyElement(Object element)
    {
        m_DestructionQueue.Remove(element);
        Destroy(element);
    }

    private void DestroyParticleSystem(ParticleSystem pSysten)
    {
        m_DestructionQueue.Remove(pSysten);
        Destroy(pSysten.gameObject);
    }

    public void DestroyObjects(params Object[] objects)
    {
        foreach (var obj in objects)
            m_DestructionQueue.Add(obj);
    }
}

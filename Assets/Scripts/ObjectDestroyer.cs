using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectDestroyer : MonoBehaviour
{
    public enum DestructionMethod
    {

    }

    /// <summary>List of all the objects to be destroyed</summary>
    private HashSet<GameObject> m_DestroyQueue = new HashSet<GameObject>();

    public void Update()
    {
        if (m_DestroyQueue.Count > 0)
        {
            var ps = m_DestroyQueue.First()?.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
                if (!ps.isPlaying)
                    DestroyFirst();
            else if (m_DestroyQueue.First().TryGetComponent(out ParticleSystem system))
                if (!system.isPlaying)
                    DestroyFirst();
        }
    }

    private void DestroyElement(GameObject obj)
    {
        m_DestroyQueue.Remove(obj);
        Destroy(obj);
    }

    public void SetDestructionMethod(DestructionMethod method) { }

    public void AddToQueue(GameObject obj) => m_DestroyQueue.Add(obj);

    public void DestroyAll()
    {
        foreach (var obj in m_DestroyQueue)
            Destroy(obj);

        m_DestroyQueue.Clear();
    }

    public void DestroyAt(int index)
    {
        if (m_DestroyQueue.Count > index)
        {
            var element = m_DestroyQueue.ElementAt(index);
            m_DestroyQueue.Remove(element);
            Destroy(element);
        }
    }

    public void DestroyFirst()
    {
        var element = m_DestroyQueue.First();
        m_DestroyQueue.Remove(element);
        Destroy(element);
    }

    public void DestroyLast()
    {
        if (m_DestroyQueue.Count > 0)
        {
            var element = m_DestroyQueue.Last();
            m_DestroyQueue.Remove(element);
            Destroy(element);
        }
    }
}

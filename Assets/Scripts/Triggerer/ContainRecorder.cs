using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainRecorder : MonoBehaviour
{
    public HashSet<Collider> inside = new HashSet<Collider>();

    // void Update()
    // {
    //     inside.Remove(null);
    // }

    public bool IsContain(GameObject other)
    {
        if (other.TryGetComponent<Collider>(out var otherCollider))
        {
            return IsContain(otherCollider);
        }
        else
        {
            return false;
        }
    }

    public bool IsContain(Collider other)
    {
        return inside.Contains(other);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        inside.Add(other);
    }

    public virtual void OnTriggerExit(Collider other)
    {
        inside.Remove(other);
    }
}

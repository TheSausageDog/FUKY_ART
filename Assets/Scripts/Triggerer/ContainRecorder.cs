using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainRecorder : MonoBehaviour
{
    public List<Collider> inside = new List<Collider>();

    void Update()
    {
        inside.RemoveAll(s => s == null);
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

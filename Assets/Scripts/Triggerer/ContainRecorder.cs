using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainRecorder : MonoBehaviour
{
    public List<Collider> inside = new List<Collider>();

    public bool IsContain(Collider other){
        return inside.Contains(other);
    }

    void OnTriggerEnter(Collider other)
    {
        inside.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        inside.Remove(other);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    public delegate void Trigged();
    public Collider target;
    public Trigged trigged = () => { };

    public bool isInside { get; protected set; }

    void OnTriggerEnter(Collider other)
    {
        if (other == target)
        {
            trigged();
            isInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == target)
        {
            isInside = false;
        }
    }
}

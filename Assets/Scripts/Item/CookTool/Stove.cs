using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : ActivatableItem
{
    protected bool isFireOn = false;

    protected bool isPanOn = false;

    public Pan pan;

    protected Collider panCollider;

    void Awake()
    {
        panCollider = pan.GetComponent<Collider>();
    }

    public override void Active()
    {
        isFireOn = true;
        if (isPanOn)
        {
            pan.Heat();
        }
    }

    public override void Deactive()
    {
        isFireOn = false;
        pan.Unheat();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == panCollider)
        {
            isPanOn = true;
            if (isFireOn)
            {
                pan.Heat();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == panCollider)
        {
            isPanOn = false;
            pan.Unheat();
        }
    }
}

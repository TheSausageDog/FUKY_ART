using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : ActivatableItem
{
    protected bool isFireOn = false;

    public Pan pan;

    protected Collider panCollider;

    void Awake()
    {
        panCollider = pan.GetComponent<Collider>();
    }

    public override void Active()
    {
        isFireOn = true;
    }

    public override void Deactive()
    {
        isFireOn = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other == panCollider)
        {

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other == panCollider)
        {

        }
    }
}

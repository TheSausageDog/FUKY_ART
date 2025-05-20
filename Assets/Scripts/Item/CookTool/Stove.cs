using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stove : InteractItemBase
{
    public bool isFireOn { protected set; get; } = false;

    public Transform fire;

    protected bool isPanOn = false;

    public Pan pan;

    protected Collider panCollider;

    void Awake()
    {
        panCollider = pan.GetComponent<Collider>();
    }

    public override void OnInteract()
    {
        isFireOn = !isFireOn;
        fire.gameObject.SetActive(isFireOn);
        if (isPanOn)
        {
            if (isFireOn) { pan.Heat(); } else { pan.Unheat(); }
        }
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

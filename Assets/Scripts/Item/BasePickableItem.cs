using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public class BasePickableItem : MonoBehaviour
{
    [NonSerialized]public Rigidbody rb;

    public float PickDelay;
    public virtual void Awake()
    {
        rb= GetComponent<Rigidbody>();
    }

    public virtual void Start()
    {
    }

    public virtual void OnHandEnter()
    {
        gameObject.layer= LayerMask.NameToLayer("Outline");
        foreach (Transform child in transform)
        {
            child.gameObject.layer= LayerMask.NameToLayer("Outline");
        }
    }
    public virtual void OnHandExit()
    {

        gameObject.layer= LayerMask.NameToLayer("Default");
        foreach (Transform child in transform)
        {
            child.gameObject.layer= LayerMask.NameToLayer("Default");
        }
    }
    
    public virtual void OnPickup(Transform holdPos)
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = holdPos.transform.parent.parent;

        UEvent.Dispatch(EventType.OnItemPicked,this);
        //heldObjRb.isKinematic = true;

    }
    
    public virtual void OnThrow()
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = null;
        //heldObjRb.isKinematic = true;
        UEvent.Dispatch(EventType.OnItemDrop);
    }
}

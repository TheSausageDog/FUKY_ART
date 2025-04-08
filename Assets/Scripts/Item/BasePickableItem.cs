using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public class BasePickableItem : MonoBehaviour, IInteractable
{
    [NonSerialized]
    public Rigidbody rb;
    [NonSerialized]
    public Collider cd;

    public virtual InteractionType interactionType{ get{ return InteractionType.Pick; } }

    public Rigidbody _rb => rb;
    public Collider _cd => cd;
    public Transform _transform => transform;

    public float PickDelay;
    public float _pickDelay=>PickDelay;
    private int LayerNumber;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        cd = GetComponent<Collider>();
        LayerNumber = LayerMask.NameToLayer("holdLayer");
    }

    public virtual void Start()
    {
    }

    public virtual void OnHandEnter()
    {
        gameObject.layer = LayerMask.NameToLayer("Outline");
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Outline");
        }
    }

    public virtual void OnHandExit()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
        foreach (Transform child in transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public virtual void OnPickup(Transform holdPos)
    {
        gameObject.layer = LayerNumber;
        
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = holdPos.transform.parent.parent;
        UEvent.Dispatch(EventType.OnItemPicked, this);
    }
    
    public virtual void OnThrow()
    {
        rb.transform.parent = null;
        gameObject.layer = 0;
        rb.isKinematic = false;
        rb.freezeRotation = false;
        rb.useGravity = true;
        
        UEvent.Dispatch(EventType.OnItemDrop);
    }
}
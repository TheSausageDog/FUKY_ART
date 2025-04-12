using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public class BasePickableItem : MonoBehaviour, IInteractable
{

    public virtual InteractionType interactionType { get { return InteractionType.Pick; } }

    public Rigidbody _rb => GetComponent<Rigidbody>();
    public Collider _cd => GetComponent<Collider>();
    public Transform _transform => transform;

    public float PickDelay;
    public float _pickDelay => PickDelay;
    public float objectSize = 1;
    public float _objectSize => objectSize;

    public Transform attachPoint;

    public bool isPicking { get; protected set; } = false;

    private int LayerNumber;

    public virtual void Awake()
    {
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

        if (attachPoint != null && _rb == null)
        {
            gameObject.AddComponent<Rigidbody>();
        }
        if (_rb != null)
        {   
            Rigidbody rb = _rb;
            rb.freezeRotation = true;
            rb.useGravity = false;
        }

        transform.parent = holdPos.transform.parent.parent;

        isPicking = true;
    }

    public virtual void OnThrow()
    {
        transform.parent = null;
        gameObject.layer = 0;

        if (_rb != null)
        {
            Rigidbody rb = _rb;
            rb.isKinematic = false;
            rb.freezeRotation = false;
            rb.useGravity = true;
        }
        if (attachPoint != null)
        {
            float dist = Vector3.Distance(attachPoint.position, transform.position);
            if (dist < 0.5)
            {
                // Destroy(gameObject.GetComponent<Rigidbody>());
                transform.parent = attachPoint;
            }
        }

        isPicking = false;
    }
}
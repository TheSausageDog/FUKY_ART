using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public abstract class InteractItemBase : MonoBehaviour
{
    public abstract void OnInteract();
}

public abstract class HoldableItem : MonoBehaviour
{
    public bool isHolding { get; protected set; } = false;

    public abstract bool isInteractable { get; }

    public Collider itemCollider { get; protected set; }

    public virtual void Start()
    {
        itemCollider = GetComponent<Collider>();
    }

    public virtual void OnPickup(Transform holdPos)
    {
        isHolding = true;
    }

    public virtual void OnThrow()
    {
        isHolding = false;
    }

    public virtual void OnInteract()
    {
        throw new NotImplementedException();
    }

    protected static void SetPickedRigidbody(Rigidbody rigidbody)
    {
        rigidbody.freezeRotation = true;
        rigidbody.useGravity = false;
    }

    protected static void SetDropRigidbody(Rigidbody rigidbody)
    {
        rigidbody.isKinematic = false;
        rigidbody.freezeRotation = false;
        rigidbody.useGravity = true;
    }
}

public abstract class PickableItem : HoldableItem
{
    public override void OnPickup(Transform holdPos)
    {
        transform.parent = holdPos.transform.parent.parent;

        base.OnPickup(holdPos);
    }

    public override void OnThrow()
    {
        transform.parent = null;

        base.OnThrow();
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public abstract class BaseItem : MonoBehaviour
{
    // public float PickDelay;
    // public float _pickDelay => PickDelay;

    public Collider itemCollider { get; protected set; }

    public abstract bool isHoldable { get; }

    public abstract bool isInteractable { get; }

    public virtual void Awake()
    {
        itemCollider = GetComponent<Collider>();
    }

    public virtual void Start()
    {
    }

    public virtual void OnInteract()
    {
        throw new NotImplementedException();
    }

    public virtual void OnPickup(Transform holdPos)
    {
        throw new NotImplementedException();
    }
}

public abstract class HoldableItem : BaseItem
{
    public bool isHolding { get; protected set; } = false;

    public override bool isHoldable { get { return true; } }

    public override void OnPickup(Transform holdPos)
    {
        isHolding = true;
    }

    public virtual void OnThrow()
    {
        isHolding = false;
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


public enum InteractionType
{
    //只能拿起来移动
    Pick,
    //怼脸上看
    Check,
    //只能根据固定限制移动
    Drag,
    //只会触发交互
    Interact
}
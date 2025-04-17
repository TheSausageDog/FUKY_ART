using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DragItem : HoldableItem
{
    public override bool isInteractable { get { return false; } }

    public Transform dragPoint;

    protected Transform holdPos;

    protected Vector3 holdPosOffset;

    void Update()
    {
        if (holdPos != null)
        {
            MoveItem(holdPos.position - holdPosOffset);
        }
    }

    protected abstract void MoveItem(Vector3 targetPosition);

    public override void OnPickup(Transform _holdPos)
    {
        holdPosOffset = _holdPos.position - dragPoint.position;
        holdPos = _holdPos;
        base.OnPickup(_holdPos);
    }

    public override void OnThrow()
    {
        base.OnThrow();
        holdPos = null;
    }
}

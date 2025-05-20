using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class Knife : AttachedPickableItem
{
    public Collider bladeCollider;

    protected override Quaternion rotateOffset { get { return Quaternion.Euler(-90, -90, 0); } }

    public override void OnPickup(Transform _holdPos)
    {
        base.OnPickup(_holdPos);
        bladeCollider.isTrigger = true;
    }

    public override void OnThrow()
    {
        base.OnThrow();
        bladeCollider.isTrigger = false;
    }
}

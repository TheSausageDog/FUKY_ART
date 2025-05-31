using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class Knife : AttachedPickableItem
{
    public Collider bladeCollider;

    void Awake()
    {
        rotateOffset = new Vector3(-90, 0, 0);
    }

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

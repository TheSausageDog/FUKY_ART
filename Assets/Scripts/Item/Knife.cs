using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class Knife : BasePickableItem
{

    public BoxCollider Collider;
    
    public override void OnPickup(Transform holdPos)
    {
        base.OnPickup(holdPos);
        //Collider.enabled = false;
        transform.DORotate(new Vector3(90,3,3), 0.5f);
    }

    public override void OnThrow()
    {
        base.OnThrow();
        //Collider.enabled = true;
    }


    
}

using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class Knife : AttachedPickableItem
{
    // public BoxCollider Collider;

    // protected override bool keepRigbody {get{ return false; }}
    public override void OnPickup(Transform holdPos)
    {
        transform.DORotate(new Vector3(-90, 3, 3), 0.5f);
        base.OnPickup(holdPos);
        //Collider.enabled = false;


    }

    // public override void OnThrow()
    // {
    //     base.OnThrow();
    //     // Destroy(itemRigidbody);

    //     //Collider.enabled = true;
    // }



}

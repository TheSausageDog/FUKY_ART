using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class Knife : BasePickableItem
{

    public BoxCollider Collider;
    
    protected override void OnPickup(Transform holdPos,PickUpAndInteract player)
    {
        base.OnPickup(holdPos,player);
        //Collider.enabled = false;
        transform.DORotate(new Vector3(90,3,3), 0.5f);
    }

    // protected override void OnThrow(PickUpAndInteract player)
    // {
    //     base.OnThrow(player);
    //     //Collider.enabled = true;
    // }


    
}

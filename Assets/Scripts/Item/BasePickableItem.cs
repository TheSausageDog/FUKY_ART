using System;
using System.Collections;
using System.Collections.Generic;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public class BasePickableItem : MonoBehaviour
{
    public Rigidbody rb;
    public Outline outline;
    public void Awake()
    {
        rb= GetComponent<Rigidbody>();
        outline= GetComponent<Outline>();
    }
    
    

    public virtual void OnHandEnter()
    {
        outline.OutlineColor=Color.white;
    }
    public virtual void OnHandExit()
    {
        outline.OutlineColor=Color.clear;
    }
    
    public virtual void OnPickup(Transform holdPos)
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = holdPos.transform.parent.parent;
        
        //heldObjRb.isKinematic = true;

    }
    
    public virtual void OnThrow()
    {
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = null;
        //heldObjRb.isKinematic = true;

    }
}

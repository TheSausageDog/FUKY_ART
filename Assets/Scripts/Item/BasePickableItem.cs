using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using BzKovSoft.ObjectSlicer;
using OutLine;
using UnityEngine;

public class BasePickableItem : MonoBehaviour,IInteractable
{
    [NonSerialized]
    public Rigidbody rb;

    public Rigidbody _rb => rb;
    public Transform _transform => transform;

    public float PickDelay;
    public float _pickDelay=>PickDelay;
    private int LayerNumber;

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
    public virtual void Interact(InteractionType type,params object[] args)
    {

        switch (type)
        {
            case InteractionType.Pick:
                if (args[0] is Transform pickTrans && args[1] is PickUpAndInteract player)
                {
                    OnPickup(pickTrans,player);
                }
                break;
            case InteractionType.Throw:
                if (args[0] is PickUpAndInteract player1)
                {
                    OnThrow(player1);
                }
                break;
            case InteractionType.Interact:
                OnAlternateAction();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
    protected virtual void OnPickup(Transform holdPos,PickUpAndInteract player)
    {
        gameObject.layer = LayerNumber;
        
        // 忽略玩家与物体的碰撞
        Physics.IgnoreCollision(GetComponent<Collider>(),  player.GetComponent<Collider>(), true);
        
        rb.freezeRotation = true;
        rb.useGravity = false;
        rb.transform.parent = holdPos.transform.parent.parent;
        UEvent.Dispatch(EventType.OnItemPicked, this);
    }
    
    protected virtual void OnThrow(PickUpAndInteract player)
    {
        Physics.IgnoreCollision(GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        rb.transform.parent = null;
        gameObject.layer = 0;
        rb.isKinematic = false;
        rb.freezeRotation = false;
        rb.useGravity = true;
        
        UEvent.Dispatch(EventType.OnItemDrop);
    }
    
    public virtual void OnAlternateAction()
    {
        Debug.Log("备用交互（短按）触发：" + gameObject.name);
    }
    

  
}
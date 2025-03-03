using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using Unity.Mathematics;
using UnityEngine;

/// <summary>
/// 黑板，用来存储玩家状态
/// </summary>
public class PlayerBlackBoard : MonoListener
{
    [NonSerialized] public bool isHeldObj=false;
    [NonSerialized] public Rigidbody heldObjRigidBody;
    [NonSerialized] public bool holdingKnife;
    [NonSerialized] public Vector3 knifeOrginPos;
    [NonSerialized] public Vector3 moveLock;

    [Listen(EventType.OnKnifeTouchBegin)]
    private void OnKnifeTouchBegin(Vector3 dir)
    {
        moveLock = dir;
//        Debug.Log(dir);
    }
    [Listen(EventType.OnKnifeTouchEnd)]
    private void OnKnifeTouchEnd()
    {
        moveLock = Vector3.zero;
//        Debug.Log(dir);
    }
}

[EventEnum]
public enum EventType
{
    OnKnifeTouchBegin,
    OnKnifeTouchEnd,
    OnItemPickUp,
    OnItemDrop
}
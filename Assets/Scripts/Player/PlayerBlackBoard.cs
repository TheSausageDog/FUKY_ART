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
    public static bool isHeldObj { protected set; get; } = false;
    [NonSerialized] public static Rigidbody heldObjRigidBody;
    [NonSerialized] public static bool holdingKnife;
    [NonSerialized] public static Vector3 knifeOrginPos;
    [NonSerialized] public static Vector3 moveLock;

    [Listen(EventType.OnKnifeTouchBegin)]
    private void OnKnifeTouchBegin(Vector3 dir)
    {
        moveLock = dir;
    }
    [Listen(EventType.OnKnifeTouchEnd)]
    private void OnKnifeTouchEnd()
    {
        moveLock = Vector3.zero;
    }

    [Listen(EventType.OnItemPicked)]
    private void OnItemPicked(BasePickableItem item)
    {
        isHeldObj = true;
    }
    [Listen(EventType.OnItemDrop)]
    private void OnItemDrop()
    {
        isHeldObj = false;
    }
}

[EventEnum]
public enum EventType
{
    OnKnifeTouchBegin,
    OnKnifeTouchEnd,
    OnItemPicked,
    OnItemDrop,
    OnPickingItem
}
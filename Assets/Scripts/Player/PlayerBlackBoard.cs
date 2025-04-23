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
    // [NonSerialized] public static Transform heldTrans;
    [NonSerialized] public static HoldableItem heldItem;
    // [NonSerialized] public static bool holdingKnife;
    // [NonSerialized] public static Vector3 knifeOrginPos;
    [NonSerialized] public static Vector3 moveLock;

    public float _handSpeed = 1;

    public static float handSpeed;

    void Start()
    {
        handSpeed = _handSpeed;
    }

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

    public static void OnItemHeld(HoldableItem item)
    {
        isHeldObj = true;
        heldItem = item;
        UEvent.Dispatch(EventType.OnItemHeld);
    }

    public static void OnItemDrop()
    {
        isHeldObj = false;
        UEvent.Dispatch(EventType.OnItemDrop);
    }
}

[EventEnum]
public enum EventType
{
    OnKnifeTouchBegin,
    OnKnifeTouchEnd,
    OnItemHeld,
    OnItemDrop,
    OnPickingItem,
}
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 黑板，用来存储玩家状态
/// </summary>
public class PlayerBlackBoard : MonoBehaviour
{
    [NonSerialized] public bool isHeldObj=false;
    [NonSerialized] public Rigidbody heldObjRigidBody;
}

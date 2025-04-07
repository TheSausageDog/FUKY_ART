using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 用来控制手的移动
/// </summary>
public class HandCursorMoveController : MonoBehaviour
{
    public Transform handTarget;
    public Transform pickUpPos;
    [Header("手移动的速度")]
    public float speed = 1f;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    // void Update()
    // {
    //     if (PlayerBlackBoard.isHeldObj)
    //     {
    //         hand.gameObject.SetActive(false);
    //     }
    //     else
    //     {
    //         hand.gameObject.SetActive(true);
    //         transform.position = Vector3.Lerp(transform.position, handTarget.position, speed * Time.deltaTime);
    //         transform.rotation = handTarget.rotation;
    //     }
    // }

    private void FixedUpdate()
    {
        if (PlayerBlackBoard.isHeldObj)
        {
            // 计算目标位置与当前物体位置之间的距离
            float distanceToTarget = Vector3.Distance(PlayerBlackBoard.heldObjRigidBody.transform.position, handTarget.position);

            // 计算目标位置与当前物体位置之间的方向
            Vector3 directionToTarget = (handTarget.position - PlayerBlackBoard.heldObjRigidBody.transform.position).normalized;

            // 动态调整速度：距离越远，速度越快
            float dynamicSpeed = speed * distanceToTarget;

            // 计算目标速度
            Vector3 targetVelocity = directionToTarget * dynamicSpeed;

            // 设置刚体速度
            PlayerBlackBoard.heldObjRigidBody.velocity = targetVelocity;

            // 同时调整手的位置，使其始终与物体位置对齐
            transform.position = transform.position - pickUpPos.position + PlayerBlackBoard.heldObjRigidBody.transform.position;

            // 保持手的旋转
            transform.rotation = handTarget.rotation;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, handTarget.position, speed * Time.deltaTime);
            transform.rotation = handTarget.rotation;
        }
    }
}

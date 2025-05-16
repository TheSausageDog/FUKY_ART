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
        transform.position = Vector3.Lerp(transform.position, handTarget.position, speed * Time.deltaTime);
        transform.rotation = handTarget.rotation;
    }
}

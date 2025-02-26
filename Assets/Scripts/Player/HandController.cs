using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// 用来控制手的移动
/// </summary>
public class HandController : MonoBehaviour
{
    public Transform hand;
    public Transform handTarget;
    public Transform pickUpPos;
    [Header("手移动的速度")]
    public float speed = 1f;

    [Header("按下Alt后手移动的灵敏度")]
    public float mouseSensitivity = 0.1f; // 鼠标灵敏度
    public float scrollSensitivity = 1f; // 滚轮灵敏度

    [Header("按下Alt后手可以移动的范围")]
    // 手部移动限制
    public Vector2 xMinMax;
    public Vector2 yMinMax;
    public Vector2 zMinMax;


    private PlayerInputController playerInputController;
    private PlayerBlackBoard playerBlackBoard;
    private Camera mainCamera;

    private Rigidbody handRb;
    
    private void Start()
    {
        playerInputController = GetComponent<PlayerInputController>();
        playerBlackBoard = GetComponent<PlayerBlackBoard>();
        mainCamera = Camera.main;
        handRb = hand.GetComponent<Rigidbody>();

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
        }
    }

    void Update()
    {

        if (playerBlackBoard.isHeldObj)
        {
            // var newPosition = Vector3.Lerp(playerBlackBoard.heldObjRigidBody.transform.position, handTarget.position, speed * Time.deltaTime*2);
            // playerBlackBoard.heldObjRigidBody.MovePosition(newPosition);
            //
            // hand.position = hand.position-pickUpPos.position+playerBlackBoard.heldObjRigidBody.transform.position;
            //
            // hand.rotation = handTarget.rotation;
            
            hand.gameObject.SetActive(false);
        }
        else
        {
            hand.gameObject.SetActive(true);
            hand.position = Vector3.Lerp(hand.position, handTarget.position, speed * Time.deltaTime);
            hand.rotation = handTarget.rotation;
        }
    
        // 控制 handTarget 的移动
        if(playerInputController.IsMoveHandHeld()|| playerBlackBoard.holdingKnife)MoveHandTarget();
    }

    private void FixedUpdate()
    {
        if (playerBlackBoard.isHeldObj)
        {
            // 计算目标位置与当前物体位置之间的距离
            float distanceToTarget = Vector3.Distance(playerBlackBoard.heldObjRigidBody.transform.position, handTarget.position);
        
            // 计算目标位置与当前物体位置之间的方向
            Vector3 directionToTarget = (handTarget.position - playerBlackBoard.heldObjRigidBody.transform.position).normalized;
        
            // 动态调整速度：距离越远，速度越快
            float dynamicSpeed = speed * distanceToTarget;

            // 计算目标速度
            Vector3 targetVelocity = directionToTarget * dynamicSpeed;

            // 设置刚体速度
            playerBlackBoard.heldObjRigidBody.velocity = targetVelocity;

            // 同时调整手的位置，使其始终与物体位置对齐
            hand.position = hand.position - pickUpPos.position + playerBlackBoard.heldObjRigidBody.transform.position;

            // 保持手的旋转
            hand.rotation = handTarget.rotation;
        }
    }



    private void MoveHandTarget()
    {
        if(mainCamera==null)return;
        // 如果没有持有物体，则允许鼠标移动 handTarget
        if (!playerBlackBoard.isHeldObj  || playerBlackBoard.holdingKnife)
        {
            // 获取鼠标输入
            Vector2 mouseInput = playerInputController.GetMouseInput();
            float scrollInput = playerInputController.GetScrollInput();

            // 计算基于相机本地坐标系的移动量
            Vector3 moveDelta = new Vector3(mouseInput.x * mouseSensitivity, mouseInput.y * mouseSensitivity, scrollInput * scrollSensitivity);

            // 将移动量转换为相机的本地空间
            Vector3 localMove = mainCamera.transform.TransformDirection(moveDelta);
            localMove.y = 0; // 如果不需要垂直方向的移动，可以保持 y 为 0

            // 更新 handTarget 的本地位置
            Vector3 newLocalPosition = mainCamera.transform.InverseTransformPoint(handTarget.position) + new Vector3(mouseInput.x * mouseSensitivity, mouseInput.y * mouseSensitivity, scrollInput * scrollSensitivity);
            
            // 限制 handTarget 的本地位置
            newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, xMinMax.x, xMinMax.y);
            newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, yMinMax.x, yMinMax.y);
            newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, zMinMax.x, zMinMax.y);

            // 将限制后的本地位置转换回世界坐标
            Vector3 clampedWorldPosition = mainCamera.transform.TransformPoint(newLocalPosition);
            handTarget.position = clampedWorldPosition;
        }
    }

    public void MoveHandTarget(Vector3 position)
    {
        handTarget.localPosition = position;
    }
}

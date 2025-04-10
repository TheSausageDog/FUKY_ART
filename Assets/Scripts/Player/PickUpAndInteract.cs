using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// 物品拾取与交互逻辑类
/// 处理玩家对物品的拾取、旋转、丢弃等操作。
/// </summary>
public class PickUpAndInteract : MonoBehaviour
{
    public Transform holdPos; // 物品持有位置
    public Transform handTarget;
    private Vector3 handTargetOffset;

    public float pickUpRange = 5f; // 拾取范围
    private float rotationSensitivity = 1f; // 旋转灵敏度

    private bool canDrop = true; // 是否可以丢弃物体

    private bool isHolding = false;

    private Camera _camera; // 主摄像机

    [Header("手部相关")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;

    public BasePickableItem currentHandObj; // 当前手部范围内的物体
    private float pickUpTimer = 0f; // 长按拾取计时器

    [Header("按下Alt后手移动的灵敏度")]
    public float mouseSensitivity = 0.1f; // 鼠标灵敏度
    public float scrollSensitivity = 1f; // 滚轮灵敏度

    [Header("按下Alt后手可以移动的范围")]
    // 手部移动限制
    public Vector2 xMinMax;
    public Vector2 yMinMax;
    public Vector2 zMinMax;



    private void Awake()
    {
        _camera = Camera.main;
        handTargetOffset = Vector3.forward;
        // handTarget.transform.localPosition;
    }

    void Update()
    {
        // 控制 handTarget 的移动
        if (PlayerInputController.IsMoveHandHeld())
        {
            if (PlayerInputController.IsLeftShiftPressed())
            {
                handTarget.localPosition = handTargetOffset;
            }
            else if (!PlayerInputController.IsRotateHeld())
            {
                MoveHandTarget();
            }
        }

        if (PlayerBlackBoard.isHeldObj)
        {
            BasePickableItem heldObj = PlayerBlackBoard.heldPickable;
            if (heldObj.interactionType == InteractionType.Check && !PlayerInputController.IsMoveHandHeld())
            {
                heldObj._transform.position = Camera.main.transform.position + heldObj._objectSize * Camera.main.transform.forward;
                heldObj._transform.LookAt(heldObj._transform.position + Camera.main.transform.up, -Camera.main.transform.forward);
                // handTarget.
            }

            if (PlayerInputController.IsRotateHeld())
            {
                float XaxisRotation = PlayerInputController.GetMouseInput().x * rotationSensitivity;
                float YaxisRotation = PlayerInputController.GetMouseInput().y * rotationSensitivity;
                heldObj._transform.Rotate(-CameraPos.up, XaxisRotation, Space.World);
                heldObj._transform.Rotate(CameraPos.right, YaxisRotation, Space.World);
                canDrop = false;
            }
            else
            {
                canDrop = true;
            }
            if (PlayerInputController.IsThrowPressed() && canDrop)
            {
                if (heldObj.interactionType == InteractionType.Check)
                {
                    handTarget.localPosition = handTargetOffset;
                }
                DropObject();
            }
        }
        else
        {
            HandlePickUpInput();
        }

        float progress = (currentHandObj != null && currentHandObj._pickDelay != 0) ?
            (float)pickUpTimer / currentHandObj._pickDelay : 0;
        UEvent.Dispatch(EventType.OnPickingItem, progress);
    }

    private void MoveHandTarget()
    {
        if (_camera == null) return;
        // 如果没有持有物体，则允许鼠标移动 handTarget
        // if (!PlayerBlackBoard.isHeldObj || PlayerBlackBoard.holdingKnife)
        // {
        // 获取鼠标输入
        Vector2 mouseInput = PlayerInputController.GetMouseInput();
        float scrollInput = PlayerInputController.GetScrollInput();
        // 计算基于相机本地坐标系的移动量
        Vector3 screen_move = new Vector3(mouseInput.x * mouseSensitivity, scrollInput * scrollSensitivity, mouseInput.y * mouseSensitivity);
        // Vector3 world_move = new Vector3(0, scrollInput * scrollSensitivity, 0);

        // bool needLock = false;

        if (PlayerBlackBoard.moveLock != Vector3.zero)
        {
            // needLock = true;

            // var length = moveDelta.y;

            // moveDelta = PlayerBlackBoard.moveLock * length;
            screen_move = Vector3.Project(screen_move, PlayerBlackBoard.moveLock);
        }
        // Debug.Log(needLock);
        // if (!needLock)
        // {
        // 更新 handTarget 的本地位置
        Vector3 newLocalPosition = transform.InverseTransformPoint(handTarget.position) + screen_move;

        // 限制 handTarget 的本地位置
        newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, xMinMax.x, xMinMax.y);
        newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, yMinMax.x, yMinMax.y);
        newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, zMinMax.x, zMinMax.y);

        // 将限制后的本地位置转换回世界坐标
        Vector3 clampedWorldPosition = transform.TransformPoint(newLocalPosition);
        handTarget.position = clampedWorldPosition;
        // }
        // else
        // {
        //     // 更新 handTarget 的本地位置
        //     Vector3 newLocalPosition = handTarget.position + moveDelta;

        //     var newCameraPos = mainCamera.transform.InverseTransformPoint(newLocalPosition);

        //     // 限制 handTarget 的本地位置
        //     newCameraPos.x = Mathf.Clamp(newCameraPos.x, xMinMax.x, xMinMax.y);
        //     newCameraPos.y = Mathf.Clamp(newCameraPos.y, yMinMax.x, yMinMax.y);
        //     newCameraPos.z = Mathf.Clamp(newCameraPos.z, zMinMax.x, zMinMax.y);

        //     Vector3 clampedWorldPosition = mainCamera.transform.TransformPoint(newCameraPos);
        //     handTarget.position = clampedWorldPosition;
        // }
        // }
    }

    /// <summary>
    /// 处理拾取输入
    /// </summary>
    void HandlePickUpInput()
    {
        if (currentHandObj != null)
        {
            if (PlayerInputController.IsPickUpPressing() && currentHandObj._pickDelay != 0)
            {
                pickUpTimer += Time.deltaTime;
                if (pickUpTimer >= currentHandObj._pickDelay)
                {
                    PickUpObject(currentHandObj);
                    currentHandObj = null;
                    pickUpTimer = 0f;
                }
            }
            else if (currentHandObj._pickDelay == 0 && PlayerInputController.IsPickUpPressed())
            {
                PickUpObject(currentHandObj);
                currentHandObj = null;
                pickUpTimer = 0f;
            }
            else
            {
                pickUpTimer = 0f;
            }
        }
        else
        {
            pickUpTimer = 0f;
        }
    }

    void PickUpObject(BasePickableItem pickUpObj)
    {
        if (Vector3.Distance(pickUpObj._transform.position, transform.position) > pickUpRange)
        {
            Debug.LogWarning("物体超出拾取范围！");
            return;
        }

        // if (heldObj.interactionType == InteractionType.Check)
        // {
        //     heldObj._cd.enabled = false;
        // }
        // else
        // {
        Physics.IgnoreCollision(pickUpObj._cd, GetComponent<Collider>(), true);
        // }
        pickUpObj.OnPickup(holdPos);
        PlayerBlackBoard.OnItemPicked(pickUpObj);


        _camera.DOFieldOfView(CameraFieldOfViewOffset, 0.5f);

        if (pickUpObj is Knife)
        {
            PlayerBlackBoard.holdingKnife = true;
            PlayerBlackBoard.knifeOrginPos = handTarget.localPosition;
        }
    }

    void DropObject()
    {
        if (PlayerBlackBoard.heldPickable is Knife)
        {
            PlayerBlackBoard.holdingKnife = false;
            handTarget.localPosition = PlayerBlackBoard.knifeOrginPos;
        }

        // if (heldObj.interactionType == InteractionType.Check)
        // {
        //     heldObj._cd.enabled = true;
        // }
        // else
        // {
        Physics.IgnoreCollision(PlayerBlackBoard.heldPickable._cd, GetComponent<Collider>(), false);
        // }

        PlayerBlackBoard.heldPickable.OnThrow();
        PlayerBlackBoard.OnItemDrop();
        _camera.DOFieldOfView(CameraFieldOfViewOrgin, 0.5f);
    }

    private void OnDrawGizmos()
    {
        if (handPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickUpRange);
        }
    }

    public void OnHandTriggerEnter(GameObject other)
    {
        if (other.CompareTag("canInteract"))
        {
            var item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                if (currentHandObj == null)
                {
                    currentHandObj = item;
                    item.OnHandEnter();
                }
                else
                {
                    OnHandTriggerExit(currentHandObj._transform.gameObject);
                    currentHandObj = item;
                    item.OnHandEnter();
                }
            }
        }
    }

    public void OnHandTriggerExit(GameObject other)
    {
        if (other.CompareTag("canInteract"))
        {
            var item = other.GetComponent<BasePickableItem>();
            if (item != null && item == (BasePickableItem)currentHandObj)
            {
                item.OnHandExit();
                currentHandObj = null;
            }
        }
    }
}

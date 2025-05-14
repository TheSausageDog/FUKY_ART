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
public class PickUpAndInteract : SingletonMono<PickUpAndInteract>
{
    private Vector3 handTargetOffset;

    public float pickUpRange = 5f; // 拾取范围
    private float rotationSensitivity = 1f; // 旋转灵敏度

    // private bool isHolding = false;

    public GameObject uiCursor;

    [Header("手部相关")]
    [SerializeField] private Transform holdPos; // 物品持有位置
    [SerializeField] private Transform handTarget;
    [SerializeField] private float CameraFieldOfViewOrgin = 0;
    [SerializeField] private float CameraFieldOfViewOffset = 0;
    [SerializeField] private float maxHandMovingSpeed = 5;

    protected GameObject selectedObj; // 当前手部范围内的物体
    // private float pickUpTimer = 0f; // 长按拾取计时器

    [Header("按下Alt后手移动的灵敏度")]
    public float mouseSensitivity = 0.1f; // 鼠标灵敏度
    public float scrollSensitivity = 1f; // 滚轮灵敏度

    [Header("按下Alt后手可以移动的范围")]
    // 手部移动限制
    public Vector2 xMinMax;
    public Vector2 yMinMax;
    public Vector2 zMinMax;

    protected override void Awake()
    {
        base.Awake();
        handTargetOffset = Vector3.forward * 1f;
        handTarget.localPosition = handTargetOffset;
        // handTarget.transform.localPosition;
    }

    public void OnHandTriggerEnter(GameObject other)
    {
        if (other.CompareTag("canInteract"))
        {
            OnHandTriggerExit();
            selectedObj = other;
            Utils.SetLayerRecursive(selectedObj.transform, "Outline");
        }
    }

    public void OnHandTriggerExit()
    {
        if (selectedObj != null)
        {
            Utils.SetLayerRecursive(selectedObj.transform, "Default");
            selectedObj = null;
        }
    }

    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        Vector3 dir = Camera.main.transform.forward;
        // funkyControl ? (fukyCursor.transform.position - Camera.main.transform.position) : Camera.main.transform.forward;
        int count = Physics.RaycastNonAlloc(Camera.main.transform.position, dir, hits, pickUpRange);

        Array.Sort(hits, 0, count, Comparer<RaycastHit>.Create((a, b) => a.distance.CompareTo(b.distance)));

        for (int i = 0; i < count; i++)
        {
            if (hits[i].collider.gameObject.CompareTag("canInteract"))
            {
                OnHandTriggerEnter(hits[i].collider.gameObject);
                //                Debug.Log(hits[i].collider.gameObject.name);
                return;
            }
        }
        OnHandTriggerExit();
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
            float distanceToTarget = Vector3.Distance(handTarget.position, holdPos.position);
            if (distanceToTarget > maxHandMovingSpeed * Time.deltaTime)
            {
                Vector3 directionToTarget = (handTarget.position - holdPos.position).normalized;
                holdPos.position += directionToTarget * maxHandMovingSpeed * Time.deltaTime;
            }
            else
            {
                holdPos.position = handTarget.position;
            }

            if (PlayerInputController.IsInteractPressed() && PlayerBlackBoard.heldItem.isInteractable)
            {
                PlayerBlackBoard.heldItem.OnInteract();
            }
            else if (PlayerInputController.IsRotateHeld())
            {
                HoldableItem heldObj = PlayerBlackBoard.heldItem;
                Vector2 mouseInput = PlayerInputController.GetMouseInput() * rotationSensitivity;
                float scrollInput = PlayerInputController.GetScrollInput() * 10;

                heldObj.transform.Rotate(Vector3.up, mouseInput.x, Space.Self);
                heldObj.transform.Rotate(Vector3.right, mouseInput.y, Space.Self);
                heldObj.transform.Rotate(Vector3.forward, scrollInput, Space.Self);
            }
            else if (PlayerInputController.IsThrowPressed())
            {
                DropObject();
            }
        }
        else
        {
            HandlePickUpInput();
        }
    }

    private void MoveHandTarget()
    {
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

        // if (!needLock)
        // {
        // 更新 handTarget 的本地位置
        Vector3 newLocalPosition = transform.InverseTransformPoint(handTarget.position) + screen_move;

        // 限制 handTarget 的本地位置
        newLocalPosition.x = Mathf.Clamp(newLocalPosition.x, xMinMax.x, xMinMax.y);
        newLocalPosition.y = Mathf.Clamp(newLocalPosition.y, yMinMax.x, yMinMax.y);
        newLocalPosition.z = Mathf.Clamp(newLocalPosition.z, zMinMax.x, zMinMax.y);

        Vector3 clampedWorldPosition = transform.TransformPoint(newLocalPosition);
        handTarget.position = clampedWorldPosition;
    }

    /// <summary>
    /// 处理拾取输入
    /// </summary>
    void HandlePickUpInput()
    {
        //暂时先不需要按住触发
        // if (selectedObj != null)
        // {
        //     if (PlayerInputController.IsPickUpPressing() && selectedObj._pickDelay != 0)
        //     {
        //         pickUpTimer += Time.deltaTime;
        //         if (pickUpTimer >= selectedObj._pickDelay)
        //         {
        //             PickUpObject(selectedObj);
        //             selectedObj = null;
        //             pickUpTimer = 0f;
        //         }
        //     }
        //     else if (selectedObj._pickDelay == 0 && PlayerInputController.IsPickUpPressed())
        //     {
        //         PickUpObject(selectedObj);
        //         selectedObj = null;
        //         pickUpTimer = 0f;
        //     }
        //     else
        //     {
        //         pickUpTimer = 0f;
        //     }
        // }
        // else
        // {
        //     pickUpTimer = 0f;
        // }
        // float progress = (selectedObj != null && selectedObj._pickDelay != 0) ?
        //     (float)pickUpTimer / selectedObj._pickDelay : 0;
        // UEvent.Dispatch(EventType.OnPickingItem, progress);

        if (selectedObj != null)
        {
            if (Vector3.Distance(selectedObj.transform.position, transform.position) > pickUpRange)
            {
                Debug.LogWarning("物体超出拾取范围！");
                return;
            }
            if (PlayerInputController.IsPickUpPressed())
            {
                if (selectedObj.TryGetComponent<HoldableItem>(out var holdItemScript))
                {
                    OnHandTriggerExit();
                    PickObject(holdItemScript);
                }
                else if (selectedObj.TryGetComponent<InteractItemBase>(out var interactItemScript)) { interactItemScript.OnInteract(); }
            }
            else if (PlayerInputController.IsInteractPressed())
            {
                if (selectedObj.TryGetComponent<InteractItemBase>(out var interactItemScript)) { interactItemScript.OnInteract(); }
            }
        }
    }

    public void PickObject(HoldableItem pickItem)
    {
        uiCursor.SetActive(false);
        holdPos.transform.position = pickItem.transform.position;
        pickItem.transform.eulerAngles = Vector3.zero;
        pickItem.OnPickup(holdPos);
        pickItem.gameObject.tag = "isPicking";
        Utils.SetLayerRecursive(pickItem.transform, "Player");
        Physics.IgnoreCollision(pickItem.itemCollider, GetComponent<Collider>(), true);

        PlayerBlackBoard.OnItemHeld(pickItem);

        Camera.main.DOFieldOfView(CameraFieldOfViewOffset, 0.5f);
    }

    public void DropObject()
    {
        uiCursor.SetActive(true);
        PlayerBlackBoard.heldItem.OnThrow();
        Physics.IgnoreCollision(PlayerBlackBoard.heldItem.itemCollider, GetComponent<Collider>(), false);
        PlayerBlackBoard.heldItem.gameObject.tag = "canInteract";
        Utils.SetLayerRecursive(PlayerBlackBoard.heldItem.transform, "Default");
        PlayerBlackBoard.OnItemDrop();

        Camera.main.DOFieldOfView(CameraFieldOfViewOrgin, 0.5f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
        Gizmos.color = Color.green;

        Vector3 size = new Vector3(Mathf.Abs(xMinMax.x - xMinMax.y), Mathf.Abs(yMinMax.x - yMinMax.y), Mathf.Abs(zMinMax.x - zMinMax.y));
        Vector3 center = new Vector3(xMinMax.x + xMinMax.y, yMinMax.x + yMinMax.y, zMinMax.x + zMinMax.y) * 0.5f;
        Gizmos.DrawWireCube(transform.TransformPoint(center), size);


        Gizmos.color = Color.red;
        Gizmos.DrawSphere(handTarget.position, 0.1f);
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(holdPos.position, 0.1f);
    }
}

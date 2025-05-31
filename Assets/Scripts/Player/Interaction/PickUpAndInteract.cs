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
[RequireComponent(typeof(InteractionConfig))]
public abstract class PickUpAndInteract : SingletonMono<PickUpAndInteract>
{
    protected InteractionConfig data;

    protected GameObject selectedObj; // 当前手部范围内的物体
                                      // private float pickUpTimer = 0f; // 长按拾取计时器

    protected abstract Ray GetPickUpRay { get; }


    protected void Awake()
    {
        data = GetComponent<InteractionConfig>();
        data.handTarget.localPosition = data.handTargetOffset;
        // handTarget.transform.localPosition;
    }


    public void OnHandTriggerEnter(GameObject other)
    {
        if (other.CompareTag("canInteract"))
        {
            OnHandTriggerExit();
            selectedObj = other;
            if (selectedObj.layer != LayerMask.NameToLayer("OutlineSecond"))
                Utils.SetLayerRecursive(selectedObj.transform, "Outline");
        }
    }

    public void OnHandTriggerExit()
    {
        if (selectedObj != null)
        {
            if (selectedObj.layer != LayerMask.NameToLayer("OutlineSecond"))
                Utils.SetLayerRecursive(selectedObj.transform, "Default");
            selectedObj = null;
        }
    }

    RaycastHit[] hits = new RaycastHit[128];
    private void FixedUpdate()
    {
        // funkyControl ? (fukyCursor.transform.position - Camera.main.transform.position) : Camera.main.transform.forward;
        int count = Physics.RaycastNonAlloc(GetPickUpRay, hits, data.pickUpRange);
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

    protected virtual void Update()
    {
        MoveHandTarget();
        // 控制 handTarget 的移动

        if (PlayerBlackBoard.isHeldObj)
        {
            float distanceToTarget = Vector3.Distance(data.handTarget.position, data.holdPos.position);
            // if (distanceToTarget > data.maxHandMovingSpeed * Time.deltaTime)
            // {
            //     Vector3 directionToTarget = (data.handTarget.position - data.holdPos.position).normalized;
            //     data.holdPos.position += directionToTarget * data.maxHandMovingSpeed * Time.deltaTime;
            // }
            // else
            // {
            data.holdPos.position = data.handTarget.position;
            // }

            if (PlayerInputController.Instance.IsInteractPressed() && PlayerBlackBoard.heldItem.isInteractable)
            {
                PlayerBlackBoard.heldItem.OnInteract();
            }
            else if (PlayerInputController.Instance.IsThrowPressed())
            {
                DropObject();
            }
        }
        else
        {
            HandlePickUpInput();
        }
    }

    protected abstract void MoveHandTarget();

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
            // if (Vector3.Distance(selectedObj.transform.position, Camera.main.transform.position) > data.pickUpRange)
            // {
            //     // Debug.LogWarning("物体超出拾取范围！");
            //     return;
            // }
            if (PlayerInputController.Instance.IsPickUpPressed())
            {
                if (selectedObj.TryGetComponent<HoldableItem>(out var holdItemScript))
                {
                    OnHandTriggerExit();
                    PickObject(holdItemScript);
                }
                else if (selectedObj.TryGetComponent<InteractItemBase>(out var interactItemScript)) { interactItemScript.OnInteract(); }
            }
            else if (PlayerInputController.Instance.IsInteractPressed())
            {
                if (selectedObj.TryGetComponent<InteractItemBase>(out var interactItemScript)) { interactItemScript.OnInteract(); }
            }
        }
    }

    public virtual void PickObject(HoldableItem pickItem)
    {
        data.holdPos.position = pickItem.transform.position;
        pickItem.OnPickup(data.holdPos);
        data.holdPos.rotation = pickItem.transform.rotation;
        pickItem.gameObject.tag = "isPicking";
        Utils.SetLayerRecursive(pickItem.transform, "Player");
        Physics.IgnoreCollision(pickItem.itemCollider, GetComponent<Collider>(), true);

        PlayerBlackBoard.OnItemHeld(pickItem);

        Camera.main.DOFieldOfView(data.CameraFieldOfViewOffset, 0.5f);
    }

    public virtual void DropObject()
    {
        PlayerBlackBoard.heldItem.OnThrow();
        Physics.IgnoreCollision(PlayerBlackBoard.heldItem.itemCollider, GetComponent<Collider>(), false);
        PlayerBlackBoard.heldItem.gameObject.tag = "canInteract";
        Utils.SetLayerRecursive(PlayerBlackBoard.heldItem.transform, "Default");
        PlayerBlackBoard.OnItemDrop();

        Camera.main.DOFieldOfView(data.CameraFieldOfViewOrgin, 0.5f);
    }

    protected virtual void OnDrawGizmos()
    {
        if (data == null) { data = GetComponent<InteractionConfig>(); }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.pickUpRange);
        Gizmos.color = Color.green;

        Vector3 size = new Vector3(Mathf.Abs(data.xMinMax.x - data.xMinMax.y), Mathf.Abs(data.yMinMax.x - data.yMinMax.y), Mathf.Abs(data.zMinMax.x - data.zMinMax.y));
        Vector3 center = new Vector3(data.xMinMax.x + data.xMinMax.y, data.yMinMax.x + data.yMinMax.y, data.zMinMax.x + data.zMinMax.y) * 0.5f;
        Gizmos.DrawWireCube(transform.TransformPoint(center), size);

        if (data.handTarget != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(data.handTarget.position, 0.05f);
        }
        if (data.holdPos != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(data.holdPos.position, 0.05f);
        }
    }
}

public abstract class MousePickUpAndInteract : PickUpAndInteract
{
    public GameObject uiCursor;

    protected override Ray GetPickUpRay => new Ray(Camera.main.transform.position, Camera.main.transform.forward);

    protected override void MoveHandTarget()
    {
        if (PlayerInputController.Instance.IsLeftShiftPressed())
        {
            if (PlayerInputController.Instance.IsMoveHandHeld())
            {
                data.handTarget.position = data.holdPos.position;
            }
            else
            {
                data.handTarget.localPosition = data.handTargetOffset;
            }
        }
    }

    public override void PickObject(HoldableItem pickItem)
    {
        uiCursor.SetActive(false);
        base.PickObject(pickItem);
    }

    public override void DropObject()
    {
        uiCursor.SetActive(true);
        base.DropObject();
    }
}
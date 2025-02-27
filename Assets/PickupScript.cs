using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    // pickUpRange 现在仅用于调试绘制Gizmos或设置Trigger范围，不再用来做检测
    public float pickUpRange = 5f;

    private float rotationSensitivity = 1f;

    private BasePickableItem heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private PlayerInputController inputController; // 引用 PlayerInputController
    private HandController _handController;
    private Camera _camera;

    [Header("手部相关")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;
    
    private BasePickableItem currentHandObj; // 当前处于手部范围内的物体

    private void Awake()
    {
        _camera = Camera.main;
        _handController = GetComponent<HandController>();
    }

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");

        // 获取输入控制器
        inputController = GetComponent<PlayerInputController>();
        if (inputController == null)
        {
            Debug.LogError("PlayerInputController is missing!");
        }
        
        playerBlackBoard = GetComponent<PlayerBlackBoard>();
    }
    
    void Update()
    {
        if (inputController == null) return;

        if (heldObj != null)
        {
            // 持有物体时执行旋转逻辑
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                //StopClipping();
                DropObject();   
            }
        }
        else
        {
            // 当按下拾取按钮且当前有目标物体时拾取
            if (inputController.IsPickUpPressed() && currentHandObj != null)
            {
                PickUpObject(currentHandObj);
                currentHandObj = null; // 拾取后清空当前手部目标
            }
        }
    }

    // 使用Trigger检测进入拾取范围的物体
    public void OnHandTriggerEnter(Collider other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // 如果当前没有目标，则设置并调用 OnHandEnter
                if (currentHandObj == null)
                {
                    currentHandObj = item;
                    currentHandObj.OnHandEnter();
                }
            }
        }
    }

    // 当物体离开触发区域时
    public void OnHandTriggerExit(Collider other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null && item == currentHandObj)
            {
                currentHandObj.OnHandExit();
                currentHandObj = null;
            }
        }
    }

    void PickUpObject(BasePickableItem pickUpObj)
    {
        heldObj = pickUpObj;
        heldObjRb = heldObj.rb;
        heldObj.OnPickup(holdPos);
        heldObj.gameObject.layer = LayerNumber;
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        playerBlackBoard.isHeldObj = true;
        playerBlackBoard.heldObjRigidBody = heldObjRb;
        _camera.DOFieldOfView(CameraFieldOfViewOffset, 0.5f);

        if (pickUpObj is Knife)
        {
            playerBlackBoard.holdingKnife = true;
            playerBlackBoard.knifeOrginPos = _handController.handTarget.localPosition;
        }
    }

    void DropObject()
    {
        if (heldObj is Knife)
        {
            playerBlackBoard.holdingKnife = false;
            _handController.MoveHandTarget(playerBlackBoard.knifeOrginPos);
        }

        heldObj.OnThrow();
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.gameObject.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.freezeRotation = false;
        heldObjRb.useGravity = true;
        heldObj.transform.parent = null;
        heldObj = null;
        playerBlackBoard.isHeldObj = false;
        playerBlackBoard.heldObjRigidBody = null;
        _camera.DOFieldOfView(CameraFieldOfViewOrgin, 0.5f);
    }

    void RotateObject()
    {
        if (inputController.IsRotateHeld())
        {
            canDrop = false;
            float XaxisRotation = inputController.GetMouseInput().x * rotationSensitivity;
            float YaxisRotation = inputController.GetMouseInput().y * rotationSensitivity;
            heldObj.transform.Rotate(-CameraPos.up, XaxisRotation, Space.World);
            heldObj.transform.Rotate(CameraPos.right, YaxisRotation, Space.World);
        }
        else
        {
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.gameObject.layer = 0;
        heldObjRb.isKinematic = false;
        heldObjRb.freezeRotation = false;
        heldObjRb.useGravity = true;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
        playerBlackBoard.isHeldObj = false;
        playerBlackBoard.heldObjRigidBody = null;
    }

    void StopClipping()
    {
        // 如果需要，添加防止穿模的逻辑
    }

    // 可选：在编辑器中绘制拾取范围，方便调试
    private void OnDrawGizmos()
    {
        if(handPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(handPos.position, pickUpRange);
        }
    }
}

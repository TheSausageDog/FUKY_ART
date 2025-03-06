using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 1f;

    private BasePickableItem heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private PlayerInputController inputController;
    private HandController _handController;
    private Camera _camera;

    [Header("手部相关")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;
    
    // 当前处于手部范围内的目标物体
    public BasePickableItem currentHandObj; 

    // 拾取和备用交互的长按计时器
    private float pickUpTimer = 0f;
    private float alternateActionTimer = 0f;
    // 备用交互长按阈值（单位秒），可根据需求调整或由物体自定义
    public float alternateActionDelay = 1.0f; 

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

        // 持有物体时处理旋转、丢弃等逻辑
        if (heldObj != null)
        {
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                DropObject();   
            }
        }
        else
        {
            // 未持有物体时检测拾取输入
            HandlePickUpInput();
            // 检测备用交互输入（例如对准物品后点击其它按键）
            HandleAlternateActionInput();
        }
        
        // 分发拾取进度事件（用于UI显示等）
        if (currentHandObj != null && currentHandObj.PickDelay != 0)
            UEvent.Dispatch(EventType.OnPickingItem, (float)pickUpTimer / currentHandObj.PickDelay);
        else 
            UEvent.Dispatch(EventType.OnPickingItem, 0f);
    }

    /// <summary>
    /// 当物体进入手部拾取触发区域时调用
    /// </summary>
    public void OnHandTriggerEnter(GameObject other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // 如果已有目标则先退出上一个目标，再设置新的目标
                if (currentHandObj != null)
                {
                    OnHandTriggerExit(currentHandObj.gameObject);
                }
                currentHandObj = item;
                currentHandObj.OnHandEnter();
            }
        }
    }

    /// <summary>
    /// 当物体离开手部拾取触发区域时调用
    /// </summary>
    public void OnHandTriggerExit(GameObject other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null && item == currentHandObj)
            {
                currentHandObj.OnHandExit();
                currentHandObj = null;
                // 可根据需要重置计时器
                pickUpTimer = 0f;
                alternateActionTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 检测拾取输入：支持点击和长按拾取
    /// </summary>
    void HandlePickUpInput()
    {
        if (currentHandObj != null)
        {
            if (inputController.IsPickUpPressing() && currentHandObj.PickDelay != 0)
            {
                pickUpTimer += Time.deltaTime;
                
                if (pickUpTimer >= currentHandObj.PickDelay)
                {
                    PickUpObject(currentHandObj);
                    currentHandObj = null;
                    pickUpTimer = 0f;
                }
            }
            else if (currentHandObj.PickDelay == 0 && inputController.IsPickUpPressed())
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
    
    /// <summary>
    /// 检测备用交互输入：支持短按和长按两种交互，后续可扩展更多操作
    /// </summary>
    void HandleAlternateActionInput()
    {
        if (currentHandObj != null)
        {
            // 检测备用交互短按（例如按下某个专用键）
            if (inputController.IsAlternateActionPressed())
            {
                currentHandObj.OnAlternateAction();
            }
            
            // 检测备用交互长按
            if (inputController.IsAlternateActionPressing())
            {
                alternateActionTimer += Time.deltaTime;
                if (alternateActionTimer >= alternateActionDelay)
                {
                    currentHandObj.OnAlternateActionLongPress();
                    alternateActionTimer = 0f; // 长按触发后重置计时器
                }
            }
            else
            {
                alternateActionTimer = 0f;
            }
        }
        else
        {
            alternateActionTimer = 0f;
        }
    }
    
    /// <summary>
    /// 执行拾取物体逻辑
    /// </summary>
    /// <param name="pickUpObj">待拾取的物体</param>
    void PickUpObject(BasePickableItem pickUpObj)
    {
        // 检查物体是否在允许拾取的范围内
        if (Vector3.Distance(pickUpObj.transform.position, transform.position) > pickUpRange)
        {
            Debug.LogWarning("物体超出拾取范围！");
            return;
        }
        
        heldObj = pickUpObj;
        heldObjRb = heldObj.rb;
        
        // 调用物体自身的拾取逻辑
        heldObj.OnPickup(holdPos);
        heldObj.gameObject.layer = LayerNumber;
        
        // 忽略玩家与物体的碰撞
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        playerBlackBoard.isHeldObj = true;
        playerBlackBoard.heldObjRigidBody = heldObjRb;
        
        // 调整相机视野
        _camera.DOFieldOfView(CameraFieldOfViewOffset, 0.5f);

        // 示例：如果拾取的是Knife，进行特殊处理
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

    private void OnDrawGizmos()
    {
        if (handPos != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, pickUpRange);
        }
    }
}

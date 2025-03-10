using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

public class PickUpAndInteract : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    // public float throwForce = 500f;

    public float pickUpRange = 5f;

    private float rotationSensitivity = 1f;

    private IInteractable heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;


    private PlayerInputController inputController; // 引用 PlayerInputController
    private HandController _handController;
    private Camera _camera;

    [Header("手部相关")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;
    
    public IInteractable currentHandObj; // 当前处于手部范围内的物体

    // 用于计时长按拾取的计时器
    private float pickUpTimer = 0f;

    private void Awake()
    {
        _camera = Camera.main;
        _handController = GetComponent<HandController>();
    }

    void Start()
    {


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
            // 已持有物体时，允许旋转等操作
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                DropObject();   
            }
        }
        else
        {
            HandlePickUpInput();
        }
        if(currentHandObj!=null && currentHandObj._pickDelay!=0)UEvent.Dispatch(EventType.OnPickingItem,(float)pickUpTimer/currentHandObj._pickDelay);
        else UEvent.Dispatch(EventType.OnPickingItem,(float)0);
        
    }

    /// <summary>
    /// 处理拾取输入：当当前手部目标存在且鼠标持续按下时，累计时间，达到目标的PickDelay后拾取
    /// </summary>
    void HandlePickUpInput()
    {
        if (currentHandObj != null)
        {
            if (inputController.IsAlternateActionPressed())
            {
                currentHandObj.Interact(InteractionType.Interact);
            }
            
            // 如果鼠标拾取键按下，则累计时间
            if (inputController.IsPickUpPressing()&& currentHandObj._pickDelay!=0)
            {
                pickUpTimer += Time.deltaTime;
                
                // 当累计时间达到物体要求的延时后执行拾取
                if (pickUpTimer >= currentHandObj._pickDelay)
                {
                    PickUpObject(currentHandObj);
                    // 拾取后清空目标和重置计时器
                    currentHandObj = null;
                    pickUpTimer = 0f;

                }

            }
            else if (currentHandObj._pickDelay == 0&&inputController.IsPickUpPressed())
            {
                PickUpObject(currentHandObj);
                // 拾取后清空目标和重置计时器
                currentHandObj = null;
                pickUpTimer = 0f;

            }
            else
            {
                // 如果中途松开，则重置计时器
                pickUpTimer = 0f;

            }


        }
        else
        {
            // 没有目标时也重置计时器
            pickUpTimer = 0f;

        }
        
        
    }

    // 使用Trigger检测进入拾取范围的物体
    public void OnHandTriggerEnter(GameObject other)
    {
//        Debug.Log(other.name);
        if (other.CompareTag("canInteract"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // 如果当前没有目标，则直接设置并调用 OnHandEnter
                if (currentHandObj == null)
                {
                    currentHandObj = item;
                    item.OnHandEnter();
                }
                else
                {
                    // 如果已有目标，则先退出上一个目标，再设置新的目标
                    OnHandTriggerExit(currentHandObj._transform.gameObject);
                    currentHandObj = item;
                    item.OnHandEnter();
                }
            }
        }
    }

    // 当物体离开触发区域时
    public void OnHandTriggerExit(GameObject other)
    {
        if (other.CompareTag("canInteract"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null && item == currentHandObj)
            {
                item.OnHandExit();
                currentHandObj = null;
                //pickUpTimer = 0f;
            }
        }
    }
    
    /// <summary>
    /// 执行拾取物体逻辑
    /// </summary>
    /// <param name="pickUpObj">待拾取的物体，需满足范围条件</param>
    void PickUpObject(IInteractable pickUpObj)
    {
        // 检查物体是否在允许拾取的范围内
        if (Vector3.Distance(pickUpObj._transform.position, transform.position) > pickUpRange)
        {
            Debug.LogWarning("物体超出拾取范围！");
            return;
        }
        
        heldObj = pickUpObj;
        heldObjRb = heldObj._rb;
        
        // 调用物体自身的拾取逻辑（例如设置父物体、禁用物理等）
        heldObj.Interact(InteractionType.Pick,holdPos,this);
        

        playerBlackBoard.isHeldObj = true;
        playerBlackBoard.heldObjRigidBody = heldObjRb;
        
        // 调整相机视野
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

        heldObj.Interact(InteractionType.Throw,this);

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
            heldObj._transform.Rotate(-CameraPos.up, XaxisRotation, Space.World);
            heldObj._transform.Rotate(CameraPos.right, YaxisRotation, Space.World);
        }
        else
        {
            canDrop = true;
        }
    }

    // void ThrowObject()
    // {
    //     Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
    //     heldObj.gameObject.layer = 0;
    //     heldObjRb.isKinematic = false;
    //     heldObjRb.freezeRotation = false;
    //     heldObjRb.useGravity = true;
    //     heldObj.transform.parent = null;
    //     heldObjRb.AddForce(transform.forward * throwForce);
    //     heldObj = null;
    //     playerBlackBoard.isHeldObj = false;
    //     playerBlackBoard.heldObjRigidBody = null;
    // }

    void StopClipping()
    {
        // 实现防止穿模的逻辑（如有需要）
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
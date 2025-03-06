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
    // public float throwForce = 500f;

    public float pickUpRange = 5f;

    private float rotationSensitivity = 1f;

    private BasePickableItem heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private PlayerInputController inputController; // ���� PlayerInputController
    private HandController _handController;
    private Camera _camera;

    [Header("�ֲ����")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;
    
    public BasePickableItem currentHandObj; // ��ǰ�����ֲ���Χ�ڵ�����

    // ���ڼ�ʱ����ʰȡ�ļ�ʱ��
    private float pickUpTimer = 0f;

    private void Awake()
    {
        _camera = Camera.main;
        _handController = GetComponent<HandController>();
    }

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer");

        // ��ȡ���������
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
            // �ѳ�������ʱ��������ת�Ȳ���
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
        if(currentHandObj!=null && currentHandObj.PickDelay!=0)UEvent.Dispatch(EventType.OnPickingItem,(float)pickUpTimer/currentHandObj.PickDelay);
        else UEvent.Dispatch(EventType.OnPickingItem,(float)0);
        
    }

    /// <summary>
    /// ����ʰȡ���룺����ǰ�ֲ�Ŀ�����������������ʱ���ۼ�ʱ�䣬�ﵽĿ���PickDelay��ʰȡ
    /// </summary>
    void HandlePickUpInput()
    {
        if (currentHandObj != null)
        {
            // ������ʰȡ�����£����ۼ�ʱ��
            if (inputController.IsPickUpPressing()&& currentHandObj.PickDelay!=0)
            {
                pickUpTimer += Time.deltaTime;
                
                // ���ۼ�ʱ��ﵽ����Ҫ�����ʱ��ִ��ʰȡ
                if (pickUpTimer >= currentHandObj.PickDelay)
                {
                    PickUpObject(currentHandObj);
                    // ʰȡ�����Ŀ������ü�ʱ��
                    currentHandObj = null;
                    pickUpTimer = 0f;

                }

            }
            else if (currentHandObj.PickDelay == 0&&inputController.IsPickUpPressed())
            {
                PickUpObject(currentHandObj);
                // ʰȡ�����Ŀ������ü�ʱ��
                currentHandObj = null;
                pickUpTimer = 0f;

            }
            else
            {
                // �����;�ɿ��������ü�ʱ��
                pickUpTimer = 0f;

            }
        }
        else
        {
            // û��Ŀ��ʱҲ���ü�ʱ��
            pickUpTimer = 0f;

        }
    }

    // ʹ��Trigger������ʰȡ��Χ������
    public void OnHandTriggerEnter(GameObject other)
    {
//        Debug.Log(other.name);
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // �����ǰû��Ŀ�꣬��ֱ�����ò����� OnHandEnter
                if (currentHandObj == null)
                {
                    currentHandObj = item;
                    currentHandObj.OnHandEnter();
                }
                else
                {
                    // �������Ŀ�꣬�����˳���һ��Ŀ�꣬�������µ�Ŀ��
                    OnHandTriggerExit(currentHandObj.gameObject);
                    currentHandObj = item;
                    currentHandObj.OnHandEnter();
                }
            }
        }
    }

    // �������뿪��������ʱ
    public void OnHandTriggerExit(GameObject other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null && item == currentHandObj)
            {
                currentHandObj.OnHandExit();
                currentHandObj = null;
                //pickUpTimer = 0f;
            }
        }
    }

    /// <summary>
    /// ִ��ʰȡ�����߼�
    /// </summary>
    /// <param name="pickUpObj">��ʰȡ�����壬�����㷶Χ����</param>
    void PickUpObject(BasePickableItem pickUpObj)
    {
        // ��������Ƿ�������ʰȡ�ķ�Χ��
        if (Vector3.Distance(pickUpObj.transform.position, transform.position) > pickUpRange)
        {
            Debug.LogWarning("���峬��ʰȡ��Χ��");
            return;
        }
        
        heldObj = pickUpObj;
        heldObjRb = heldObj.rb;
        
        // �������������ʰȡ�߼����������ø����塢��������ȣ�
        heldObj.OnPickup(holdPos);
        heldObj.gameObject.layer = LayerNumber;
        
        // ����������������ײ
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        playerBlackBoard.isHeldObj = true;
        playerBlackBoard.heldObjRigidBody = heldObjRb;
        
        // ���������Ұ
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
        // ʵ�ַ�ֹ��ģ���߼���������Ҫ��
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
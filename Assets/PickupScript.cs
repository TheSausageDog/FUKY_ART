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

    [Header("�ֲ����")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;
    
    // ��ǰ�����ֲ���Χ�ڵ�Ŀ������
    public BasePickableItem currentHandObj; 

    // ʰȡ�ͱ��ý����ĳ�����ʱ��
    private float pickUpTimer = 0f;
    private float alternateActionTimer = 0f;
    // ���ý���������ֵ����λ�룩���ɸ�������������������Զ���
    public float alternateActionDelay = 1.0f; 

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

        // ��������ʱ������ת���������߼�
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
            // δ��������ʱ���ʰȡ����
            HandlePickUpInput();
            // ��ⱸ�ý������루�����׼��Ʒ��������������
            HandleAlternateActionInput();
        }
        
        // �ַ�ʰȡ�����¼�������UI��ʾ�ȣ�
        if (currentHandObj != null && currentHandObj.PickDelay != 0)
            UEvent.Dispatch(EventType.OnPickingItem, (float)pickUpTimer / currentHandObj.PickDelay);
        else 
            UEvent.Dispatch(EventType.OnPickingItem, 0f);
    }

    /// <summary>
    /// ����������ֲ�ʰȡ��������ʱ����
    /// </summary>
    public void OnHandTriggerEnter(GameObject other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // �������Ŀ�������˳���һ��Ŀ�꣬�������µ�Ŀ��
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
    /// �������뿪�ֲ�ʰȡ��������ʱ����
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
                // �ɸ�����Ҫ���ü�ʱ��
                pickUpTimer = 0f;
                alternateActionTimer = 0f;
            }
        }
    }

    /// <summary>
    /// ���ʰȡ���룺֧�ֵ���ͳ���ʰȡ
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
    /// ��ⱸ�ý������룺֧�ֶ̰��ͳ������ֽ�������������չ�������
    /// </summary>
    void HandleAlternateActionInput()
    {
        if (currentHandObj != null)
        {
            // ��ⱸ�ý����̰������簴��ĳ��ר�ü���
            if (inputController.IsAlternateActionPressed())
            {
                currentHandObj.OnAlternateAction();
            }
            
            // ��ⱸ�ý�������
            if (inputController.IsAlternateActionPressing())
            {
                alternateActionTimer += Time.deltaTime;
                if (alternateActionTimer >= alternateActionDelay)
                {
                    currentHandObj.OnAlternateActionLongPress();
                    alternateActionTimer = 0f; // �������������ü�ʱ��
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
    /// ִ��ʰȡ�����߼�
    /// </summary>
    /// <param name="pickUpObj">��ʰȡ������</param>
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
        
        // �������������ʰȡ�߼�
        heldObj.OnPickup(holdPos);
        heldObj.gameObject.layer = LayerNumber;
        
        // ����������������ײ
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        playerBlackBoard.isHeldObj = true;
        playerBlackBoard.heldObjRigidBody = heldObjRb;
        
        // ���������Ұ
        _camera.DOFieldOfView(CameraFieldOfViewOffset, 0.5f);

        // ʾ�������ʰȡ����Knife���������⴦��
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

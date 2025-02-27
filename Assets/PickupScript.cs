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
    // pickUpRange ���ڽ����ڵ��Ի���Gizmos������Trigger��Χ���������������
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
    
    private BasePickableItem currentHandObj; // ��ǰ�����ֲ���Χ�ڵ�����

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
            // ��������ʱִ����ת�߼�
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                //StopClipping();
                DropObject();   
            }
        }
        else
        {
            // ������ʰȡ��ť�ҵ�ǰ��Ŀ������ʱʰȡ
            if (inputController.IsPickUpPressed() && currentHandObj != null)
            {
                PickUpObject(currentHandObj);
                currentHandObj = null; // ʰȡ����յ�ǰ�ֲ�Ŀ��
            }
        }
    }

    // ʹ��Trigger������ʰȡ��Χ������
    public void OnHandTriggerEnter(Collider other)
    {
        if (other.CompareTag("canPickUp"))
        {
            BasePickableItem item = other.GetComponent<BasePickableItem>();
            if (item != null)
            {
                // �����ǰû��Ŀ�꣬�����ò����� OnHandEnter
                if (currentHandObj == null)
                {
                    currentHandObj = item;
                    currentHandObj.OnHandEnter();
                }
            }
        }
    }

    // �������뿪��������ʱ
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
        // �����Ҫ����ӷ�ֹ��ģ���߼�
    }

    // ��ѡ���ڱ༭���л���ʰȡ��Χ���������
    private void OnDrawGizmos()
    {
        if(handPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(handPos.position, pickUpRange);
        }
    }
}

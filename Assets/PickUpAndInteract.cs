using System;
using System.Collections;
using System.Collections.Generic;
using Aya.Events;
using DG.Tweening;
using UnityEngine;

/// <summary>
/// ��Ʒʰȡ�뽻���߼���
/// ������Ҷ���Ʒ��ʰȡ����ת�������Ȳ�����
/// </summary>
public class PickUpAndInteract : MonoBehaviour
{
    public Transform holdPos; // ��Ʒ����λ��

    public float pickUpRange = 5f; // ʰȡ��Χ
    private float rotationSensitivity = 1f; // ��ת������

    private IInteractable heldObj; // ��ǰ��������ӿ�
    private Rigidbody heldObjRb; // ��ǰ��������ĸ���
    private bool canDrop = true; // �Ƿ���Զ�������

    private PlayerInputController inputController; // ���������
    private HandController _handController; // �ֲ�������
    private Camera _camera; // �������

    [Header("�ֲ����")]
    [SerializeField] private Transform handPos;
    [SerializeField] private Transform CameraPos;
    [SerializeField] private float CameraFieldOfViewOrgin;
    [SerializeField] private float CameraFieldOfViewOffset;
    private PlayerBlackBoard playerBlackBoard;

    public IInteractable currentHandObj; // ��ǰ�ֲ���Χ�ڵ�����
    private float pickUpTimer = 0f; // ����ʰȡ��ʱ��

    private void Awake()
    {
        _camera = Camera.main;
        _handController = GetComponent<HandController>();
        inputController = GetComponent<PlayerInputController>();
        playerBlackBoard = GetComponent<PlayerBlackBoard>();

        if (inputController == null)
        {
            Debug.LogError("PlayerInputController is missing!");
        }
    }

    void Update()
    {
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
            HandlePickUpInput();
        }

        float progress = (currentHandObj != null && currentHandObj._pickDelay != 0) ?
            (float)pickUpTimer / currentHandObj._pickDelay : 0;
        UEvent.Dispatch(EventType.OnPickingItem, progress);
    }

    /// <summary>
    /// ����ʰȡ����
    /// </summary>
    void HandlePickUpInput()
    {
        if (currentHandObj != null)
        {
            if (inputController.IsInteractPressed())
            {
                currentHandObj.Interact(InteractionType.Interact);
            }

            if (inputController.IsPickUpPressing() && currentHandObj._pickDelay != 0)
            {
                pickUpTimer += Time.deltaTime;
                if (pickUpTimer >= currentHandObj._pickDelay)
                {
                    PickUpObject(currentHandObj);
                    currentHandObj = null;
                    pickUpTimer = 0f;
                }
            }
            else if (currentHandObj._pickDelay == 0 && inputController.IsPickUpPressed())
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

    void PickUpObject(IInteractable pickUpObj)
    {
        if (Vector3.Distance(pickUpObj._transform.position, transform.position) > pickUpRange)
        {
            Debug.LogWarning("���峬��ʰȡ��Χ��");
            return;
        }

        heldObj = pickUpObj;
        heldObjRb = heldObj._rb;
        heldObj.Interact(InteractionType.Pick, holdPos, this);

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

        heldObj.Interact(InteractionType.Throw, this);
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

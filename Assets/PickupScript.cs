using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    public float throwForce = 500f;
    public float pickUpRange = 5f;
    private float rotationSensitivity = 1f;

    private BasePickableItem heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private PlayerInputController inputController; // ���� PlayerInputController


    [Header("�ֲ����")]
    //[Tooltip("���뾶�����־���С������ľͻᱻʰȡ")][SerializeField] private float checkRadius; 
    [SerializeField] private Transform handPos; 
    [SerializeField] private Transform CameraPos;
    private PlayerBlackBoard playerBlackBoard;

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
    
    private BasePickableItem currentHandObj; // ��ǰ�����ֲ���Χ�ڵ�����

    void Update()
    {
        if (inputController == null) return;

        if (heldObj != null)
        {
            // MoveObject();
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                StopClipping();
                DropObject();   
            }
        }
        else
        {
            // -----------------------------
            // ���������Χ�����壬������ OnHandEnter/OnHandExit
            // -----------------------------
            BasePickableItem newTarget = null;
            Collider[] hitColliders = Physics.OverlapSphere(handPos.position, pickUpRange);
            foreach (var hit in hitColliders)
            {
                if (hit.transform.gameObject.CompareTag("canPickUp"))
                {
                    newTarget = hit.transform.gameObject.GetComponent<BasePickableItem>();
                    break; // ȡ��һ����⵽������
                }
            }
    
            // �����Ŀ��仯ʱ���뿪ԭ��Ŀ���Ӵ����µ�Ŀ��
            if (newTarget != currentHandObj)
            {
                if (currentHandObj != null)
                {
                    // �뿪ԭ������
                    currentHandObj.OnHandExit();
                }
                if (newTarget != null)
                {
                    // ��������뷶Χ
                    newTarget.OnHandEnter();
                }
                currentHandObj = newTarget;
            }

            // -----------------------------
            // ԭʼʰȡ/Ͷ���߼����ֲ���
            // -----------------------------
            if (inputController.IsPickUpPressed())
            {
                if (heldObj == null)
                {
                    // ������ʰȡʱ��ʰȡ��⵽�����壨��ԭ�߼�һ�£�ѡ���һ���������������壩
                    if (newTarget != null)
                    {
                        PickUpObject(newTarget);
                        // ʰȡ����Խ� currentHandObj ��ջ��� null���Է������������� OnHandExit���Ӿ������������
                        currentHandObj = null;
                        return;
                    }
                }
            }
        }

    }

    void PickUpObject(BasePickableItem pickUpObj)
    {

            heldObj = pickUpObj;
            heldObjRb =heldObj. rb;
            heldObj.OnPickup(holdPos);
            heldObj.gameObject.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            playerBlackBoard.isHeldObj = true;
            playerBlackBoard.heldObjRigidBody = heldObjRb;
        
        
    }

    void DropObject()
    {

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
        

        
    }

    void MoveObject()
    {
        heldObj.transform.position = holdPos.transform.position;
    }
    

    void RotateObject()
    {
        if (inputController.IsRotateHeld())
        {
            canDrop = false;

            float XaxisRotation = inputController.GetMouseInput().x * rotationSensitivity;
            float YaxisRotation = inputController.GetMouseInput().y * rotationSensitivity;
            heldObj.transform.Rotate(-CameraPos.up, XaxisRotation,Space.World);
            heldObj.transform.Rotate(CameraPos.right, YaxisRotation,Space.World);
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
        //var clipRange = Vector3.Distance(heldObj.transform.position, transform.position);
        // RaycastHit[] hits;
        // hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        // if (hits.Length > 1)
        // {
        //     heldObj.transform.position = CameraPos.transform.position + new Vector3(0f, -0.5f, 0f);
        // }
        // Collider[] hitColliders = Physics.OverlapSphere(handPos.position, pickUpRange);
        // if (hitColliders.Length > 1)
        // {
        //     heldObj.transform.position = CameraPos.transform.position + new Vector3(0f, -0.5f, 0f);
        // }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color= Color.red;
        Gizmos.DrawWireSphere(handPos.position, pickUpRange);
    }
}

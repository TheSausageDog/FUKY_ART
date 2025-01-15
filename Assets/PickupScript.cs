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

    private GameObject heldObj;
    private Rigidbody heldObjRb;
    private bool canDrop = true;
    private int LayerNumber;

    private PlayerInputController inputController; // 引用 PlayerInputController


    [Header("手部相关")]
    //[Tooltip("检查半径，与手距离小于这个的就会被拾取")][SerializeField] private float checkRadius; 
    [SerializeField] private Transform handPos; 
    [SerializeField] private Transform CameraPos;
    private PlayerBlackBoard playerBlackBoard;

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

        // 检测捡起/丢弃
        if (inputController.IsPickUpPressed())
        {
            if (heldObj == null)
            {
                // RaycastHit hit;
                // if (Physics.Raycast(CameraPos.transform.position,CameraPos.transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                // {
                //     if (hit.transform.gameObject.tag == "canPickUp")
                //     {
                //         PickUpObject(hit.transform.gameObject);
                //     }
                // }
                
                Collider[] hitColliders = Physics.OverlapSphere(handPos.position, pickUpRange);
                foreach (var hit in hitColliders)
                {
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        PickUpObject(hit.transform.gameObject);
                        return;
                    }
                }
            }
            // else
            // {
            //     if (canDrop)
            //     {
            //         StopClipping();
            //         DropObject();
            //     }
            // }
        }

        if (heldObj != null)
        {
            //MoveObject();
            RotateObject();

            if (inputController.IsThrowPressed() && canDrop)
            {
                StopClipping();
                DropObject();   
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>())
        {
            heldObj = pickUpObj;
            heldObjRb = pickUpObj.GetComponent<Rigidbody>();
            //heldObjRb.isKinematic = true;
            heldObjRb.freezeRotation = true;
            heldObjRb.useGravity = false;
            heldObjRb.transform.parent = holdPos.transform.parent.parent;
            heldObj.layer = LayerNumber;
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
            playerBlackBoard.isHeldObj = true;
            playerBlackBoard.heldObjRigidBody = heldObjRb;
        }
    }

    void DropObject()
    {
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
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
        heldObj.layer = 0;
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

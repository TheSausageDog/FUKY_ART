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
    
    private BasePickableItem currentHandObj; // 当前处于手部范围内的物体

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
            // 持续检测周围的物体，并调用 OnHandEnter/OnHandExit
            // -----------------------------
            BasePickableItem newTarget = null;
            Collider[] hitColliders = Physics.OverlapSphere(handPos.position, pickUpRange);
            foreach (var hit in hitColliders)
            {
                if (hit.transform.gameObject.CompareTag("canPickUp"))
                {
                    newTarget = hit.transform.gameObject.GetComponent<BasePickableItem>();
                    break; // 取第一个检测到的物体
                }
            }
    
            // 当检测目标变化时：离开原有目标或接触到新的目标
            if (newTarget != currentHandObj)
            {
                if (currentHandObj != null)
                {
                    // 离开原有物体
                    currentHandObj.OnHandExit();
                }
                if (newTarget != null)
                {
                    // 新物体进入范围
                    newTarget.OnHandEnter();
                }
                currentHandObj = newTarget;
            }

            // -----------------------------
            // 原始拾取/投掷逻辑保持不变
            // -----------------------------
            if (inputController.IsPickUpPressed())
            {
                if (heldObj == null)
                {
                    // 当按下拾取时，拾取检测到的物体（与原逻辑一致：选择第一个符合条件的物体）
                    if (newTarget != null)
                    {
                        PickUpObject(newTarget);
                        // 拾取后可以将 currentHandObj 清空或置 null，以防后续继续触发 OnHandExit（视具体需求而定）
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

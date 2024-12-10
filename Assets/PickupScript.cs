using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpScript : MonoBehaviour
{
    public GameObject player;
    public Transform holdPos;
    //����������²��ָ��ƣ����ڷ�������Ҫϲ�������Ƶ
    public float throwForce = 500f; //���屻Ͷ��ʱ������
    public float pickUpRange = 5f; //��ҿ��Լ��������������
    private float rotationSensitivity = 1f; //������ת�������ȣ�������ƶ����
    private GameObject heldObj; //��Ҽ��������
    private Rigidbody heldObjRb; //��������ĸ���
    private bool canDrop = true; //���ڷ�ֹ����ת����ʱ����/��������
    private int LayerNumber; //�㼶����

    //���ð����������ƶ����鿴��Χ���Ľű�
    //����ת����ʱ������ϣ��������ҵĲ鿴����
    //������ʾ��
    //MouseLookScript mouseLookScript;

    void Start()
    {
        LayerNumber = LayerMask.NameToLayer("holdLayer"); //������� holdLayer ���Ʋ�ͬ����ȷ�����Ĵ˴��� ""

        //mouseLookScript = player.GetComponent<MouseLookScript>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E)) //���� E Ϊ��ϣ�����µļ����
        {
            if (heldObj == null) //�����ǰû�г����κ�����
            {
                //ִ�����߼�⣬�������Ƿ��ڼ���Χ�ڿ�������
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, pickUpRange))
                {
                    //ȷ�����帽���� "canPickUp" ��ǩ
                    if (hit.transform.gameObject.tag == "canPickUp")
                    {
                        //�������߻��е����崫�ݸ� PickUpObject ����
                        PickUpObject(hit.transform.gameObject);
                    }
                }
            }
            else
            {
                if (canDrop == true)
                {
                    StopClipping(); //��ֹ���崩͸ǽ��
                    DropObject();
                }
            }
        }

        if (heldObj != null) //�����ҳ�������
        {
            MoveObject(); //���������� holdPos λ��
            RotateObject();
            if (Input.GetKeyDown(KeyCode.Mouse0) && canDrop == true) //Mouse0�����������Ͷ�������Ը���Ϊ������ť
            {
                StopClipping();
                ThrowObject();
            }
        }
    }

    void PickUpObject(GameObject pickUpObj)
    {
        if (pickUpObj.GetComponent<Rigidbody>()) //ȷ�������и���
        {
            heldObj = pickUpObj; //�� heldObj ��ֵΪ�����߻��е����壨����Ϊ null��
            heldObjRb = pickUpObj.GetComponent<Rigidbody>(); //��ȡ����ĸ���
            heldObjRb.isKinematic = true;
            heldObjRb.transform.parent = holdPos.transform; //��������Ϊ holdPos ��������
            heldObj.layer = LayerNumber; //������Ĳ㼶����Ϊ holdLayer
            //ȷ�����岻������ҷ�����ײ���Է�ֹ��ֵĴ���
            Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), true);
        }
    }

    void DropObject()
    {
        //������������ҵ���ײ
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0; //������㼶���û�Ĭ�ϲ�
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null; //ȡ������ĸ��ӹ�ϵ
        heldObj = null; //ȡ�������������
    }

    void MoveObject()
    {
        //��������λ���� holdPos һ��
        heldObj.transform.position = holdPos.transform.position;
    }

    void RotateObject()
    {
        if (Input.GetKey(KeyCode.R)) //��ס R ����ת�����Ը���Ϊ������
        {
            canDrop = false; //ȷ������תʱ�޷�����

            //������ҵĲ鿴����
            //mouseLookScript.verticalSensitivity = 0f;
            //mouseLookScript.lateralSensitivity = 0f;

            float XaxisRotation = Input.GetAxis("Mouse X") * rotationSensitivity;
            float YaxisRotation = Input.GetAxis("Mouse Y") * rotationSensitivity;
            //������� X-Y ����ת����
            heldObj.transform.Rotate(Vector3.down, XaxisRotation);
            heldObj.transform.Rotate(Vector3.right, YaxisRotation);
        }
        else
        {
            //����������ҵĲ鿴����
            //mouseLookScript.verticalSensitivity = originalvalue;
            //mouseLookScript.lateralSensitivity = originalvalue;
            canDrop = true;
        }
    }

    void ThrowObject()
    {
        //�� Drop ������ͬ������ȡ������ǰ�����������
        Physics.IgnoreCollision(heldObj.GetComponent<Collider>(), player.GetComponent<Collider>(), false);
        heldObj.layer = 0;
        heldObjRb.isKinematic = false;
        heldObj.transform.parent = null;
        heldObjRb.AddForce(transform.forward * throwForce);
        heldObj = null;
    }

    void StopClipping() //���ڶ���/Ͷ��ʱ���õĺ���
    {
        var clipRange = Vector3.Distance(heldObj.transform.position, transform.position); //holdPos �������֮��ľ���
        //����ʹ�� RaycastAll����Ϊ������赲��Ļ���ĵ�����
        //RaycastAll ������ clipRange �����б����е���ײ������
        RaycastHit[] hits;
        hits = Physics.RaycastAll(transform.position, transform.TransformDirection(Vector3.forward), clipRange);
        //������鳤�ȴ��� 1����ζ���������˳������ǳ��е�����֮�����������
        if (hits.Length > 1)
        {
            //������λ�ø���Ϊ�����λ��
            heldObj.transform.position = transform.position + new Vector3(0f, -0.5f, 0f); //��΢����ƫ�ƣ��Է�ֹ����������Ϸ�����
            //���������ҽ�С�����Խ� -0.5f ����Ϊ��С����ֵ������ֵ�������磺-0.1f
        }
    }
}



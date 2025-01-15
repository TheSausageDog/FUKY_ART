using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;

    // �ƶ�����Ծ����
    public float speed = 5f;
    public float jumpHeight = 2f; // ��Ծ�߶�
    public float gravity = -9.81f; // �������ٶ�

    private Vector3 velocity; // ���ڴ洢��ɫ���ٶ�
    private bool isGrounded; // ���ڼ���ɫ�Ƿ��ڵ�����

    // ���������
    public Transform groundCheck; // ������ĵ�
    public float groundDistance = 0.4f; // ������ķ�Χ
    private LayerMask floorMask; // �������Ĳ㼶

    private PlayerInputController inputController; // �������������

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        inputController = GetComponent<PlayerInputController>(); // ��ȡ���������
        floorMask = LayerMask.GetMask("floor"); // ���á�floor���㼶
    }

    private void Update()
    {
        // ����Ƿ��ڵ�����
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, floorMask);

        // ����ڵ��������ٶ����£����ô�ֱ�ٶ�
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // ȷ����ɫ����
        }

        // ��ȡ����
        var input = inputController.GetMovementInput();

        // �����ƶ�����
        var direction = transform.right * input.x + transform.forward * input.y;
        direction.Normalize();

        // �ƶ���ɫ
        controller.Move(direction * speed * Time.deltaTime);

        // ��Ծ�߼�
        if (isGrounded && inputController.IsJumping())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // ������Ծ���ٶ�
        }

        // Ӧ������
        velocity.y += gravity * Time.deltaTime;

        // ��ֱ�ƶ�
        controller.Move(velocity * Time.deltaTime);
    }
}
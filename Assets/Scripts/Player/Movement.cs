using UnityEngine;

public class Movement : MonoBehaviour
{
    private CharacterController controller;

    // 移动和跳跃参数
    public float speed = 5f;
    public float jumpHeight = 2f; // 跳跃高度
    public float gravity = -9.81f; // 重力加速度

    private Vector3 velocity; // 用于存储角色的速度
    private bool isGrounded; // 用于检查角色是否在地面上

    // 地面检测参数
    public Transform groundCheck; // 检测地面的点
    public float groundDistance = 0.4f; // 检测地面的范围
    private LayerMask floorMask; // 定义地面的层级

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        floorMask = LayerMask.GetMask("floor"); // 设置“floor”层级
    }

    private void Update()
    {
        // 检测是否在地面上
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, floorMask);

        // 如果在地面上且速度向下，重置垂直速度
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // 确保角色贴地
        }

        // 获取输入
        var input = PlayerInputController.GetMovementInput();

        // 计算移动方向
        var direction = transform.right * input.x + transform.forward * input.y;
        direction.Normalize();

        // 移动角色
        controller.Move(direction * speed * Time.deltaTime);

        // 跳跃逻辑
        if (isGrounded && PlayerInputController.IsJumping())
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity); // 计算跳跃初速度
        }

        // 应用重力
        velocity.y += gravity * Time.deltaTime;

        // 垂直移动
        controller.Move(velocity * Time.deltaTime);
    }
}
using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    // 获取移动输入
    public Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontal, vertical);
    }

    // 检查是否按下跳跃键
    public bool IsJumping()
    {
        return Input.GetButtonDown("Jump");
    }
    
    // 检查是否按下下蹲键
    public bool IsCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }
}
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

    // 获取鼠标输入
    public Vector2 GetMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }

    // 获取鼠标滚轮输入
    public float GetScrollInput()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
    // 检查是否按下拾取键
    public bool IsPickUpPressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    // 检查是否按下投掷键
    public bool IsThrowPressed()
    {
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    // 检查是否按住旋转键
    public bool IsRotateHeld()
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }
    // 检查是否移动手
    public bool IsMoveHandHeld()
    {
        return Input.GetKey(KeyCode.LeftAlt);
    }

}
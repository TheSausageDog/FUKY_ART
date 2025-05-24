using UnityEngine;

public class PlayerInputController : MonoBehaviour
{
    protected static bool isUplift = false;
    protected static bool isRotating = false;

    protected static bool isLeftClick;
    protected static bool isRightClick;


    // public 

    void Update()
    {
        if(FUKYMouse.Instance != null)
        {
            isUplift = FUKYMouse.Instance.isMouseFloating;
            isRotating = FUKYMouse.Instance.isMouseFloating;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isUplift = !isUplift;
        }

        isRotating = Input.GetKey(KeyCode.LeftAlt);
    }

    public static bool IsInteractPressed()
    {
        if (FUKYMouse.Instance != null)
        {
            return Input.GetKeyDown(KeyCode.Mouse1) || FUKYMouse.Instance.Right_pressed;
        }
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    public static bool IsRecipePressed()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

    // 获取移动输入
    public static Vector2 GetMovementInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        return new Vector2(horizontal, vertical);
    }


    // 检查是否按下跳跃键
    public static bool IsJumping()
    {
        return Input.GetButtonDown("Jump");
    }

    // 检查是否按下下蹲键
    public static bool IsCrouching()
    {
        return Input.GetKey(KeyCode.LeftControl);
    }

    // 获取鼠标输入
    public static Vector2 GetMouseInput()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        return new Vector2(mouseX, mouseY);
    }

    // 获取鼠标滚轮输入
    public static float GetScrollInput()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
    // 检查是否按下拾取键
    public static bool IsPickUpPressed()
    {
        if (FUKYMouse.Instance != null)
        {
            return Input.GetKeyDown(KeyCode.Mouse0)||FUKYMouse.Instance.Left_pressed;
        }
        return Input.GetKeyDown(KeyCode.Mouse0);
    }
    public static bool IsPickUpPressing()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    // 检查是否按下投掷键
    public static bool IsThrowPressed()
    {
        if (FUKYMouse.Instance != null)
        {
            return Input.GetKeyDown(KeyCode.Mouse0) || FUKYMouse.Instance.Left_pressed;
        }
        return Input.GetKeyDown(KeyCode.Mouse0);
    }

    // 检查是否按住旋转键temp
    public static bool IsRotateHeld()
    {
        return isRotating;
    }
    // 检查是否移动手
    public static bool IsMoveHandHeld()
    {
        return isUplift;
    }

    public static bool IsLeftShiftPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
}
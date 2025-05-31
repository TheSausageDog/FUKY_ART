using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputController : SingletonMono<PlayerInputController>
{
    protected bool isUplift = false;

    public InputActionReference tirgger_Action;

    void Update()
    {
        if (FUKYMouse.Instance != null)
        {
            isUplift = FUKYMouse.Instance.isMouseFloating;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isUplift = !isUplift;
        }
    }

    public bool IsInteractPressed()
    {
        if (FUKYMouse.Instance != null)
        {
            return Input.GetKeyDown(KeyCode.Mouse1) || FUKYMouse.Instance.Right_pressed;
        }
        return Input.GetKeyDown(KeyCode.Mouse1);
    }

    public bool IsRecipePressed()
    {
        return Input.GetKeyDown(KeyCode.R);
    }

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
        bool pressed = Input.GetKeyDown(KeyCode.Mouse0);
        if (tirgger_Action != null)
        {
            pressed |= tirgger_Action.action.WasPerformedThisFrame();
        }
        if (FUKYMouse.Instance != null)
        {
            pressed |= FUKYMouse.Instance.Left_pressed;
        }
        return pressed;
    }
    public bool IsPickUpPressing()
    {
        return Input.GetKey(KeyCode.Mouse0);
    }

    // 检查是否按下投掷键
    public bool IsThrowPressed()
    {
        bool pressed = Input.GetKeyDown(KeyCode.Mouse0);
        if (tirgger_Action != null)
        {
            pressed |= tirgger_Action.action.WasPerformedThisFrame();
        }
        if (FUKYMouse.Instance != null)
        {
            if (FUKYMouse.Instance.isMouseFloating) pressed |= Input.GetKeyDown(KeyCode.G);
        }
        return pressed;
    }

    // 检查是否按住旋转键temp
    public bool IsRotateHeld()
    {
        if (FUKYMouse.Instance != null && FUKYMouse.Instance.isMouseFloating)
        {
            return FUKYMouse.Instance.Left_pressed;
        }
        return Input.GetKey(KeyCode.LeftAlt);
    }
    // 检查是否移动手
    public bool IsMoveHandHeld()
    {
        return isUplift;
    }

    public bool IsLeftShiftPressed()
    {
        return Input.GetKeyDown(KeyCode.LeftShift);
    }
}
using UnityEngine;

public class Look : MonoBehaviour
{

    public float sensitivity = 200f; // 鼠标灵敏度
    private float xRotation = 0f; // 当前X轴的旋转角度

    [SerializeField] private Transform playerCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        if (PlayerInputController.IsMoveHandHeld()) return;

        // 获取鼠标输入
        var input = PlayerInputController.GetMouseInput() * (sensitivity * Time.deltaTime);

        // 计算垂直旋转并限制角度
        xRotation -= input.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // 应用旋转
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        playerCamera.transform.root.Rotate(Vector3.up * input.x);
    }
}
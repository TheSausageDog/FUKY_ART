using UnityEngine;

public class Look : MonoBehaviour
{

    public float sensitivity = 200f; // 鼠标灵敏度

    [SerializeField] private Transform playerCamera;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {

        if (PlayerInputController.Instance.IsMoveHandHeld()) return;

        // 获取鼠标输入
        var input = PlayerInputController.Instance.GetMouseInput() * (sensitivity * Time.deltaTime);
        float currentXRotation = playerCamera.localEulerAngles.x;
        if (currentXRotation > 180) currentXRotation -= 360;

        currentXRotation -= input.y;
        currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);

        // 应用旋转
        playerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        playerCamera.transform.root.Rotate(Vector3.up * input.x);
    }
}
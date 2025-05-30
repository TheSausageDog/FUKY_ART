using UnityEngine;

[DisallowMultipleComponent]
public class ObjectOrientationController : MonoBehaviour
{
    [Header("旋转控制参数")]
    [Tooltip("正常旋转速度")]
    public float normalRotationSpeed = 3f;
    [Tooltip("调整模式旋转速度")]
    public float adjustRotationSpeed = 1.5f;
    [Tooltip("旋转平滑过渡系数")]
    public float rotationSmoothing = 5f;

    [Header("状态指示")]
    [SerializeField] private bool _isAdjusting;  // Serialized for debug

    // 旋转基准值和调整值
    private Quaternion _baseRotation;
    private Quaternion _adjustmentRotation;

    void Start()
    {
        InitializeRotation();
    }

    void Update()
    {
        HandleRotationInput();
        ApplySmoothRotation();
    }

    // 初始化旋转基准
    void InitializeRotation()
    {
        _baseRotation = transform.rotation;
        _adjustmentRotation = Quaternion.identity;
    }

    // 处理输入逻辑
    void HandleRotationInput()
    {
        // 鼠标左键按下时开始调整
        if (Input.GetMouseButtonDown(0))
        {
            StartAdjustment();
        }

        // 鼠标左键松开时结束调整
        if (Input.GetMouseButtonUp(0))
        {
            EndAdjustment();
        }

        // 持续处理旋转输入
        ProcessRotation();
    }

    // 开始调整模式
    void StartAdjustment()
    {
        _isAdjusting = true;
        _adjustmentRotation = transform.rotation;
    }

    // 结束调整模式
    void EndAdjustment()
    {
        _isAdjusting = false;
        _baseRotation = transform.rotation;
    }

    // 处理旋转计算
    void ProcessRotation()
    {
        // 获取鼠标输入
        Vector2 mouseDelta = new Vector2(
            Input.GetAxis("Mouse X"),
            Input.GetAxis("Mouse Y")
        );

        // 根据模式选择旋转速度
        float speed = _isAdjusting ? adjustRotationSpeed : normalRotationSpeed;

        // 计算旋转量（Y轴水平，X轴垂直）
        Quaternion yaw = Quaternion.AngleAxis(mouseDelta.x * speed, Vector3.up);
        Quaternion pitch = Quaternion.AngleAxis(-mouseDelta.y * speed, Vector3.right);

        // 应用旋转
        if (_isAdjusting)
        {
            _adjustmentRotation = yaw * pitch * _adjustmentRotation;
        }
        else
        {
            _baseRotation = yaw * pitch * _baseRotation;
        }
    }

    // 应用平滑旋转
    void ApplySmoothRotation()
    {
        // 根据模式选择目标旋转
        Quaternion targetRotation = _isAdjusting ?
            _adjustmentRotation :
            _baseRotation;

        // 使用球面插值平滑过渡
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * rotationSmoothing
        );
    }

    // 调试用重置功能
    public void ResetOrientation()
    {
        InitializeRotation();
        transform.rotation = _baseRotation;
    }
}
using UnityEngine;

/// <summary>
/// 摄像机旋转控制器
/// 使摄像机在场景运行时沿Y轴进行慢速循环旋转
/// </summary>
[DisallowMultipleComponent]
public class CameraRotator : MonoBehaviour
{
    [Header("旋转设置")]
    [Tooltip("旋转速度 (度/秒)")]
    [Range(0.1f, 20f)]
    public float rotationSpeed = 5f; // 默认旋转速度为5度/秒

    [Tooltip("是否逆时针旋转 (默认为顺时针)")]
    public bool counterClockwise = false;

    [Tooltip("是否启用旋转")]
    public bool enableRotation = true;

    [Header("高级设置")]
    [Tooltip("是否限制旋转角度")]
    public bool limitRotation = false;

    [Tooltip("最小旋转角度")]
    [Range(-180f, 180f)]
    public float minAngle = -45f;

    [Tooltip("最大旋转角度")]
    [Range(-180f, 180f)]
    public float maxAngle = 45f;

    // 初始旋转角度
    private Quaternion initialRotation;
    // 当前Y轴旋转角度
    private float currentYRotation = 0f;
    // 旋转方向乘数
    private int directionMultiplier = 1;

    /// <summary>
    /// 初始化组件
    /// </summary>
    private void Start()
    {
        // 保存初始旋转状态
        initialRotation = transform.rotation;
        
        // 获取当前Y轴旋转角度
        currentYRotation = transform.eulerAngles.y;
        
        // 设置旋转方向
        directionMultiplier = counterClockwise ? -1 : 1;
    }

    /// <summary>
    /// 每帧更新旋转
    /// </summary>
    private void Update()
    {
        // 如果未启用旋转，则跳过
        if (!enableRotation)
            return;

        // 计算这一帧的旋转增量
        float rotationAmount = rotationSpeed * Time.deltaTime * directionMultiplier;
        
        // 更新当前Y轴旋转角度
        currentYRotation += rotationAmount;

        // 如果启用了旋转限制，则确保角度在限制范围内
        if (limitRotation)
        {
            // 将角度标准化到-180到180度范围内，便于比较
            float normalizedAngle = NormalizeAngle(currentYRotation);
            
            // 如果超出范围，则反转方向
            if (normalizedAngle < minAngle || normalizedAngle > maxAngle)
            {
                directionMultiplier *= -1; // 反转方向
                currentYRotation += rotationAmount * 2 * directionMultiplier; // 调整角度，避免卡在边界
            }
        }

        // 应用旋转
        ApplyRotation();
    }

    /// <summary>
    /// 应用计算后的旋转到摄像机
    /// </summary>
    private void ApplyRotation()
    {
        // 创建一个仅包含Y轴旋转的四元数
        Quaternion yRotation = Quaternion.Euler(0, currentYRotation, 0);
        
        // 将Y轴旋转与初始旋转结合
        // 这样可以保持摄像机的初始俯仰角和翻滚角
        transform.rotation = initialRotation * Quaternion.Inverse(Quaternion.Euler(0, initialRotation.eulerAngles.y, 0)) * yRotation;
    }

    /// <summary>
    /// 将角度标准化到-180到180度范围
    /// </summary>
    /// <param name="angle">输入角度</param>
    /// <returns>标准化后的角度</returns>
    private float NormalizeAngle(float angle)
    {
        // 首先将角度限制在0-360范围内
        angle = angle % 360;
        
        // 然后转换到-180到180范围
        if (angle > 180)
            angle -= 360;
            
        return angle;
    }

    /// <summary>
    /// 重置摄像机旋转到初始状态
    /// </summary>
    public void ResetRotation()
    {
        transform.rotation = initialRotation;
        currentYRotation = initialRotation.eulerAngles.y;
    }

    /// <summary>
    /// 切换旋转方向
    /// </summary>
    public void ToggleDirection()
    {
        counterClockwise = !counterClockwise;
        directionMultiplier = counterClockwise ? -1 : 1;
    }

    /// <summary>
    /// 设置旋转速度
    /// </summary>
    /// <param name="speed">新的旋转速度</param>
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = Mathf.Clamp(speed, 0.1f, 20f);
    }
}

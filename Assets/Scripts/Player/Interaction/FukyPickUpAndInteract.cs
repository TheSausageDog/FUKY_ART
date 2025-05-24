using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Windows;

public class FukyPickUpAndInteract : PickUpAndInteract
{
    //摄像机控制参数
    [Header("屏幕边缘控制")]
    [Tooltip("距离屏幕边缘开始旋转的像素阈值")]
    public float borderThreshold = 50f;
    [Tooltip("旋转速度系数")]
    public float rotationSpeed = 1f;
    private float xRotation; // 垂直旋转累积量

    public Transform Player;
    public Camera PlayerCamera;
    //public bool DeltaMethord = false;
    [Tooltip("Z轴偏移")]
    [Range(-10F, 10F)]
    public float Z_Offset;
    protected override void MoveHandTarget()
    {

        Vector3 NewPos = data.handTarget.localPosition + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue; // 使用了delta的方式，更方便控制位移量
        NewPos.x = Mathf.Clamp(NewPos.x, data.xMinMax.x, data.xMinMax.y);// 限制 handTarget 的本地位置
        NewPos.y = Mathf.Clamp(NewPos.y, data.yMinMax.x, data.yMinMax.y);
        NewPos.z = Mathf.Clamp(NewPos.z, data.zMinMax.x, data.zMinMax.y);
        data.handTarget.localPosition = NewPos;

        // 将世界空间的旋转转换到相机空间
        Quaternion PlayerCameraeraSpaceRotation = Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;
        data.holdPos.rotation = PlayerCameraeraSpaceRotation;

        HandleScreenEdgeRotation();
    }
    private void HandleScreenEdgeRotation()
    {
        if (PlayerCamera == null) return;

        // 将handTarget的世界坐标转换为屏幕坐标
        Vector3 screenPos = PlayerCamera.WorldToScreenPoint(data.handTarget.position);

        // 标准化屏幕坐标 (0-1)
        Vector2 viewportPos = new Vector2(
            screenPos.x / PlayerCamera.pixelWidth,
            screenPos.y / PlayerCamera.pixelHeight
        );

        // 计算四个方向的边界偏移量
        float leftOffset = Mathf.Clamp01(viewportPos.x * PlayerCamera.pixelWidth / borderThreshold);
        float rightOffset = Mathf.Clamp01((PlayerCamera.pixelWidth - viewportPos.x) * PlayerCamera.pixelWidth / borderThreshold);
        float topOffset = Mathf.Clamp01((PlayerCamera.pixelHeight - viewportPos.y) * PlayerCamera.pixelHeight / borderThreshold);
        float bottomOffset = Mathf.Clamp01(viewportPos.y * PlayerCamera.pixelHeight / borderThreshold);

        // 计算旋转输入量
        Vector2 rotationInput = Vector2.zero;

        // 水平旋转（Y轴）
        if (viewportPos.x < borderThreshold / PlayerCamera.pixelWidth)
            rotationInput.x = -Mathf.Lerp(1f, 0f, leftOffset);
        else if (viewportPos.x > 1 - (borderThreshold / PlayerCamera.pixelWidth))
            rotationInput.x = Mathf.Lerp(1f, 0f, rightOffset);

        // 垂直旋转（X轴）
        if (viewportPos.y < borderThreshold / PlayerCamera.pixelHeight)
            rotationInput.y = -Mathf.Lerp(1f, 0f, bottomOffset);
        else if (viewportPos.y > 1 - (borderThreshold / PlayerCamera.pixelHeight))
            rotationInput.y = Mathf.Lerp(1f, 0f, topOffset);

        // 应用旋转
        ApplyPlayerCameraeraRotation(rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void ApplyPlayerCameraeraRotation(Vector2 input)
    {
        // 垂直旋转（X轴）水平旋转（Y轴）
        xRotation -= input.y;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        PlayerCamera.transform.root.Rotate(Vector3.up * input.x);
    }
}



using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Windows;

public class FukyPickUpAndInteract : PickUpAndInteract
{
    public GameObject Fuky_Ball; 
    //摄像机控制参数
    [Header("屏幕边缘控制")]
    [Tooltip("距离屏幕边缘开始旋转的像素阈值")]
    [Range(0.05f,0.2f)]
    public float borderThreshold = 0.1f;
    [Tooltip("旋转速度系数")]
    public float rotationSpeed = 1f;
    private float xRotation; // 垂直旋转累积量

    public Transform Player;
    public Camera PlayerCamera;

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

        // 将handTarget的世界坐标转换为屏幕坐标
        Vector3 screenPos = PlayerCamera.WorldToScreenPoint(data.handTarget.position);

        // 标准化屏幕坐标 (0-1)
        Vector2 viewportPos = new Vector2(
            Mathf.Clamp01(screenPos.x / PlayerCamera.pixelWidth),
            Mathf.Clamp01(screenPos.y / PlayerCamera.pixelHeight)
        );

        //// 计算旋转输入量
        Vector2 rotationInput = Vector2.zero;

        //// 水平旋转（Y轴）
        if (viewportPos.x < borderThreshold)
        {
            float leftOffset = Mathf.Clamp01(borderThreshold - viewportPos.x);
            rotationInput.x = -leftOffset;
        }
        else if (viewportPos.x > 1 - borderThreshold)
        {
            float rightOffset = Mathf.Clamp01(viewportPos.x - 0.9f);
            rotationInput.x = rightOffset;
        }

        //// 垂直旋转（X轴）
        if (viewportPos.y < borderThreshold)
        {
            float bottomOffset = Mathf.Clamp01(borderThreshold - viewportPos.y);
            rotationInput.y = -bottomOffset;
        }
        else if (viewportPos.y > 1 - borderThreshold)
        {
            float topOffset = Mathf.Clamp01( viewportPos.y - 0.9f);
            rotationInput.y = topOffset;
        }


        //// 应用旋转
        ApplyPlayerCameraeraRotation(rotationInput * rotationSpeed * Time.deltaTime );
    }

    private void ApplyPlayerCameraeraRotation(Vector2 input)
    {
        // 垂直旋转（X轴）水平旋转（Y轴）
        float currentXRotation = PlayerCamera.transform.localEulerAngles.x;
        if (currentXRotation > 180) currentXRotation -= 360;


        currentXRotation -= input.y;
        currentXRotation = Mathf.Clamp(xRotation, -90f, 90f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        PlayerCamera.transform.root.Rotate(Vector3.up * input.x);
    }
}



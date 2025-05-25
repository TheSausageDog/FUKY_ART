using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Windows;

public class FukyPickUpAndInteract : PickUpAndInteract
{
    //摄像机控制参数
    [Header("屏幕边缘控制")]
    [Tooltip("距离屏幕边缘开始旋转的像素阈值")]
    [Range(0.05f,0.5f)]
    public float borderThreshold = 0.1f;
    [Tooltip("旋转速度系数")]
    public float rotationSpeed = 1f;
    public Transform Player;
    public Camera PlayerCamera;

    //摄像机控制参数
    [Header("旋转控制")]
    public GameObject Fuky_Ball;
    public GameObject Fuky_Range;
    public bool InRotate = false;
    public float Range = 0.5f;
    [Range(0.5f,0.9f)]
    public float StartRotateThershold = 0.7f;
    private quaternion OffsetQuat = quaternion.identity;

    [Header("FUKYBALL颜色控制")]
    private MaterialPropertyBlock materialPropertyBlock; // 需要动态修改的材质
    public Renderer TargetRenderer; // 关联的渲染器组件
    public Color minColor = Color.green;
    public Color maxColor = Color.red;
    private int baseColorShaderID; // Shader中_BaseColor属性的ID

    private void Start()
    {
        baseColorShaderID = Shader.PropertyToID("_EmissionColor");        // 获取Shader中_BaseColor属性的ID
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    protected override void MoveHandTarget()
    {
        if (PlayerInputController.IsRotateHeld())
        {
            if (!InRotate)
            {
                Fuky_Ball.transform.position = data.handTarget.position;
                Fuky_Range.transform.position = data.handTarget.position;
                Fuky_Ball.SetActive(true);
                Fuky_Range.SetActive(true);
                InRotate =true;
                return;
            }
            Vector3 NewPos = Fuky_Ball.transform.position + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue; // 使用了delta的方式，更方便控制位移量
                                                                                                                                  // 将 NewPos 限制在球体内
            Vector3 offset = NewPos - data.handTarget.position;
            if (offset.magnitude > Range)
            {
                NewPos = data.handTarget.position + offset.normalized * Range; // 超出半径时，强制到球体表面
            }
            Fuky_Ball.transform.position = NewPos;
            UpdateMaterialColor();
            if (FUKYMouse.Instance.PressureValue > StartRotateThershold)
            {
                Vector3 Fuky_ball_Vector = Fuky_Ball.transform.position - data.handTarget.position;
                //quaternion Curr_Quat = 
            }
            return;
        }
        if (FUKYMouse.Instance.Right_pressed)
        {
            if (InRotate) 
            {
                InRotate = false;
                Fuky_Ball.SetActive(false);
                Fuky_Range.SetActive(false);
            }
            Vector3 NewPos = data.handTarget.localPosition + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue; // 使用了delta的方式，更方便控制位移量
            NewPos.x = Mathf.Clamp(NewPos.x, data.xMinMax.x, data.xMinMax.y);// 限制 handTarget 的本地位置
            NewPos.y = Mathf.Clamp(NewPos.y, data.yMinMax.x, data.yMinMax.y);
            NewPos.z = Mathf.Clamp(NewPos.z, data.zMinMax.x, data.zMinMax.y);
            data.handTarget.localPosition = NewPos;

            // 将世界空间的旋转转换到相机空间
            Quaternion PlayerCameraeraSpaceRotation = Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * OffsetQuat * FUKYMouse.Instance.rawRotation;
            data.holdPos.rotation = PlayerCameraeraSpaceRotation;

            HandleScreenEdgeRotation();
        }
    }
    private void HandleScreenEdgeRotation()
    {

        // 将handTarget的世界坐标转换为屏幕坐标
        Vector3 screenPos = PlayerCamera.WorldToScreenPoint(data.handTarget.position);

        // 标准化屏幕坐标 (0-1)
        Vector2 viewportPos = new Vector2
        (
            Mathf.Clamp01(screenPos.x / PlayerCamera.pixelWidth),
            Mathf.Clamp01(screenPos.y / PlayerCamera.pixelHeight)
        );

        //// 计算旋转输入量
        float rotationInput = 0;

        //// 水平旋转（Y轴）
        if (viewportPos.x < borderThreshold)
        {
            float leftOffset = Mathf.Clamp01(borderThreshold - viewportPos.x);
            rotationInput = -leftOffset;
        }
        else if (viewportPos.x > 1 - borderThreshold)
        {
            float rightOffset = Mathf.Clamp01(viewportPos.x - 0.9f);
            rotationInput = rightOffset;
        }

        //// 垂直旋转（X轴）
        //if (viewportPos.y < borderThreshold)
        //{
        //    float bottomOffset = Mathf.Clamp01(borderThreshold - viewportPos.y);
        //    rotationInput.y = -bottomOffset;
        //}
        //else if (viewportPos.y > 1 - borderThreshold)
        //{
        //    float topOffset = Mathf.Clamp01( viewportPos.y - 0.9f);
        //    rotationInput.y = topOffset;
        //}
        //// 应用旋转
        //ApplyPlayerCameraeraRotation(rotationInput * rotationSpeed * Time.deltaTime );
        PlayerCamera.transform.root.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void ApplyPlayerCameraeraRotation(float input)
    {
        // 垂直旋转（X轴）水平旋转（Y轴）
        //float currentXRotation = PlayerCamera.transform.localEulerAngles.x;
        //if (currentXRotation > 180) currentXRotation -= 360;
        //currentXRotation -= input.y;
        //currentXRotation = Mathf.Clamp(currentXRotation, -90f, 90f);
        //PlayerCamera.transform.localRotation = Quaternion.Euler(currentXRotation, 0f, 0f);
        PlayerCamera.transform.root.Rotate(Vector3.up * input);
    }

    private void UpdateMaterialColor()
    {
        if (TargetRenderer == null) return;

        // 计算颜色强度（假设PressureValue范围0~1）
        float pressure = FUKYMouse.Instance.PressureValue;
        Color targetColor = Color.Lerp(minColor, maxColor, pressure);
        // 应用颜色到材质

        TargetRenderer.GetPropertyBlock(materialPropertyBlock);
        materialPropertyBlock.SetColor(baseColorShaderID, targetColor);
        TargetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

}



using DG.Tweening;
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
    public bool InRotateActive = false;
    public float Range = 0.5f;

    [Header("二次旋转控制")]
    [Tooltip("如果压力超过该量就进入旋转控制模式")]
    [Range(0.5f, 0.9f)]
    public float StartRotateThershold = 0.7f;
    public Renderer TargetRenderer; // 关联的渲染器组件
    public Color minColor = Color.green;
    public Color maxColor = Color.red;
    private quaternion Adj_Rotation = quaternion.identity;
    private MaterialPropertyBlock materialPropertyBlock; // 需要动态修改的材质
    private int EmissionShaderID; // Shader中_EmissionColor属性的ID
    private int BaseColorShaderID; // Shader中_BaseColor属性的ID
    private quaternion Init_Rota_Range_LookAt; // 记录初始LookAt旋转
    private quaternion Init_Delta_Rotation; // 记录初始旋转差

    [Header("第一人称握持位置控制")]
    [Tooltip("如果压力超过该量就进入实际控制模式")]
    public Transform DefaultHoldPos;
    [Range(0.001f,0.1f)]
    public float BackHomeSpeed = 0.001f;
    private float CurrBackHomeValue = 0f;

    private void Start()
    {
        EmissionShaderID = Shader.PropertyToID("_EmissionColor");        // 获取Shader中_EmissionColor属性的ID
        BaseColorShaderID = Shader.PropertyToID("_BaseColor");        // 获取Shader中_BaseColor属性的ID
        materialPropertyBlock = new MaterialPropertyBlock();
    }

    protected override void MoveHandTarget()
    {
        if (PlayerInputController.IsRotateHeld())
        {
            //更新UI的颜色
            UpdateMaterialColor();
            Fuky_Range.transform.position = data.handTarget.position;            //更新UI的位置
            //移动小球
            Vector3 NewPos = Fuky_Ball.transform.localPosition + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue;
            Vector3 offset = NewPos - data.handTarget.localPosition;
            if (offset.magnitude > Range){NewPos = data.handTarget.localPosition + offset.normalized * Range; }// 超出半径时，强制到球体表面
            Fuky_Ball.transform.localPosition = NewPos;
            //在第一次进入旋转状态的时候，把两个当UI的3D物体移动到操控对象上
            if (!InRotate)
            {
                Fuky_Ball.transform.position = data.handTarget.position;
                Fuky_Ball.SetActive(true);
                Fuky_Range.SetActive(true);
                InRotate =true;
                return;
            }
            //如果在按左键过程中按下了右键，就计算当前手持物品到小球的向量，并保存向量到手持物体forward的相对旋转
            if (FUKYMouse.Instance.Right_pressed)
            {
                Fuky_Range.transform.LookAt(Fuky_Ball.transform);
                if (!InRotateActive)//如果初次进入循环
                {

                    // D                                                  A                           B                              A*D=B
                    //1相对量                                        LookAt旋转               初始物体旋转        <===得到  LookAt旋转 乘相对量
                    //2相对量                                        移动前的range旋转        移动后的range旋转   <===得到  移动前的range旋转 乘相对量
                    Init_Delta_Rotation = Quaternion.Inverse(Fuky_Range.transform.rotation) * data.holdPos.rotation;
                    Init_Rota_Range_LookAt = Fuky_Range.transform.rotation;
                    //data.holdPos.rotation = Fuky_Range.transform.rotation * Init_Delta_Rotation;
                    InRotateActive = true;
                    return;
                }
                //quaternion CurrPointerRotation_Delta = Quaternion.Inverse(Init_Rota_Range_LookAt) * Fuky_Range.transform.rotation;
                quaternion CurrHoldRotation_Delta = Init_Delta_Rotation * (Quaternion.Inverse(Init_Rota_Range_LookAt) * Fuky_Range.transform.rotation);

                Adj_Rotation = Fuky_Range.transform.rotation * CurrHoldRotation_Delta * Quaternion.Inverse(Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation);
                data.holdPos.rotation = Adj_Rotation * Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;
                return;
            }
            InRotateActive =false;
            return;
        }
        if (InRotate)
        {
            InRotate = false;
            Fuky_Ball.SetActive(false);
            Fuky_Range.SetActive(false);
        }
        if (FUKYMouse.Instance.Right_pressed)
        {
            Vector3 NewPos = data.handTarget.localPosition + FUKYMouse.Instance.deltaTranslate * FUKYMouse.Instance.PressureValue; // 使用了delta的方式，更方便控制位移量
            NewPos.x = Mathf.Clamp(NewPos.x, data.xMinMax.x, data.xMinMax.y);// 限制 handTarget 的本地位置
            NewPos.y = Mathf.Clamp(NewPos.y, data.yMinMax.x, data.yMinMax.y);
            NewPos.z = Mathf.Clamp(NewPos.z, data.zMinMax.x, data.zMinMax.y);
            data.handTarget.localPosition = NewPos;
            HandleScreenEdgeRotation();
        }
        // 将世界空间的旋转转换到相机空间
        Quaternion PlayerCameraeraSpaceRotation = Adj_Rotation * Quaternion.AngleAxis(Player.eulerAngles.y, transform.up) * FUKYMouse.Instance.rawRotation;
        data.holdPos.rotation = PlayerCameraeraSpaceRotation;

    }

    protected override void DefaultControl()
    {
        Vector3 CurrDistance = DefaultHoldPos.position - data.handTarget.position;
        if (CurrDistance.magnitude > 0.001f)
        {
            CurrBackHomeValue = math.min(1f, CurrBackHomeValue + BackHomeSpeed * Time.deltaTime);
            data.handTarget.position = Vector3.Lerp(data.handTarget.position, DefaultHoldPos.position, CurrBackHomeValue);
        }
        CurrBackHomeValue = 0f;

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

        PlayerCamera.transform.root.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void ApplyPlayerCameraeraRotation(float input)
    {

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
        materialPropertyBlock.SetColor(BaseColorShaderID, targetColor);
        materialPropertyBlock.SetColor(EmissionShaderID, targetColor);
        TargetRenderer.SetPropertyBlock(materialPropertyBlock);
    }

}



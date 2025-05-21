using UnityEngine;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Runtime.InteropServices;
//using System.Numerics;
using Unity.Mathematics;


public class FUKYMouse : SingletonMono<FUKYMouse>
{

    [Tooltip("缩放值会缩放相应坐标")]
    [Range(0.001F, 1F)]
    public float Scaler;
    [Tooltip("偏移量")]
    [Range(0.001f, 10f)]
    public float X_Scale;

    [Tooltip("X轴单独的缩放")]
    public Vector3 Rotation_Offset;
    [Tooltip("Y轴单独的缩放")]
    [Range(0.001f, 10f)]
    public float Y_Scale;
    [Tooltip("Z轴单独的缩放")]
    [Range(0.001f, 10f)]
    public float Z_Scale;
    #region 滤波器
    private OneEuroFilter<Vector3> PosFilter;
    [Header("OneEuro滤波器设置")]
    [Tooltip("较高minCutoff值会导致更多的高频噪声通过，而较低的值则会使输出更加平滑")]
    public float minCutoff = 0.2f;
    [Tooltip("增加beta会使滤波器在快速移动时更加响应，但也可能引入更多的高频噪声。\r\n减小beta则会使滤波器更加平滑，但可能导致在快速移动时响应滞后。")]
    public float beta = 0.2f;
    [Tooltip("较高的dCutoff值会使滤波器对速度变化更加敏感，而较低的值则会使输出在速度变化时更加平滑")]
    public float dCutoff = 0.6f;
    [Tooltip("位置数据的更新频率")]
    public float PosFreq = 0.02f;//数据的更新频率
    [Tooltip("旋转数据的更新频率")]
    public float RotateFreq = 0.02f;//数据的更新频率
    public float LastPosUpdateTime = 0.00f;//上一次更新时间
    public float LastRotateUpdateTime = 0.02f;//上一次更新时间
    #endregion
    // 共享内存配置（需与Python端完全一致）
    private const string MOUSE_MEM_NAME = "FUKY_IMU_Memory";
    private const int MOUSE_MEM_SIZE = 32; // Python端定义的32字节
    private const string LOCATOR_MEM_NAME = "FUKY_Locator_Memory";
    private const int LOCATOR_MEM_SIZE = 12; // Python端定义的32字节
    private const string BTN_MEM_NAME = "FUKY_BTN_Memory";
    private const int BTN_MEM_SIZE = 1; // Python端定义的1字节
    private const string PRESS_MEM_NAME = "FUKY_PRESS_Memory";
    private const int PRESS_MEM_SIZE = 2; // Python端定义的2字节


    // 内存映射对象
    private MemoryMappedFile _IMU_MemFile;
    private MemoryMappedViewAccessor _IMU_Accessor;

    private MemoryMappedFile _BTN_MemFile;
    private MemoryMappedViewAccessor _BTN_Accessor;

    private MemoryMappedFile _PRESS_MemFile;
    private MemoryMappedViewAccessor _PRESS_Accessor;

    private MemoryMappedFile _locatorMemFile;
    private MemoryMappedViewAccessor _locatorAccessor;
    // IMU数据结构（与Python打包结构一致）
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct IMUData
    {
        public float accelX;
        public float accelY;
        public float accelZ;
        public float quatX;
        public float quatY;
        public float quatZ;
        public float quatW;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct LocatorData
    {
        public float CoordX;
        public float CoordY;
        public float CoordZ;
    }




    public Vector3 rawAcceleration { get; private set; }
    public Quaternion rawRotation { get; private set; }
    public Vector3 rawTranslate { get; private set; }

    public bool Left_pressed = false;
    public bool Right_pressed = false;
    public bool Middle_pressed = false;
    public bool isMouseFloating = false;
    public ushort PressureValue { get; private set; } // 压力值（0-65535）

    public Vector3 filteredTranslate { get; private set; }

    private Vector3 lastRawTranslate;//上一帧的灯珠位置值
    private Vector3 lastFilteredTranslate;//上一帧的位置值
    private Quaternion lastRawRotation;//上一帧鼠标的旋转值

    public Vector3 deltaTranslate { get; private set; }
    public Quaternion deltaRotation { get; private set; }
    public Vector3 deltaEuler { get; private set; }

    void Start()
    {
        try
        {
            // IMU 打开已存在的共享内存-IMU的数据
            _IMU_MemFile = MemoryMappedFile.OpenExisting(
                MOUSE_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // IMU 创建访问器
            _IMU_Accessor = _IMU_MemFile.CreateViewAccessor(
                0,  // 偏移量
                MOUSE_MEM_SIZE,
                MemoryMappedFileAccess.Read
            );

            // 定位器 打开已存在的共享内存-定位器的数据
            _locatorMemFile = MemoryMappedFile.OpenExisting(
                LOCATOR_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // 定位器 创建访问器
            _locatorAccessor = _locatorMemFile.CreateViewAccessor(
                0,  // 偏移量
                LOCATOR_MEM_SIZE,
                MemoryMappedFileAccess.Read
            );

            // 打开已存在的共享内存-鼠标的数据
            _BTN_MemFile = MemoryMappedFile.OpenExisting(
                BTN_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // 创建访问器
            _BTN_Accessor = _BTN_MemFile.CreateViewAccessor(
                0,  // 偏移量
                BTN_MEM_SIZE,
                MemoryMappedFileAccess.Read
            );

            // 打开已存在的共享内存-鼠标的数据
            _PRESS_MemFile = MemoryMappedFile.OpenExisting(
                PRESS_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // 创建访问器
            _PRESS_Accessor = _PRESS_MemFile.CreateViewAccessor(
                0,  // 偏移量
                PRESS_MEM_SIZE,
                MemoryMappedFileAccess.Read
            );


            Debug.Log("成功连接共享内存");
        }
        catch (Exception e)
        {
            Debug.LogError($"共享内存初始化失败: {e.Message}");
        }

        PosFilter = new OneEuroFilter<Vector3>(50);
    }

    void Update()
    {

        if (_IMU_Accessor == null) return;

        try
        {
            // 读取数据结构
            IMUData data;
            LocatorData data2;
            _IMU_Accessor.Read(0, out data);

            // 转换数据格式
            rawAcceleration = new Vector3(
                data.accelX,
                data.accelY,
                data.accelZ
            );

            // 注意：四元数坐标系的转换（可能需要调整符号）
            rawRotation = new Quaternion(
                -data.quatX,
                -data.quatZ,
                -data.quatY,
                data.quatW
            )* quaternion.Euler(Rotation_Offset);
            //Debug.Log("加速度数据:" + rawAcceleration + "四元数数据:" + rawRotation);

            _locatorAccessor.Read(0, out data2);

            rawTranslate = new Vector3(
                data2.CoordX * X_Scale,
                data2.CoordY * Y_Scale,
                data2.CoordZ * Z_Scale
            ) * Scaler;
            //Debug.Log("定位器坐标数据:" + rawTranslate);

        }
        catch (Exception e)
        {
            Debug.LogError($"读取失败: {e.Message}");
        }

        if (rawRotation != lastRawRotation)
        {
            RotateFreq = 1 / LastRotateUpdateTime;
            LastRotateUpdateTime = 0f;
        }
        if (rawTranslate != lastRawTranslate)
        {
            PosFreq = 1 / LastPosUpdateTime;
            LastPosUpdateTime = 0f;
        }

        try
        {
            // ====== 按钮状态读取 ======
            byte buttonState = _BTN_Accessor.ReadByte(0);

            // 解析按钮位状态（使用位掩码）
            Left_pressed = (buttonState & 0x01) != 0;    // 第0位：左键
            Right_pressed = (buttonState & 0x02) != 0;   // 第1位：右键
            Middle_pressed = (buttonState & 0x04) != 0;  // 第2位：中键
            // 解析第四位（bit3）的浮动状态
            isMouseFloating = (buttonState & 0x08) != 0; // 第3位：浮动状态
            Debug.Log($"按钮值: {buttonState}");

        }
        catch (Exception e)
        {
            Debug.LogError($"读取按钮状态失败: {e.Message}");
        }

        //=== 压力值解析 ===//
        try
        {
            // 读取两个字节（小端格式）
            byte lowByte = _PRESS_Accessor.ReadByte(0);
            byte highByte = _PRESS_Accessor.ReadByte(1);
            PressureValue = (ushort)((highByte << 8) | lowByte);
             Debug.Log($"压力值: {PressureValue}");
        }
        catch (Exception e)
        {
            Debug.LogError($"读取压力值失败: {e.Message}");
        }

        lastRawTranslate = rawTranslate;

        deltaRotation = rawRotation * Quaternion.Inverse(lastRawRotation);
        deltaEuler = rawRotation.eulerAngles - lastRawRotation.eulerAngles;
        lastRawRotation = rawRotation;

        PosFilter.UpdateParams(PosFreq, minCutoff, beta, dCutoff);

        // OE_Rotation = RotationFilter.Filter(Raw_Rotation);
        filteredTranslate = PosFilter.Filter(rawTranslate);

        deltaTranslate = -(filteredTranslate - lastFilteredTranslate);
        lastFilteredTranslate = filteredTranslate;


        LastPosUpdateTime += Time.deltaTime;
        LastRotateUpdateTime += Time.deltaTime;


    }

}
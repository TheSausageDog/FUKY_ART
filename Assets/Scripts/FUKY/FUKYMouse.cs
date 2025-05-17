using UnityEngine;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Runtime.InteropServices;


public class FUKYMouse : SingletonMono<FUKYMouse>
{
    [Tooltip("缩放值会缩放相应坐标")]
    [Range(0.001F, 1F)]
    public float Scaler;
    // [Tooltip("Z轴单独的偏移")]
    // [Range(-500F, 500F)]
    // public float Z_Offset;

    [Tooltip("X轴单独的缩放")]
    [Range(0.001f, 10f)]
    public float X_Scale;
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
    private const string MOUSE_MEM_NAME = "FUKY_Mouse_Memory";
    private const int MOUSE_MEM_SIZE = 32; // Python端定义的32字节
    private const string LOCATOR_MEM_NAME = "FUKY_Locator_Memory";
    private const int LOCATOR_MEM_SIZE = 12; // Python端定义的32字节
    // 内存映射对象
    private MemoryMappedFile _mouseMemFile;
    private MemoryMappedViewAccessor _mouseAccessor;

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


    public Vector3 filteredTranslate { get; private set; }

    private Vector3 lastRawTranslate;//上一帧的灯珠位置值
    private Vector3 lastFilteredTranslate;//上一帧的灯珠位置值
    private Quaternion lastRawRotation;//上一帧鼠标的旋转值

    public Vector3 deltaTranslate { get; private set; }
    public Quaternion deltaRotation { get; private set; }
    public Vector3 deltaEuler { get; private set; }

    void Start()
    {
        try
        {
            // 打开已存在的共享内存-鼠标的数据
            _mouseMemFile = MemoryMappedFile.OpenExisting(
                MOUSE_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // 创建访问器
            _mouseAccessor = _mouseMemFile.CreateViewAccessor(
                0,  // 偏移量
                MOUSE_MEM_SIZE,
                MemoryMappedFileAccess.Read
            );

            // 打开已存在的共享内存-定位器的数据
            _locatorMemFile = MemoryMappedFile.OpenExisting(
                LOCATOR_MEM_NAME,
                MemoryMappedFileRights.Read
            );

            // 创建访问器
            _locatorAccessor = _locatorMemFile.CreateViewAccessor(
                0,  // 偏移量
                LOCATOR_MEM_SIZE,
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
        if (_mouseAccessor == null) return;

        try
        {
            // 读取数据结构
            IMUData data;
            LocatorData data2;
            _mouseAccessor.Read(0, out data);

            // 转换数据格式
            rawAcceleration = new Vector3(
                data.accelX,
                data.accelY,
                data.accelZ
            );

            // 注意：四元数坐标系的转换（可能需要调整符号）
            rawRotation = new Quaternion(
                data.quatX,
                data.quatY,
                data.quatZ,
                data.quatW
            );
            // Debug.Log("加速度数据:" + rawAcceleration + "四元数数据:" + rawRotation);

            _locatorAccessor.Read(0, out data2);

            rawTranslate = new Vector3(
                -data2.CoordX * X_Scale,
                data2.CoordY * Y_Scale,
                data2.CoordZ * Z_Scale
            ) * Scaler;
            // Debug.Log("定位器坐标数据:" + rawTranslate);

        }
        catch (Exception e)
        {
            Debug.LogError($"读取失败: {e.Message}");
        }

        if (rawRotation != lastRawRotation)
        {
            RotateFreq = 1 / LastRotateUpdateTime;
            //RotateFreq = Mathf.Clamp(RotateFreq, 0.01F, 1);
            LastRotateUpdateTime = 0f;
        }
        if (rawTranslate != lastRawTranslate)
        {
            PosFreq = 1 / LastPosUpdateTime;
            //PosFreq = Mathf.Clamp(PosFreq, 0.01F, 1);
            LastPosUpdateTime = 0f;
        }

        lastRawTranslate = rawTranslate;

        deltaRotation = rawRotation * Quaternion.Inverse(lastRawRotation);
        deltaEuler = rawRotation.eulerAngles - lastRawRotation.eulerAngles;
        lastRawRotation = rawRotation;

        PosFilter.UpdateParams(PosFreq, minCutoff, beta, dCutoff);

        // OE_Rotation = RotationFilter.Filter(Raw_Rotation);
        filteredTranslate = PosFilter.Filter(rawTranslate);

        deltaTranslate = filteredTranslate - lastFilteredTranslate;
        lastFilteredTranslate = filteredTranslate;


        LastPosUpdateTime += Time.deltaTime;
        LastRotateUpdateTime += Time.deltaTime;


    }

}
using UnityEngine;
using System;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Runtime.InteropServices;


public class SharedMemoryReader : MonoBehaviour
{
    public Transform TestObject;


    // 共享内存配置（需与Python端完全一致）
    private const string MOUSE_MEM_NAME = "FUKY_Mouse_Memory";
    private const int MOUSE_MEM_SIZE = 32; // 注意：Python端定义的32字节
    private const string LOCATOR_MEM_NAME = "FUKY_Locator_Memory";
    private const int LOCATOR_MEM_SIZE = 12; // 注意：Python端定义的32字节
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

    // 当前数据
    public Vector3 Acceleration { get; private set; }
    public Quaternion Rotation { get; private set; }

    public Vector3 Locator_TargetPos { get; private set; }

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
            Acceleration = new Vector3(
                data.accelX,
                data.accelY,
                data.accelZ
            );

            // 注意：四元数坐标系的转换（可能需要调整符号）
            Rotation = new Quaternion(
                data.quatX,
                data.quatY,
                data.quatZ,
                data.quatW
            );
            Debug.Log("加速度数据:"+ Acceleration + "四元数数据:" + Rotation);
            

            _locatorAccessor.Read(0, out data2);

            Locator_TargetPos = new Vector3(
                data2.CoordX,
                data2.CoordY,
                data2.CoordZ
            );
            Debug.Log("定位器坐标数据:" + Locator_TargetPos);
            TestObject.position = Locator_TargetPos;
            TestObject.rotation = Rotation;
        }
        catch (Exception e)
        {
            Debug.LogError($"读取失败: {e.Message}");
        }
    }

    void OnDestroy()
    {
        // 释放资源
        _mouseAccessor?.Dispose();
        _mouseMemFile?.Dispose();
    }
}
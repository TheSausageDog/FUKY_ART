using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using UnityEngine;

public class SimpleMemoryReader : MonoBehaviour
{
    // 共享内存配置（必须与Python代码完全一致）
    private const string IMU_MEM_NAME = "IMU_Memory";
    private const string BTN_MEM_NAME = "BTN_Memory";
    private const string PRESS_MEM_NAME = "PRESS_Memory";

    // 数据结构定义（必须与Python打包方式一致）
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct IMUData
    {
        public float accelX;
        public float accelY;
        public float accelZ;
        public float quatW;
        public float quatX;
        public float quatY;
        public float quatZ;
    }

    // 更新频率控制
    [Range(1, 60)]
    public int updateRate = 30;
    private float timer;

    void Update()
    {
        // 控制读取频率
        timer += Time.deltaTime;
        if (timer < 1f / updateRate) return;
        timer = 0;

        ReadAllData();
    }

    void ReadAllData()
    {
        try
        {
            // 读取IMU数据
            using (var mmf = MemoryMappedFile.OpenExisting(IMU_MEM_NAME))
            using (var view = mmf.CreateViewAccessor())
            {
                view.Read(0, out IMUData imuData);
                Debug.Log($"IMU - Accel: ({imuData.accelX:F2}, {imuData.accelY:F2}, {imuData.accelZ:F2}) " +
                        $"Quat: ({imuData.quatW:F4}, {imuData.quatX:F4}, {imuData.quatY:F4}, {imuData.quatZ:F4})");
            }

            // 读取按钮状态
            using (var mmf = MemoryMappedFile.OpenExisting(BTN_MEM_NAME))
            using (var view = mmf.CreateViewAccessor())
            {
                byte buttonState = view.ReadByte(0);
            }

            // 读取压力值
            using (var mmf = MemoryMappedFile.OpenExisting(PRESS_MEM_NAME))
            using (var view = mmf.CreateViewAccessor())
            {
                byte low = view.ReadByte(0);
                byte high = view.ReadByte(1);
                ushort pressure = (ushort)((high << 8) | low);
                Debug.Log($"Pressure: {pressure} (0x{pressure:X4})");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"读取失败: {e.GetType().Name} - {e.Message}");
        }
    }
}
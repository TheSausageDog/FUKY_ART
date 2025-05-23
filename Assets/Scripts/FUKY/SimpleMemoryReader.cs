using System;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using UnityEngine;

public class SimpleMemoryReader : MonoBehaviour
{
    // �����ڴ����ã�������Python������ȫһ�£�
    private const string IMU_MEM_NAME = "IMU_Memory";
    private const string BTN_MEM_NAME = "BTN_Memory";
    private const string PRESS_MEM_NAME = "PRESS_Memory";

    // ���ݽṹ���壨������Python�����ʽһ�£�
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

    // ����Ƶ�ʿ���
    [Range(1, 60)]
    public int updateRate = 30;
    private float timer;

    void Update()
    {
        // ���ƶ�ȡƵ��
        timer += Time.deltaTime;
        if (timer < 1f / updateRate) return;
        timer = 0;

        ReadAllData();
    }

    void ReadAllData()
    {
        try
        {
            // ��ȡIMU����
            using (var mmf = MemoryMappedFile.OpenExisting(IMU_MEM_NAME))
            using (var view = mmf.CreateViewAccessor())
            {
                view.Read(0, out IMUData imuData);
                Debug.Log($"IMU - Accel: ({imuData.accelX:F2}, {imuData.accelY:F2}, {imuData.accelZ:F2}) " +
                        $"Quat: ({imuData.quatW:F4}, {imuData.quatX:F4}, {imuData.quatY:F4}, {imuData.quatZ:F4})");
            }

            // ��ȡ��ť״̬
            using (var mmf = MemoryMappedFile.OpenExisting(BTN_MEM_NAME))
            using (var view = mmf.CreateViewAccessor())
            {
                byte buttonState = view.ReadByte(0);
            }

            // ��ȡѹ��ֵ
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
            Debug.LogError($"��ȡʧ��: {e.GetType().Name} - {e.Message}");
        }
    }
}
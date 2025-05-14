using System;
using System.Threading;
using UnityEngine;
using System.Collections.Concurrent;

public class WindowsEventTest : MonoBehaviour
{
    private EventWaitHandle eventHandle;
    private Thread eventThread;
    private bool running = true;
    private readonly ConcurrentQueue<Action> mainThreadActions = new ConcurrentQueue<Action>();

    void Start()
    {
        string eventName = "Global\\FukyDeviceEvent_UnityTestEvent";

        try
        {
            eventHandle = EventWaitHandle.OpenExisting(eventName);
            Debug.Log("[成功] 已连接到现有的Windows事件对象");
        }
        catch (WaitHandleCannotBeOpenedException)
        {
            eventHandle = new EventWaitHandle(
                false,
                EventResetMode.ManualReset,
                eventName);
            Debug.Log("[初始化] 创建了新的Windows事件对象");
        }

        eventThread = new Thread(EventMonitor);
        eventThread.IsBackground = true; // 设置为后台线程
        eventThread.Start();
    }

    void Update()
    {
        // 在主线程执行所有排队操作
        while (mainThreadActions.TryDequeue(out var action))
        {
            action();
        }
    }

    void EventMonitor()
    {
        while (running)
        {
            try
            {
                bool signaled = eventHandle.WaitOne(500); // 半秒检查一次

                if (signaled)
                {
                    Debug.Log("[事件] 检测到Windows事件触发");
                    mainThreadActions.Enqueue(() =>
                    {
                        // 现在在主线程执行
                        var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        sphere.transform.position = UnityEngine.Random.insideUnitSphere * 2f;
                        sphere.name = "EventSphere_" + Time.time;
                    });
                }
            }
            catch (Exception e)
            {
                mainThreadActions.Enqueue(() =>
                    Debug.LogError($"事件监听错误: {e.Message}"));
                break;
            }
        }
    }

    void OnDestroy()
    {
        running = false;
        eventThread?.Join(1000); // 等待线程结束（最多1秒）
        eventHandle?.Close();
        Debug.Log("[清理] 资源已释放");
    }

    void OnApplicationQuit()
    {
        OnDestroy(); // 确保退出时清理
    }
}
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 异步任务队列，每帧限制执行一定数量的任务（默认4个）。
/// 调用者可以通过 EnqueueTask 方法入队带返回值的任务。
/// </summary>
public class AsyncTaskQueue : SingletonMono<AsyncTaskQueue>
{
    
    [Tooltip("每帧最多执行的任务数量")]
    public int maxTasksPerFrame = 4;
    
    // 队列中存储的任务均为一个返回 UniTask 的无泛型委托
    private readonly Queue<Func<UniTask>> taskQueue = new Queue<Func<UniTask>>();

    /// <summary>
    /// 入队一个返回 T 结果的异步任务。
    /// </summary>
    public UniTask<T> EnqueueTask<T>(Func<UniTask<T>> taskFunc)
    {
        var tcs = new UniTaskCompletionSource<T>();
        // 包装为无泛型的任务委托，执行后将结果传递给 tcs
        taskQueue.Enqueue(async () =>
        {
            try
            {
                T result = await taskFunc();
                tcs.TrySetResult(result);
            }
            catch (Exception ex)
            {
                tcs.TrySetException(ex);
            }
        });
        return tcs.Task;
    }

    /// <summary>
    /// 持续不断地处理队列，每帧最多执行 maxTasksPerFrame 个任务。
    /// </summary>
    private async UniTask ProcessQueue()
    {
        while (true)
        {
            int tasksThisFrame = 0;
            while (taskQueue.Count > 0 && tasksThisFrame < maxTasksPerFrame)
            {
                var task = taskQueue.Dequeue();
                await task();
                tasksThisFrame++;
            }
            // 等待下一帧再处理剩余任务
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
    }

    private void Start()
    {
        // 开始异步处理队列（不会阻塞主线程）
        ProcessQueue().Forget();
    }
}
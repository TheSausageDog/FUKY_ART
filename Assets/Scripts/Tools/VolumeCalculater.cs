using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public static class VolumeCalculator
{
    /// <summary>
    /// 异步遍历传入物体的所有子物体，计算各自的体积并返回总和
    /// </summary>
    public static async UniTask<float> CalculateVolumesAsync(GameObject gameObject)
    {
        float totalVolume = 0f;
        List<UniTask<float>> tasks = new List<UniTask<float>>();
    
        // 遍历所有子物体（在主线程中操作）
        foreach (Transform child in gameObject.transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                continue;
        
            // 在主线程中获取 sharedMesh 数据，避免后台线程调用 Unity API
            Mesh sharedMesh = mf.sharedMesh;
            var childLocal = child;
            Renderer renderer = child.GetComponent<Renderer>();
            Bounds bounds = default;
            
            if (renderer == null)
            {
                bounds = renderer.bounds;
            }
            if (bounds == default)
                bounds = TransformBounds(sharedMesh.bounds, childLocal);
        
            var vertices = sharedMesh.vertices;
            var trianglesIndices = sharedMesh.triangles;
            int triangleCount = trianglesIndices.Length / 3;
            Triangle[] trianglesArray = new Triangle[triangleCount];
            for (int i = 0; i < triangleCount; i++)
            {
                Vector3 v0 = childLocal.TransformPoint(vertices[trianglesIndices[i * 3]]);
                Vector3 v1 = childLocal.TransformPoint(vertices[trianglesIndices[i * 3 + 1]]);
                Vector3 v2 = childLocal.TransformPoint(vertices[trianglesIndices[i * 3 + 2]]);
                trianglesArray[i] = new Triangle { v0 = v0, v1 = v1, v2 = v2 };
            }
            // 将体积计算任务通过队列调度，每帧只执行部分任务
            var task = AsyncTaskQueue.Instance.EnqueueTask<float>(() =>
                UniTask.Create(() => CalculateMeshVolume(bounds, trianglesArray, triangleCount))
                );
            tasks.Add(task);
        }

        float[] results = await UniTask.WhenAll(tasks);
        foreach (var v in results)
        {
            totalVolume += v;
        }
        Debug.Log($"{gameObject.name}的体积: " + totalVolume);
    
        return totalVolume * 10;
    }


    /// <summary>
    /// 同步调用 GPU 计算单个 Mesh 的体积（在 UniTask.Run 中调用，避免阻塞主线程）
    /// </summary>
    private static UniTask<float> CalculateMeshVolume(Bounds bounds,Triangle[] trianglesArray,int triangleCount)
    {
        return new UniTask<float>( ComputeVolumeCalculatorGPU.Instance.CalculateVolume(bounds, trianglesArray, triangleCount));
    }

    /// <summary>
    /// 计算物体在 world 坐标系中的包围盒
    /// </summary>
    public static Bounds CalculateWorldBounds(GameObject gameObject)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
    
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);

        Bounds worldBounds = renderers[0].bounds;
        foreach (Renderer renderer in renderers)
        {
            worldBounds.Encapsulate(renderer.bounds);
        }
        return worldBounds;
    }

    /// <summary>
    /// 将局部包围盒转换为 world 坐标下的包围盒
    /// </summary>
    public static Bounds TransformBounds(Bounds localBounds, Transform t)
    {
        Vector3 center = t.TransformPoint(localBounds.center);
        Vector3 extents = localBounds.extents;
        Vector3 axisX = t.TransformVector(new Vector3(extents.x, 0, 0));
        Vector3 axisY = t.TransformVector(new Vector3(0, extents.y, 0));
        Vector3 axisZ = t.TransformVector(new Vector3(0, 0, extents.z));
        extents.x = Mathf.Abs(axisX.x) + Mathf.Abs(axisY.x) + Mathf.Abs(axisZ.x);
        extents.y = Mathf.Abs(axisX.y) + Mathf.Abs(axisY.y) + Mathf.Abs(axisZ.y);
        extents.z = Mathf.Abs(axisX.z) + Mathf.Abs(axisY.z) + Mathf.Abs(axisZ.z);
        return new Bounds(center, extents * 2);
    }
}

using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

public static class VolumeCalculator
{
    // 体素大小，可根据需求调整
    private static float voxelSize = 0.05f;

    /// <summary>
    /// 遍历传入物体的所有子物体，计算各自的体积并返回总和
    /// </summary>
    public static float CalculateVolumes(GameObject gameObject)
    {
        float totalVolume = 0f;

        // 遍历所有子物体
        foreach (Transform child in gameObject.transform)
        {
            MeshFilter mf = child.GetComponent<MeshFilter>();
            if (mf == null || mf.sharedMesh == null)
                continue;

            // 采用 Job System 计算该网格的体积
            float meshVolume = CalculateMeshVolume(child, mf.sharedMesh);
            Debug.Log(child.name + " 体积约为: " + meshVolume);
            totalVolume += meshVolume;
        }
//        Debug.Log("所有子物体总体积约为: " + totalVolume);
        return totalVolume * 10;
    }
    /// <summary>
    /// 计算物体在世界坐标系中的包围盒
    /// </summary>
    public static Bounds CalculateWorldBounds(GameObject gameObject)
    {
        // 获取所有 MeshRenderer 和 MeshFilter 组件，包括当前 GameObject 上的 Renderer
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
    
        // 初始化一个包围盒，给它一个有效的初始值
        if (renderers.Length == 0)
            return new Bounds(Vector3.zero, Vector3.zero);  // 没有任何Renderer时返回空包围盒

        Bounds worldBounds = renderers[0].bounds; // 使用第一个 Renderer 的包围盒初始化

        foreach (Renderer renderer in renderers)
        {
            worldBounds.Encapsulate(renderer.bounds);  // 合并当前 Renderer 的包围盒
        }

        return worldBounds;
    }


    /// <summary>
    /// 使用体素采样+Job System计算单个 Mesh 的体积
    /// </summary>
    private static float CalculateMeshVolume(Transform meshTransform, Mesh mesh)
    {
        // 获取物体包围盒：优先使用 Renderer 的 bounds，否则用 mesh.bounds 进行转换
        Renderer renderer = meshTransform.GetComponent<Renderer>();
        Bounds bounds;
        if (renderer != null)
            bounds = renderer.bounds;
        else
            bounds = TransformBounds(mesh.bounds, meshTransform);

        // 为避免边界遗漏，稍微扩展包围盒
        bounds.Expand(voxelSize * 0.5f);

        // 构建包围盒内的采样点（以体素为间隔）
        List<float3> pointsList = new List<float3>();
        for (float x = bounds.min.x; x <= bounds.max.x; x += voxelSize)
        {
            for (float y = bounds.min.y; y <= bounds.max.y; y += voxelSize)
            {
                for (float z = bounds.min.z; z <= bounds.max.z; z += voxelSize)
                {
                    pointsList.Add(new float3(x, y, z));
                }
            }
        }
        int pointCount = pointsList.Count;
        NativeArray<float3> samplePoints = new NativeArray<float3>(pointCount, Allocator.TempJob);
        for (int i = 0; i < pointCount; i++)
        {
            samplePoints[i] = pointsList[i];
        }

        // 将 Mesh 的三角形数据转换为 world 坐标，并存入 NativeArray 供 Job 使用
        Vector3[] vertices = mesh.vertices;
        int[] trianglesIndices = mesh.triangles;
        int triangleCount = trianglesIndices.Length / 3;
        NativeArray<Triangle> triangleArray = new NativeArray<Triangle>(triangleCount, Allocator.TempJob);
        for (int i = 0; i < triangleCount; i++)
        {
            Vector3 v0 = meshTransform.TransformPoint(vertices[trianglesIndices[i * 3]]);
            Vector3 v1 = meshTransform.TransformPoint(vertices[trianglesIndices[i * 3 + 1]]);
            Vector3 v2 = meshTransform.TransformPoint(vertices[trianglesIndices[i * 3 + 2]]);
            triangleArray[i] = new Triangle { v0 = (float3)v0, v1 = (float3)v1, v2 = (float3)v2 };
        }

        // 结果数组，每个采样点判断结果 1 表示在网格内，0 表示在外
        NativeArray<int> results = new NativeArray<int>(pointCount, Allocator.TempJob);

        // 构建 Job 并调度
        VoxelVolumeJob job = new VoxelVolumeJob
        {
            samplePoints = samplePoints,
            triangles = triangleArray,
            results = results,
        };

        JobHandle handle = job.Schedule(pointCount, 64);
        handle.Complete();

        // 累加在网格内的采样点数量
        int countInside = 0;
        for (int i = 0; i < pointCount; i++)
        {
            countInside += results[i];
        }
        float volume = countInside * voxelSize * voxelSize * voxelSize;

        // 清理 NativeArray
        samplePoints.Dispose();
        triangleArray.Dispose();
        results.Dispose();

        return volume;
    }

    /// <summary>
    /// 将局部包围盒转换为 world 坐标下的包围盒
    /// </summary>
    private static Bounds TransformBounds(Bounds localBounds, Transform t)
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

[BurstCompile]
public struct VoxelVolumeJob : IJobParallelFor
{
    [ReadOnly] public NativeArray<float3> samplePoints;
    [ReadOnly] public NativeArray<Triangle> triangles;
    public NativeArray<int> results;

    public void Execute(int index)
    {
        float3 point = samplePoints[index];
        // 固定射线方向（向右）
        float3 rayDir = new float3(1f, 0f, 0f);
        int hitCount = 0;
        // 对所有三角形进行射线交点检测
        for (int i = 0; i < triangles.Length; i++)
        {
            if (RayIntersectsTriangle(point, rayDir, triangles[i]))
            {
                hitCount++;
            }
        }
        // 奇偶规则：交点数为奇数则认为采样点在网格内部
        results[index] = (hitCount % 2 == 1) ? 1 : 0;
    }

    /// <summary>
    /// 利用 Möller–Trumbore 算法判断射线与三角形是否相交
    /// </summary>
    private bool RayIntersectsTriangle(float3 origin, float3 dir, Triangle tri)
    {
        float3 v0 = tri.v0;
        float3 v1 = tri.v1;
        float3 v2 = tri.v2;
        float3 edge1 = v1 - v0;
        float3 edge2 = v2 - v0;
        float3 h = math.cross(dir, edge2);
        float a = math.dot(edge1, h);
        if (math.abs(a) < 1e-5f)
            return false; // 平行于三角形

        float f = 1.0f / a;
        float3 s = origin - v0;
        float u = f * math.dot(s, h);
        if (u < 0f || u > 1f)
            return false;

        float3 q = math.cross(s, edge1);
        float v = f * math.dot(dir, q);
        if (v < 0f || u + v > 1f)
            return false;

        float t = f * math.dot(edge2, q);
        return t > 1e-5f; // t大于0，表示交点在射线正方向上
    }
}

/// <summary>
/// 三角形数据结构，用于存储三角形顶点（world 坐标）
/// </summary>
public struct Triangle
{
    public float3 v0;
    public float3 v1;
    public float3 v2;
}

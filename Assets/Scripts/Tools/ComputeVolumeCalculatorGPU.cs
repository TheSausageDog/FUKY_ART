using UnityEngine;

public class ComputeVolumeCalculatorGPU : SingletonMono<ComputeVolumeCalculatorGPU>
{
    public ComputeShader voxelComputeShader;
    public float voxelSize = 0.01f;

    // 计算单个 Mesh 的体积（Mesh 已转换为 world 坐标）
    public float CalculateVolume(Bounds bounds,Triangle[] trianglesArray,int triangleCount)
    {
        // 1. 计算包围盒（优先使用 Renderer.bounds，否则转换 mesh.bounds）
        //Renderer renderer = meshTransform.GetComponent<Renderer>();

        // 为避免遗漏，扩展包围盒
        bounds.Expand(voxelSize * 0.5f);

        // 2. 计算采样网格的维度（确保包含包围盒边界）
        int gridDimX = Mathf.CeilToInt((bounds.max.x - bounds.min.x) / voxelSize) + 1;
        int gridDimY = Mathf.CeilToInt((bounds.max.y - bounds.min.y) / voxelSize) + 1;
        int gridDimZ = Mathf.CeilToInt((bounds.max.z - bounds.min.z) / voxelSize) + 1;
        int totalSamples = gridDimX * gridDimY * gridDimZ;

        // 3. 构造三角形数据（转换为 world 坐标）




        // 4. 创建 ComputeBuffer 用于传递三角形数据和存放结果
        ComputeBuffer trianglesBuffer = new ComputeBuffer(triangleCount, sizeof(float) * 9);
        trianglesBuffer.SetData(trianglesArray);

        ComputeBuffer resultsBuffer = new ComputeBuffer(totalSamples, sizeof(int));

        // 5. 设置 Compute Shader 参数
        int kernel = voxelComputeShader.FindKernel("CSMain");
        voxelComputeShader.SetBuffer(kernel, "triangles", trianglesBuffer);
        voxelComputeShader.SetBuffer(kernel, "results", resultsBuffer);
        voxelComputeShader.SetVector("boundsMin", bounds.min);
        voxelComputeShader.SetFloat("voxelSize", voxelSize);
        voxelComputeShader.SetInt("gridDimX", gridDimX);
        voxelComputeShader.SetInt("gridDimY", gridDimY);
        voxelComputeShader.SetInt("gridDimZ", gridDimZ);

        // 6. 根据总采样点数和线程组大小调度 Compute Shader
        int threadGroups = Mathf.CeilToInt(totalSamples / 64.0f);
        voxelComputeShader.Dispatch(kernel, threadGroups, 1, 1);

        // 7. 读取结果并统计内部采样点数量
        int[] results = new int[totalSamples];
        resultsBuffer.GetData(results);
        int countInside = 0;
        for (int i = 0; i < results.Length; i++)
        {
            countInside += results[i];
        }
        float volume = countInside * voxelSize * voxelSize * voxelSize;

        // 8. 释放 ComputeBuffer
        trianglesBuffer.Release();
        resultsBuffer.Release();

        return volume;
    }

    // 与 Compute Shader 对应的三角形数据结构

}
public struct Triangle
{
    public Vector3 v0;
    public Vector3 v1;
    public Vector3 v2;
}
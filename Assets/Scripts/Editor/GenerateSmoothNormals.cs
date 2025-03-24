using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine.Rendering;

// using System.Diagnostics;

namespace ProjectWK
{
    public enum WRITETYPE
    {
        VertexColor=0,
        Tangent=1,
        UV3 = 2,
        // Texter=2,
    }
    
    public struct NormalWeight {
        public Vector3 normal;
        public float weight;
    }

    public class GenerateSmoothNormals : EditorWindow
    {
    public WRITETYPE wt = WRITETYPE.UV3; 
    // public bool customMesh;

    [MenuItem("ArtTools/平滑法线工具_TS")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GenerateSmoothNormals));//显示现有窗口实例。如果没有，请创建一个。
    }

    private bool tangentSpace = true;
    private string savePath = "Assets";
        
    void OnGUI()
    {
        
        GUILayout.Space(5);
        GUILayout.Label ("1、请在Scene中选择需要平滑法线的物体", EditorStyles.boldLabel);
        // mesh = (MeshFilter)EditorGUILayout.ObjectField(mesh,typeof(MeshFilter),true);
        GUILayout.Space(10);
        GUILayout.Label ("2、请选择需要写入平滑后的物体空间法线数据的目标", EditorStyles.boldLabel);
        wt = (WRITETYPE)EditorGUILayout.EnumPopup("写入目标",wt);
        GUILayout.Space(10);

        tangentSpace = GUILayout.Toggle(tangentSpace, "平滑法线在切线空间(用于UV3)");
        if (wt==WRITETYPE.Tangent)
        {
            if (tangentSpace==true)
            {
                Debug.LogError("写入目标为切线时不可以将平滑法线存储为切线空间");
                tangentSpace = false;
            }
        }
        GUILayout.Space(10);

        // savePath = GUILayout.(savePath, "保存路径");
        savePath = EditorGUILayout.TextField("保存路径：", savePath);
        if (GUILayout.Button("设置保存路径"))
        {
            // var currentRampObjPath = AssetDatabase.GetAssetPath(m_MaterialEditor.serializedObject.targetObject);
            // savePath = System.IO.Path.GetDirectoryName(savePath);
            
            savePath = EditorUtility.SaveFolderPanel("Select an output path", "", "");
            // 将绝对路径转换为相对路径
            savePath = "Assets" + savePath.Substring(Application.dataPath.Length);
            
            // string assetsPath = "Assets" + savePath.Substring(Application.dataPath.Length);
            Debug.Log(savePath);
        }

        GUILayout.Space(10);
        
        switch(wt){
            case WRITETYPE.Tangent://执行写入到 顶点切线
                        GUILayout.Label ("  将会把平滑后的法线写入到顶点切线中", EditorStyles.boldLabel);
                        break;
            case WRITETYPE.VertexColor:// 写入到顶点色
                        GUILayout.Label ("  将会把平滑后的法线写入到顶点色的RGB中，A保持不变", EditorStyles.boldLabel); 
                        break;   
            case WRITETYPE.UV3:// 写入到UV3中
                        GUILayout.Label ("  将会把平滑后的法线写入到UV3中", EditorStyles.boldLabel);
                        break;
        }
        
        if(GUILayout.Button("4、导出Mesh")){
            selectMesh();
        }

    }
    public  void SmoothNormalPrev(WRITETYPE wt)//Mesh选择器 修改并预览
    {  


        if(Selection.activeGameObject==null){//检测是否获取到物体
            Debug.LogError("请选择物体");
            return ;
        }
        MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinMeshRenders = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var meshFilter in meshFilters)//遍历两种Mesh 调用平滑法线方法
        {
            Mesh mesh = meshFilter.sharedMesh;
            Vector3 [] averageNormals= AverageNormal(mesh);
            write2mesh(mesh,averageNormals);
        }
        foreach (var skinMeshRender in skinMeshRenders)
        {   
            Mesh mesh = skinMeshRender.sharedMesh;
            Vector3 [] averageNormals= AverageNormal(mesh);
            write2mesh(mesh,averageNormals);
        }
    }

    public Vector3[] AverageNormal(Mesh mesh)
    {

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        
        var averageNormalHash = new Dictionary<Vector3, Vector3>();
        for (var j = 0; j < mesh.vertexCount; j++)
        {
            if (!averageNormalHash.ContainsKey(vertices[j]))
            {
                averageNormalHash.Add(vertices[j],normals[j]);
            }
            else
            {
                averageNormalHash[vertices[j]] =
                    (averageNormalHash[vertices[j]] + normals[j]).normalized;
            }
        }

        Vector4[] tangents = mesh.tangents;
        var averageNormals = new Vector3[mesh.vertexCount];

        if (tangentSpace == true)
        {
            for (var j = 0; j < mesh.vertexCount; j++)
            {
                //转换为切线空间
                Vector3 tangent = tangents[j];
                Vector3 normal = normals[j];
                Vector3 binormal = Vector3.Cross(normal, tangent) * tangents[j].w;
                Matrix4x4 tbnMatrix = Matrix4x4.identity;
                tbnMatrix.SetRow(0,new Vector4(tangent.x,tangent.y,tangent.z,0));
                tbnMatrix.SetRow(1,new Vector4(binormal.x,binormal.y,binormal.z,0));
                tbnMatrix.SetRow(2,new Vector4(normal.x,normal.y,normal.z,0));
                tbnMatrix.SetRow(3,new Vector4(0,0,0,0));
        
                averageNormals[j] = tbnMatrix * averageNormalHash[vertices[j]];
        
                Debug.Log("存储为切线空间");
                // averageNormals[j] = averageNormalHash[mesh.vertices[j]];
                // averageNormals[j] = averageNormals[j].normalized;
            }   
        }
        
        return averageNormals;
    } 
    
    Vector3[] SmoothNormals(Mesh mesh)
    {                                                                                 //一个顶点存着多条法线
        Dictionary<Vector3, List<NormalWeight>> normalDict = new Dictionary<Vector3, List<NormalWeight>>();
        var triangles = mesh.triangles;
        var vertices = mesh.vertices;
        var normals = mesh.normals;
        var tangents = mesh.tangents;
        var smoothNormals = mesh.normals;

        for (int i = 0; i <= triangles.Length - 3; i += 3)
        {
            int[] triangle = new int[] {triangles[i], triangles[i+1], triangles[i+2]};
            for (int j = 0; j < 3; j++)
            {
                int vertexIndex = triangle[j];
                Vector3 vertex = vertices[vertexIndex];
                if (!normalDict.ContainsKey(vertex))  //如果没有包含这个顶点，进行初始化
                {
                    normalDict.Add(vertex, new List<NormalWeight>());
                }

                NormalWeight nw;
                Vector3 lineA = Vector3.zero;
                Vector3 lineB = Vector3.zero;
                if (j == 0)
                {
                    lineA = vertices[triangle[1]] - vertex;
                    lineB = vertices[triangle[2]] - vertex;
                }
                else if (j == 1)
                {
                    lineA = vertices[triangle[2]] - vertex;
                    lineB = vertices[triangle[0]] - vertex;
                }
                else
                {
                    lineA = vertices[triangle[0]] - vertex;
                    lineB = vertices[triangle[1]] - vertex;
                }
                //修复精度问题
                lineA *= 10000.0f;  
                lineB *= 10000.0f;  
                float angle = Mathf.Acos(Mathf.Max(Mathf.Min(Vector3.Dot(lineA, lineB)/(lineA.magnitude  * lineB.magnitude), 1), -1));
                nw.normal = Vector3.Cross(lineA, lineB).normalized;
                nw.weight = angle;
                normalDict[vertex].Add(nw);
            }
        }

        for (int i = 0; i < vertices.Length; i++) {
            Vector3 vertex = vertices[i];
            if (!normalDict.ContainsKey(vertex)) {
                continue;
            }
            List<NormalWeight> normalList = normalDict[vertex];

            Vector3 smoothNormal = Vector3.zero;
            float weightSum = 0;
            for (int j = 0; j < normalList.Count; j++)
            {
                NormalWeight nw = normalList[j];
                weightSum += nw.weight;                          //计算整体权重
            }


            for (int j = 0; j < normalList.Count; j++)
            {
                NormalWeight nw = normalList[j];
                smoothNormal += nw.normal * nw.weight/weightSum; //计算平滑法线
            }


            smoothNormal = smoothNormal.normalized;
            smoothNormals[i] = smoothNormal;

            //objectSpace to tangentSpace
            if (tangentSpace == true)
            {
                var normal = normals[i];
                var tangent = tangents[i];
                var binormal = (Vector3.Cross(normal, tangent) * tangent.w).normalized;
                var tbn = new Matrix4x4(tangent, binormal, normal, Vector3.zero);
                tbn = tbn.transpose;
                smoothNormals[i] = tbn.MultiplyVector(smoothNormals[i]).normalized;
            }
        }
        // mesh.SetUVs(3, smoothNormals);
        return smoothNormals;
    }
    
    
    public void write2mesh(Mesh mesh,Vector3[] averageNormals){
        switch(wt){
            case WRITETYPE.Tangent://执行写入到 顶点切线
                    var tangents = new Vector4[mesh.vertexCount];
                    for (var j = 0; j < mesh.vertexCount; j++)
                    {
                        tangents[j] = new Vector4(averageNormals[j].x, averageNormals[j].y, averageNormals[j].z, 0);
                    }
                    mesh.tangents = tangents;
            break;
            case WRITETYPE.VertexColor:// 写入到顶点色
                    Color[] _colors = new Color[mesh.vertexCount];
                    Color[] _colors2 = new Color[mesh.vertexCount];
                    _colors2=mesh.colors;
                    for (var j = 0; j < mesh.vertexCount; j++)
                    {
                        _colors[j] = new Vector4(averageNormals[j].x, averageNormals[j].y, averageNormals[j].z,_colors2[j].a);       
                    }   
                    mesh.colors = _colors;
            break;
            }
        }

    public Vector3[] GetSmoothNormal(Mesh sourceMesh, Mesh targetMesh)
    {
        if (sourceMesh != null && targetMesh != null)
        {
            Vector3[] sourceNormals = sourceMesh.normals;
            Vector3[] targetNormals = new Vector3[targetMesh.vertexCount];
            
            List<Vector3> targetVert = new List<Vector3>();
            targetMesh.GetVertices(targetVert);
            List<Vector3> sourceVert = new List<Vector3>();
            sourceMesh.GetVertices(sourceVert);
            
            System.Diagnostics.Stopwatch stopWatch = new  System.Diagnostics.Stopwatch();
            stopWatch.Start();

            int targetCount = targetMesh.vertexCount;
            int sourceCount = sourceMesh.vertexCount;
            
                    // 使用并行循环
            Parallel.For(0, targetCount, i =>
            {
                Vector3 targetVertex = targetVert[i];
                double closestDistance = double.MaxValue;
                int closestIndex = 0;


                for (int j = 0; j < sourceCount; j++)
                {
                    double distance = Vector3.Distance(targetVertex, sourceVert[j]);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestIndex = j;
                        // if (distance <= 0.0001f)
                        // {
                        //     break;
                        // }
                    }
                }

                targetNormals[i] = sourceNormals[closestIndex];
            });
            stopWatch.Stop();
            Debug.Log("烘焙法线用时" + stopWatch.ElapsedMilliseconds * 0.001);
            
            Vector4[] tangents = targetMesh.tangents;
            Vector3[] normals = targetMesh.normals;
            var averageNormals = new Vector3[targetCount];

            if (tangentSpace)
            {
                for (var j = 0; j < targetCount; j++)
                {
                    //转换为切线空间
                    Vector3 tangent = tangents[j];
                    Vector3 normal = normals[j];
                    Vector3 binormal = Vector3.Cross(normal, tangent) * tangents[j].w;
                    Matrix4x4 tbnMatrix = Matrix4x4.identity;
                    tbnMatrix.SetRow(0,new Vector4(tangent.x,tangent.y,tangent.z,0));
                    tbnMatrix.SetRow(1,new Vector4(binormal.x,binormal.y,binormal.z,0));
                    tbnMatrix.SetRow(2,new Vector4(normal.x,normal.y,normal.z,0));
                    tbnMatrix.SetRow(3,new Vector4(0,0,0,0));
            
                    averageNormals[j] = tbnMatrix * targetNormals[j];
                    Debug.Log("存储为切线空间");
                }   
            }
            return averageNormals;
        }

        return null;
    }

    public void selectMesh(){

        if(Selection.activeGameObject==null){//检测是否获取到物体
            Debug.LogError("请选择物体");
            return ;
        }
        MeshFilter[] meshFilters = Selection.activeGameObject.GetComponentsInChildren<MeshFilter>();
        SkinnedMeshRenderer[] skinMeshRenders = Selection.activeGameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
       
        
        foreach (var meshFilter in meshFilters)//遍历两种Mesh 调用平滑法线方法
        {
            // Mesh mesh=new Mesh();//target hard normal
            // Copy(mesh, meshFilter.sharedMesh);
            //
            // string path = AssetDatabase.GetAssetPath(meshFilter.sharedMesh);
            // ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter; 
            // modelImporter.importNormals = ModelImporterNormals.Calculate;
            // modelImporter.normalSmoothingAngle = 180;
            // AssetDatabase.ImportAsset(path);
            //
            // Mesh smoothMesh = new Mesh();//source smooth normal
            // Copy(smoothMesh,meshFilter.sharedMesh);
            //
            // // Vector3 [] averageNormals= AverageNormal(mesh);
            // Vector3 [] averageNormals= GetSmoothNormal(smoothMesh,mesh);
            //
            // exportMesh(mesh,averageNormals);
            //
            // ModelImporter close_modelImporter = AssetImporter.GetAtPath(path) as ModelImporter; 
            // close_modelImporter.importNormals = ModelImporterNormals.Import;
            // close_modelImporter.normalSmoothingAngle = 60;
            // AssetDatabase.ImportAsset(path);
            
            Mesh mesh = meshFilter.sharedMesh;
            // Vector3 [] averageNormals= AverageNormal(mesh);
            Vector3 [] averageNormals= SmoothNormals(mesh);
            exportMesh(mesh,averageNormals);
            
        }
        foreach (var skinMeshRender in skinMeshRenders)
        {   
            Mesh mesh = skinMeshRender.sharedMesh;
            // Vector3 [] averageNormals= AverageNormal(mesh);
            Vector3 [] averageNormals= SmoothNormals(mesh);
            exportMesh(mesh,averageNormals);
            
            // Mesh mesh=new Mesh();//target hard normal
            // Copy(mesh, skinMeshRender.sharedMesh);
            //
            // string path = AssetDatabase.GetAssetPath(skinMeshRender.sharedMesh);
            // ModelImporter modelImporter = AssetImporter.GetAtPath(path) as ModelImporter; 
            // modelImporter.importNormals = ModelImporterNormals.Calculate;
            // modelImporter.normalSmoothingAngle = 180;
            // AssetDatabase.ImportAsset(path);
            //
            // Mesh smoothMesh = new Mesh();//source smooth normal
            // Copy(smoothMesh,skinMeshRender.sharedMesh);
            //
            // // Vector3 [] averageNormals= AverageNormal(mesh);
            // Vector3 [] averageNormals= GetSmoothNormal(smoothMesh,mesh);
            //
            // exportMesh(mesh,averageNormals);
            //
            // ModelImporter close_modelImporter = AssetImporter.GetAtPath(path) as ModelImporter; 
            // close_modelImporter.importNormals = ModelImporterNormals.Import;
            // close_modelImporter.normalSmoothingAngle = 60;
            // AssetDatabase.ImportAsset(path);
        }
    }
    

    
     public void Copy(Mesh dest, Mesh src)
    {
        dest.Clear();
        dest.vertices = src.vertices;

        List<Vector4> uvs = new List<Vector4>();

        src.GetUVs(0, uvs); dest.SetUVs(0, uvs);
        src.GetUVs(1, uvs); dest.SetUVs(1, uvs);
        src.GetUVs(2, uvs); dest.SetUVs(2, uvs);
        src.GetUVs(3, uvs); dest.SetUVs(3, uvs);

        dest.normals = src.normals;
        dest.tangents = src.tangents;
        dest.boneWeights = src.boneWeights;
        dest.colors = src.colors;
        dest.colors32 = src.colors32;
        dest.bindposes = src.bindposes;

        dest.subMeshCount = src.subMeshCount;

        for (int i = 0; i < src.subMeshCount; i++)
            dest.SetIndices(src.GetIndices(i), src.GetTopology(i), i);

        dest.name = src.name ;
    }
    public void exportMesh(Mesh mesh,Vector3[] averageNormals){
        Mesh mesh2=new Mesh();
        Copy(mesh2,mesh);
        switch(wt){
            case WRITETYPE.Tangent://执行写入到 顶点切线
                    Debug.Log("写入到切线中");
                    var tangents = new Vector4[mesh2.vertexCount];
                    for (var j = 0; j < mesh2.vertexCount; j++)
                    {
                        tangents[j] = new Vector4(averageNormals[j].x, averageNormals[j].y, averageNormals[j].z, 0);
                    }
                    mesh2.tangents = tangents;
            break;
            case WRITETYPE.VertexColor:// 写入到顶点色
                Debug.Log("写入到顶点色");
                Color[] _colors = new Color[mesh2.vertexCount];
                    Color[] _colors2 = new Color[mesh2.vertexCount];
                    _colors2=mesh2.colors;
                    for (var j = 0; j < mesh2.vertexCount; j++)
                    {
                        _colors[j] = new Vector4(averageNormals[j].x, averageNormals[j].y, averageNormals[j].z);       
                    }   
                    mesh2.colors = _colors;
                    break;
            case WRITETYPE.UV3:// 写入到UV3中
                Debug.Log("写入到UV3中");
                mesh2.SetUVs(3,averageNormals);
                break;
            
            }

        //创建文件夹路径
        Debug.Log(savePath);
        // 判断文件夹路径是否存在
        if (!AssetDatabase.IsValidFolder(savePath))
        {
            // 创建文件夹
            AssetDatabase.CreateFolder(Path.GetDirectoryName(savePath), Path.GetFileName(savePath));
        }
        //刷新
        AssetDatabase.Refresh();
        
        
        string smoothDataState = "";
        switch (wt)
        {
            case WRITETYPE.Tangent:
                smoothDataState = "_Tangent";
                break;
            case WRITETYPE.VertexColor:
                smoothDataState = "_VertexColor";
                break;
            case WRITETYPE.UV3:
                smoothDataState = "_UV3";
                break;
        }
        smoothDataState += tangentSpace ? "_TS" : "_OS";
        
        mesh2.name=mesh2.name+"_SMNormal"+smoothDataState;
        Debug.Log(mesh2.vertexCount);
        AssetDatabase.CreateAsset(mesh2, savePath + "/" + mesh2.name + ".asset");

    }     
    }
}

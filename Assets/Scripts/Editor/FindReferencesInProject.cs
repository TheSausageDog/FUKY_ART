using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ProjectWK
{
    public class FcArtAssetsEx : EditorWindow
    {
        #region Init

        static FcArtAssetsEx Instance;
        static List<string> _filePrefabPath = new List<string>();
        static List<PrefabDependencies> _prefabs = new List<PrefabDependencies>();
        static List<string> _readyToFind;
        static bool _path;
        static public AssetUsedData _AssetUsedData;
        static public AllAssetsData _AllAssetsData;
        static public string _queryScope = Application.dataPath;

        //查询文件类型
        static public string[] _AssetType = new String[] { ".unity", ".mat", ".prefab" };
        static public int _AssetTypeIndxe = 1;

        public static void Init()
        {
            _filePrefabPath.Add(_queryScope);
            //_filePrefabPath.Add(Application.dataPath);
            Instance = GetWindow<FcArtAssetsEx>();
        }


        [MenuItem("Assets/在项目中查找资源引用", false, 30)]
        public static void FindUsePrefab()
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (Instance == null)
            {
                _queryScope = EditorUtility.OpenFolderPanel("选择查询范围", Application.dataPath, "");
                if (string.IsNullOrEmpty(_queryScope))
                {
                    Debug.LogError("请选择查询范围");
                    return;
                }
                Init();
                Find();
            }

            _AssetUsedData = new AssetUsedData(path);
            _AssetUsedData.FindUsed(_prefabs);
        }

        void OnGUI()
        {
            GUILayout.TextField("查询范围: "+_queryScope);
            if (_prefabs != null && _AllAssetsData != null)
            {
                EditorGUILayout.BeginVertical();
                //EditorGUILayout.LabelField("项目的预制与场景数目:" + _prefabs.Count);
                //EditorGUILayout.LabelField("项目使用的文件类型数:" + _AllAssetsData._data.Count);

                //检测按键更新查找数据
                EditorGUI.BeginChangeCheck();
                _AssetTypeIndxe = GUILayout.Toolbar(_AssetTypeIndxe, new[] { "场景引用", "材质引用", "预制体引用" });
                if (EditorGUI.EndChangeCheck())
                {
                    Find();
                    _AssetUsedData.FindUsed(_prefabs);
                }

                EditorGUILayout.EndVertical();
                ShowTest();
            }
        }

        Vector2 pos;

        void ShowTest()
        {
            if (_AssetUsedData != null)
            {
                pos = EditorGUILayout.BeginScrollView(pos);
                _AssetUsedData.OnGUI();
                EditorGUILayout.EndScrollView();
            }
        }

        #endregion

        #region Find

        static void Find()
        {
            if (_filePrefabPath == null || _filePrefabPath.Count == 0)
                _filePrefabPath.Add(Application.dataPath);
            _readyToFind = new List<string>();
            _prefabs = new List<PrefabDependencies>();
            _AllAssetsData = new AllAssetsData();
            for (int i = 0; i < _filePrefabPath.Count; i++)
            {
                FindAllPath(_filePrefabPath[i]);
            }

            CreatePrefabData();
            RestPrefabDependencie();
        }

        static void CreatePrefabData()
        {
            for (int i = 0; i < _readyToFind.Count; i++)
            {
                string expandname = Path.GetExtension(_readyToFind[i]);
                //if (expandname == ".prefab" || expandname == ".unity" )
                if (expandname == _AssetType[_AssetTypeIndxe])
                {
                    PrefabDependencies pd = new PrefabDependencies(_readyToFind[i]);
                    _prefabs.Add(pd);
                }

                _AllAssetsData.AddAssets(_readyToFind[i]);
            }
        }

        static void RestPrefabDependencie()
        {
            for (int i = 0; i < _prefabs.Count; i++)
            {
                _prefabs[i].GetDependencies();
                if (EditorUtility.DisplayCancelableProgressBar("获取索引", "GetDependencie:" + i,
                        (float)i / _prefabs.Count))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
            }

            EditorUtility.ClearProgressBar();
        }

        static void FindAllPath(string path)
        {
            string[] Directorys = new string[0];
            try
            {
                Directorys = Directory.GetFiles(path);
            }
            catch
            {
            }

            if (Directorys != null)
            {
                for (var i = 0; i < Directorys.Length; i++)
                {
                    if (!_readyToFind.Contains(Directorys[i]))
                        _readyToFind.Add(Directorys[i]);
                }
            }

            Directorys = Directory.GetDirectories(path);
            for (int i = 0; i < Directorys.Length; i++)
            {
                string newpath;
                newpath = Path.GetDirectoryName(Directorys[i]) + "/" + Path.GetFileName(Directorys[i]);
                FindAllPath(newpath);
            }
        }

        #endregion
    }

    public class AllAssetsData
    {

        public List<AssetsUsedData> _data = new List<AssetsUsedData>();

        public void AddAssets(string path)
        {
            string expandname = Path.GetExtension(path);
            if (expandname == ".meta")
                return;
            int id = IsContainsKey(expandname);
            if (id == -1)
            {
                _data.Add(new AssetsUsedData(expandname));
                id = _data.Count - 1;
            }

            _data[id].AddData(path);
        }

        public int IsContainsKey(string expandname)
        {
            for (int i = 0; i < _data.Count; i++)
            {
                if (_data[i]._expandname == expandname)
                    return i;
            }

            return -1;
        }

    }

    public class AssetsUsedData
    {
        public string _expandname;

        public AssetsUsedData(string expandname)
        {
            _expandname = expandname;
        }

        public List<AssetUsedData> _data = new List<AssetUsedData>();

        public void AddData(string path)
        {
            path = "Assets" + path.Replace(Application.dataPath, "");
            path = path.Replace("\\", "/");
            _data.Add(new AssetUsedData(path));
        }
    }

    public class AssetUsedData
    {
        public AssetUsedData(string path)
        {
            _path = path;
        }

        public string _path;

        public Object _assetObj;

        // public void LoadAssetObj()
        // {
        //     if (_assetObj == null)
        //         _assetObj = AssetDatabase.LoadAssetAtPath<Object>(_path);
        // }
        public Object assetObj
        {
            get
            {
                if (_assetObj == null)
                    _assetObj = AssetDatabase.LoadAssetAtPath<Object>(_path);
                return _assetObj;
            }
        }

        public List<Object> _usedPrefab = new List<Object>();

        public void FindUsed(List<PrefabDependencies> prefabs)
        {
            _usedPrefab = new List<Object>();
            for (int i = 0; i < prefabs.Count; i++)
            {
                if (prefabs[i]._dependencies.Contains(_path))
                {
                    _usedPrefab.Add(AssetDatabase.LoadAssetAtPath<Object>(prefabs[i]._prefabPath));
                }
            }
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.ObjectField("被引用次数:"+_usedPrefab.Count+"/  查询的资源:", assetObj, typeof(GameObject), true);
            
            for (int i = 0; i < _usedPrefab.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField("被引用: ", _usedPrefab[i], typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndHorizontal();
        }
    }

    public class PrefabDependencies
    {
        public PrefabDependencies(string path)
        {
#if UNITY_EDITOR_WIN
            path = path.Replace("\\", "/");
#endif
            _prefabPath = "Assets" + path.Replace(Application.dataPath, "");
            _prefabPath = _prefabPath.Replace("\\", "/");
        }

        public string _prefabPath;
        public List<string> _dependencies;

        public void GetDependencies()
        {
            string[] paths = AssetDatabase.GetDependencies(new string[] { _prefabPath });
            _dependencies = new List<string>();
            for (int i = 0; i < paths.Length; i++)
            {
                _dependencies.Add(paths[i]);
            }
        }
    }
}

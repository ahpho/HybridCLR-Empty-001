/*
using HybridCLR;
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

/////////////////////////////////////////////////////////////////////////////////////////
// 参考：https://blog.csdn.net/Czhenya/article/details/135164154
/////////////////////////////////////////////////////////////////////////////////////////
public class LoadDll : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("main", LoadSceneMode.Single);
    }

    /*
    private Assembly m_HotUpdateAssembly = null;

    private static Dictionary<string, byte[]> ms_assetsData = new ();
    public static byte[] ReadBytesFromStreamingAssets(string assetName) {
        bool has = ms_assetsData.TryGetValue(assetName, out byte[] result);
        return (has && result != null) ? result : null;
    }
    public static void CleanUpAssetsData() {
        ms_assetsData.Clear();
        ms_assetsData = null;
    }

    private static List<string> AOTMetaAssemblyFiles { get; } = new List<string>() {
        "mscorlib.dll.bytes",
        "System.dll.bytes",
        "System.Core.dll.bytes",
    };

    void Start()
    {
        StartCoroutine(DownloadAssets(StartGame));
    }

    IEnumerator DownloadAssets(Action onDownloadComplete)
    {
        Debug.Log($"Application.dataPath={Application.dataPath}");
        Debug.Log($"Application.streamingAssetsPath={Application.streamingAssetsPath}");
        Debug.Log($"Application.persistentDataPath={Application.persistentDataPath}");

        Debug.Log($"to create1={Path.Combine(Application.persistentDataPath, "Res/AssemblyPak/")}");
        Debug.Log($"to create2={Path.Combine(Application.persistentDataPath, "Res/Assembly/")}");

        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Res/AssemblyPak/"));
        Directory.CreateDirectory(Path.Combine(Application.persistentDataPath, "Res/Assembly/"));

        Debug.Log($"Begin DownloadAssets().");

        var assets = new List<string> {
            "prefabs",
            "HotUpdate.dll.bytes",
        }.Concat(AOTMetaAssemblyFiles);

        foreach (string asset in assets)
        {
            string sourceDll = "", targetDll = "", targetPakDll = "";
#if UNITY_EDITOR
            continue;
#elif UNITY_ANDROID
            sourceDll = Path.Combine(Application.streamingAssetsPath, asset);
            targetDll = Path.Combine(Application.persistentDataPath, string.Format($"Res/Assembly/{asset}"));
            targetPakDll = Path.Combine(Application.persistentDataPath, string.Format($"Res/AssemblyPak/{asset}"));
#elif UNITY_IOS

#elif UNITY_STANDALONE_WIN

#endif
            if (!File.Exists(targetPakDll))
            {
                UnityWebRequest www = UnityWebRequest.Get(sourceDll);
                yield return www.SendWebRequest();
                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"Error={asset}, err={www.error}");
                    continue;
                }
                byte[] assetData = www.downloadHandler.data;
                File.WriteAllBytes(targetPakDll, assetData);
            }

            string dll = File.Exists(targetDll) ? targetDll : targetPakDll;
            byte[] dllBytes = File.ReadAllBytes(dll);
            ms_assetsData[asset] = dllBytes;
            Debug.Log($"Extracting={asset}, size={dllBytes.Length}");
        }

        onDownloadComplete();
    }

    private void StartGame()
    {
        InitHotUpdateDLLs();

        Run_InstantiateComponentByAsset();

        CleanUpAssetsData(); 
    }

    private void InitHotUpdateDLLs()
    {
        // 补充元数据
        LoadMetadataForAOTAssemblies();

        // Editor环境下，HotUpdate.dll.bytes已经被自动加载，不需要加载，重复加载反而会出问题。
#if !UNITY_EDITOR
        m_HotUpdateAssembly = Assembly.Load(ReadBytesFromStreamingAssets("HotUpdate.dll.bytes"));
#else
        m_HotUpdateAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate"); //Editor直接查找程序集
#endif

        // 使用热更的代码
        Type type = m_HotUpdateAssembly.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);

        // 释放内存。Demo这里晚点儿调，因为演示AB还要用
        //CleanUpAssetsData();
    }

    /// <summary>
    /// 为aot assembly加载原始metadata， 这个代码放aot或者热更新都行。
    /// 一旦加载后，如果AOT泛型函数对应native实现不存在，则自动替换为解释模式执行
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        // 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
        // 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            if (dllBytes == null)
            {
                Debug.LogError($"LoadMetadata read failed: {aotDllName}");
                continue;
            }
            // 加载assembly对应的dll，会自动为它hook。一旦aot泛型函数的native函数不存在，用解释器版本代码
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    private static void Run_InstantiateComponentByAsset()
    {
        // 通过实例化AssetbBundle中的资源，还原资源上的热更新脚本
        string abName = "prefabs";
        byte[] abBytes = ReadBytesFromStreamingAssets(abName);
        if (abBytes == null)
        {
            Debug.LogError($"Run_Instantiate, AB failed: {abName}");
            return;
        }
        AssetBundle ab = AssetBundle.LoadFromMemory(abBytes);
        GameObject cube = ab.LoadAsset<GameObject>("Cube");
        GameObject.Instantiate(cube);
    }
    */
}

using HybridCLR;
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

/////////////////////////////////////////////////////////////////////////////////////////
// �ο���https://blog.csdn.net/Czhenya/article/details/135164154
/////////////////////////////////////////////////////////////////////////////////////////
public class LoadDll : MonoBehaviour
{
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
        // ����Ԫ����
        LoadMetadataForAOTAssemblies();

        // Editor�����£�HotUpdate.dll.bytes�Ѿ����Զ����أ�����Ҫ���أ��ظ����ط���������⡣
#if !UNITY_EDITOR
        m_HotUpdateAssembly = Assembly.Load(ReadBytesFromStreamingAssets("HotUpdate.dll.bytes"));
#else
        m_HotUpdateAssembly = System.AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name == "HotUpdate"); //Editorֱ�Ӳ��ҳ���
#endif

        // ʹ���ȸ��Ĵ���
        Type type = m_HotUpdateAssembly.GetType("Hello");
        type.GetMethod("Run").Invoke(null, null);

        // �ͷ��ڴ档Demo��������������Ϊ��ʾAB��Ҫ��
        //CleanUpAssetsData();
    }

    /// <summary>
    /// Ϊaot assembly����ԭʼmetadata�� ��������aot�����ȸ��¶��С�
    /// һ�����غ����AOT���ͺ�����Ӧnativeʵ�ֲ����ڣ����Զ��滻Ϊ����ģʽִ��
    /// </summary>
    private static void LoadMetadataForAOTAssemblies()
    {
        // ע�⣬����Ԫ�����Ǹ�AOT dll����Ԫ���ݣ������Ǹ��ȸ���dll����Ԫ���ݡ�
        // �ȸ���dll��ȱԪ���ݣ�����Ҫ���䣬�������LoadMetadataForAOTAssembly�᷵�ش���
        HomologousImageMode mode = HomologousImageMode.SuperSet;
        foreach (var aotDllName in AOTMetaAssemblyFiles)
        {
            byte[] dllBytes = ReadBytesFromStreamingAssets(aotDllName);
            if (dllBytes == null)
            {
                Debug.LogError($"LoadMetadata read failed: {aotDllName}");
                continue;
            }
            // ����assembly��Ӧ��dll�����Զ�Ϊ��hook��һ��aot���ͺ�����native���������ڣ��ý������汾����
            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
        }
    }

    private static void Run_InstantiateComponentByAsset()
    {
        // ͨ��ʵ����AssetbBundle�е���Դ����ԭ��Դ�ϵ��ȸ��½ű�
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
}

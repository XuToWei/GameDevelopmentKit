#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using ThunderFireUITool;

public class ReferenceFinderData
{
    public string[] allAssets;

    //缓存路径
    private const string CACHE_PATH = "Library/ReferenceFinderCache-2019-10-09";
    //资源引用信息字典
    public Dictionary<string, AssetDescription> assetDict = new Dictionary<string, AssetDescription>();
    private const int MinThreadCount = 8;
    private const int SINGLE_THREAD_READ_COUNT = 100;
    private static int ThreadCount = Math.Max(MinThreadCount, Environment.ProcessorCount);
    private List<Thread> threadList = new List<Thread>();
    private int totalCount = 0;
    private int curReadAssetCount = 0;

    private static string basePath;
    private string settingIconPath;

    private List<Dictionary<string, AssetDescription>> threadAssetDict = new List<Dictionary<string, AssetDescription>>();

    //收集资源引用信息并更新缓存
    public void CollectDependenciesInfo()
    {
        try
        {
            basePath = Application.dataPath.Replace("/Assets", "");
            ReadFromCache();
            allAssets = AssetDatabase.GetAllAssetPaths();
            totalCount = allAssets.Length;
            threadList.Clear();
            curReadAssetCount = 0;

            foreach (var i in threadAssetDict)
            {
                i.Clear();
            }
            threadAssetDict.Clear();
            for (int i = 0; i < ThreadCount; i++)
            {
                threadAssetDict.Add(new Dictionary<string, AssetDescription>());
            }

            bool allThreadFinish = false;
            for (int i = 0; i < ThreadCount; i++)
            {
                ThreadStart method = () => ReadAssetInfo();
                Thread readThread = new Thread(method);
                threadList.Add(readThread);
                readThread.Start();
            }
            while (!allThreadFinish)
            {
                if ((curReadAssetCount % 500 == 0) && EditorUtility.DisplayCancelableProgressBar("更新中", string.Format("处理 {0}", curReadAssetCount), (float)curReadAssetCount / totalCount))
                {
                    EditorUtility.ClearProgressBar();
                    foreach (var i in threadList)
                    {
                        i.Abort();
                    }
                    return;
                }
                allThreadFinish = true;
                foreach (var i in threadList)
                {
                    if (i.IsAlive)
                    {
                        allThreadFinish = false;
                        break;
                    }
                }
            }
            foreach (var dict in threadAssetDict)
            {
                foreach (var j in dict)
                {
                    if (assetDict.ContainsKey(j.Key))
                    {
                        assetDict[j.Key] = j.Value;
                    }
                    else
                    {
                        assetDict.Add(j.Key, j.Value);
                    }
                }
            }
            //将信息写入缓存
            EditorUtility.DisplayCancelableProgressBar("更新中", "写入缓存", 1f);
            WriteToChache();
            //生成引用数据
            EditorUtility.DisplayCancelableProgressBar("更新中", "生成引用数据", 1f);
            UpdateReferenceInfo();
            EditorUtility.ClearProgressBar();
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }
    }

    public void ReadAssetInfo()
    {
        int index = Thread.CurrentThread.ManagedThreadId % ThreadCount;
        int intervalLength = totalCount / ThreadCount;
        int start = intervalLength * index;
        int end = start + intervalLength;
        if (totalCount - end < intervalLength)
        {
            end = totalCount;
        }
        int readAssetCount = 0;
        for (int i = start; i < end; i++)
        {
            if (readAssetCount % SINGLE_THREAD_READ_COUNT == 0)
            {
                curReadAssetCount += readAssetCount;
                readAssetCount = 0;
            }
            GetAsset(basePath, allAssets[i]);
            readAssetCount++;
        }
    }

    private static HashSet<string> fileExtension = new HashSet<string>()
    {
        ".prefab",
        ".unity",
        ".mat",
        ".asset",
        ".anim",
        ".controller",
    };

    // 注意 所有meta文件都要读
    private static Regex guidRegex = new Regex("guid: ([a-z0-9]{32})", RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
    public void GetAsset(string dataPath, string assetPath)
    {
        string extLowerStr = Path.GetExtension(assetPath).ToLower();
        bool needReadFile = fileExtension.Contains(extLowerStr);
        string fileName = $"{dataPath}/{assetPath}";
        string metaFile = $"{dataPath}/{assetPath}.meta";
        if (File.Exists(fileName) && File.Exists(metaFile))
        {
            //           
            string metaText = File.ReadAllText(metaFile, Encoding.UTF8);
            var matchRs = guidRegex.Matches(metaText);
            string selfGuid = matchRs[0].Groups[1].Value.ToLower();
            string lastModifyTime = File.GetLastWriteTime(fileName).ToString();
            MatchCollection guids = null;
            List<string> depend = new List<string>();
            if (needReadFile)
            {
                string fileStr;
                if (extLowerStr == ".prefab")
                {
                    fileStr = PrefabYamlUtils.FilterNestPrefabInstanceContent(fileName);
                }
                else
                {
                    fileStr = File.ReadAllText(fileName, Encoding.UTF8);
                }
                guids = guidRegex.Matches(fileStr);
            }
            int curListIndex = Thread.CurrentThread.ManagedThreadId % ThreadCount;
            var curDict = threadAssetDict[curListIndex];
            if (!curDict.ContainsKey(selfGuid) || curDict[selfGuid].assetDependencyHashString != lastModifyTime)
            {
                if (guids != null)
                {
                    foreach (Match i in guids)
                    {
                        depend.Add(i.Groups[1].Value.ToLower());
                    }
                }
                AssetDescription ad = new AssetDescription();
                ad.name = Path.GetFileNameWithoutExtension(assetPath);
                ad.path = assetPath;
                ad.assetDependencyHashString = lastModifyTime;
                ad.dependencies = depend;
                if (threadAssetDict[curListIndex].ContainsKey(selfGuid))
                {
                    threadAssetDict[curListIndex][selfGuid] = ad;
                }
                else
                {
                    threadAssetDict[curListIndex].Add(selfGuid, ad);
                }
            }
        }
    }

    //通过依赖信息更新引用信息
    private void UpdateReferenceInfo()
    {
        foreach (var asset in assetDict)
        {
            foreach (var assetGuid in asset.Value.dependencies)
            {
                if (assetDict.ContainsKey(assetGuid))
                {
                    assetDict[assetGuid].references.Add(asset.Key);
                }
            }
        }
    }

    //读取缓存信息
    public bool ReadFromCache()
    {
        assetDict.Clear();
        Texture2D[] t = PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Android);
        settingIconPath = AssetDatabase.GetAssetPath(t[0]);
        ClearCache();
        if (File.Exists(CACHE_PATH))
        {
            var serializedGuid = new List<string>();
            var serializedDependencyHash = new List<string>();
            var serializedDenpendencies = new List<int[]>();
            //反序列化数据
            using (FileStream fs = File.OpenRead(CACHE_PATH))
            {
                BinaryFormatter bf = new BinaryFormatter();
                if (EditorUtility.DisplayCancelableProgressBar("Import Cache", "Reading Cache", 0))
                {
                    EditorUtility.ClearProgressBar();
                    return false;
                }
                serializedGuid = (List<string>)bf.Deserialize(fs);
                serializedDependencyHash = (List<string>)bf.Deserialize(fs);
                serializedDenpendencies = (List<int[]>)bf.Deserialize(fs);
                EditorUtility.ClearProgressBar();
            }
            for (int i = 0; i < serializedGuid.Count; ++i)
            {
                string path = AssetDatabase.GUIDToAssetPath(serializedGuid[i]);
                if (!string.IsNullOrEmpty(path))
                {
                    var ad = new AssetDescription();
                    ad.name = Path.GetFileNameWithoutExtension(path);
                    ad.path = path;
                    ad.assetDependencyHashString = serializedDependencyHash[i];
                    assetDict.Add(serializedGuid[i], ad);
                }
            }
            for (int i = 0; i < serializedGuid.Count; ++i)
            {
                string guid = serializedGuid[i];
                if (assetDict.ContainsKey(guid))
                {
                    var guids = serializedDenpendencies[i].
                                Select(index => serializedGuid[index]).
                                Where(g => assetDict.ContainsKey(g)).
                                ToList();
                    assetDict[guid].dependencies = guids;
                }
            }
            UpdateReferenceInfo();
            return true;
        }
        return false;
    }

    //写入缓存
    private void WriteToChache()
    {
        if (File.Exists(CACHE_PATH))
        {
            File.Delete(CACHE_PATH);
        }
        //Debug.Log("write cache");
        var serializedGuid = new List<string>();
        var serializedDependencyHash = new List<string>();
        var serializedDenpendencies = new List<int[]>();
        //辅助映射字典
        var guidIndex = new Dictionary<string, int>();
        //序列化
        using (FileStream fs = File.OpenWrite(CACHE_PATH))
        {
            foreach (var pair in assetDict)
            {
                guidIndex.Add(pair.Key, guidIndex.Count);
                serializedGuid.Add(pair.Key);
                serializedDependencyHash.Add(pair.Value.assetDependencyHashString);
            }
            foreach (var guid in serializedGuid)
            {
                List<int> res = new List<int>();
                foreach (var i in assetDict[guid].dependencies)
                {
                    if (guidIndex.ContainsKey(i))
                    {
                        res.Add(guidIndex[i]);
                    }
                }
                int[] indexes = res.ToArray();
                serializedDenpendencies.Add(indexes);
            }
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, serializedGuid);
            bf.Serialize(fs, serializedDependencyHash);
            bf.Serialize(fs, serializedDenpendencies);
        }
    }

    //更新引用信息状态
    public void UpdateAssetState(string guid)
    {
        AssetDescription ad;
        if (assetDict.TryGetValue(guid, out ad) && ad.state != AssetState.NODATA)
        {
            if (File.Exists(ad.path))
            {
                //修改时间与记录的不同为修改过的资源
                if (ad.assetDependencyHashString != File.GetLastWriteTime(ad.path).ToString())
                {
                    ad.state = AssetState.CHANGED;
                }
                else
                {
                    //默认为普通资源
                    ad.state = AssetState.NORMAL;
                }
            }
            //不存在为丢失
            else
            {
                ad.state = AssetState.MISSING;
            }
        }
        //字典中没有该数据
        else if (!assetDict.TryGetValue(guid, out ad))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ad = new AssetDescription();
            ad.name = Path.GetFileNameWithoutExtension(path);
            ad.path = path;
            ad.state = AssetState.NODATA;
            assetDict.Add(guid, ad);
        }
    }

    //根据引用信息状态获取状态描述
    public static string GetInfoByState(AssetState state)
    {
        if (state == AssetState.CHANGED)
        {
            return "<color=red>与缓存不符</color>";
        }
        else if (state == AssetState.MISSING)
        {
            return "<color=red>丢失</color>";
        }
        else if (state == AssetState.NODATA)
        {
            return "<color=yellow>没缓存</color>";
        }
        return "<color=green>缓存正常</color>";
    }


    int GetRefCount(string assetGUID, AssetDescription desc, List<string> guidStack)
    {
        if (guidStack.Contains(assetGUID))
        {
            Debug.Log("有循环引用,计数可能不准");
            return 0;
        }
        guidStack.Add(assetGUID);
        var total = 0;
        if (assetDict.TryGetValue(assetGUID, out var value))
        {
            if (value.references.Count > 0)
            {
                Dictionary<string, int> cachedRefCount = new Dictionary<string, int>();
                foreach (var refs in value.references)
                {
                    if (!cachedRefCount.ContainsKey(refs))
                    {
                        int refCount = GetRefCount(refs, value, guidStack);
                        cachedRefCount[refs] = refCount;
                        total += refCount;
                    }
                }
            }
            else
            {
                total = 0;
                if (desc != null)
                {
                    var guid = AssetDatabase.AssetPathToGUID(desc.path);
                    foreach (var deps in value.dependencies)
                    {
                        if (guid == deps)
                            total++;
                    }
                }
            }
        }
        guidStack.RemoveAt(guidStack.Count - 1);
        return total;
    }

    private Dictionary<(AssetDescription, AssetDescription), int> dictCache =
        new Dictionary<(AssetDescription, AssetDescription), int>();

    public void ClearCache()
    {
        dictCache.Clear();
    }
    public int GetRefCount(AssetDescription desc, AssetDescription parentDesc)
    {
        var total = 0;
        if (dictCache.TryGetValue((desc, parentDesc), out total))
        {
            return total;
        }
        var rootGUID = AssetDatabase.AssetPathToGUID(desc.path);
        List<string> guidInStack = new List<string>();
        guidInStack.Add(rootGUID);

        Dictionary<string, int> cachedRefCount = new Dictionary<string, int>();//比较费
        foreach (var refs in desc.references)
        {
            if (!cachedRefCount.ContainsKey(refs))
            {
                int refCount = GetRefCount(refs, desc, guidInStack);
                cachedRefCount[refs] = refCount;
                total += refCount;
            }
        }
        if (desc.references.Count == 0 && parentDesc != null)
        {
            var guid = AssetDatabase.AssetPathToGUID(desc.path);
            foreach (var refs in parentDesc.references)
            {
                if (refs == guid)
                    total++;
            }
        }
        guidInStack.RemoveAt(guidInStack.Count - 1);
        //特判是不是ProjectSetting的icon
        if (!string.IsNullOrEmpty(settingIconPath))
        {
            int index = settingIconPath.LastIndexOf("_");
            string prefix = settingIconPath.Substring(0, index);
            if (desc.path.StartsWith(prefix))
            {
                total += 1;
            }
        }

        dictCache.Add((desc, parentDesc), total);
        return total;
    }

    public sealed class AssetDescription
    {
        public string name = "";
        public string path = "";
        public int count = 0;
        public string assetDependencyHashString;
        public List<string> dependencies = new List<string>();
        public List<string> references = new List<string>();
        public AssetState state = AssetState.NORMAL;
    }

    public enum AssetState
    {
        NORMAL,
        CHANGED,
        MISSING,
        NODATA,
    }
}
#endif
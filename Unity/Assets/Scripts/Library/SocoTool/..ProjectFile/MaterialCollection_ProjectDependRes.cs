using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Code.Editor.Res;
using Newtonsoft.Json.Utilities;
using UnityEditor;
using UnityEngine;

public sealed class MaterialCollection_ProjectDependRes : Soco.ShaderVariantsCollection.IMaterialCollector
{
    public override void AddMaterialBuildDependency(IList<Material> buildDependencyList)
    {
        //获取资源表中所有资源
        var resList = ResConfigFileEnter.GetConfigFile();

        int resIndex = 0;
        foreach (string res in resList)
        {
            EditorUtility.DisplayProgressBar("材质收集", $"正在收集直接资源",
                (float)resIndex++ / (float)resList.Count);
            //如果资源本身是材质，则直接添加到列表中
            if (res.EndsWith(".mat"))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(res);
                if(mat != null)
                    buildDependencyList.Add(mat);
            }
            //如果不是材质，则找到资源所引用的材质添加到列表中
            //这样会导致大量申请小数组的GC，因此用下面的API
            // else
            // {
            //     foreach (string depRes in AssetDatabase.GetDependencies(res))
            //     {
            //         if (depRes.EndsWith(".mat"))
            //         {
            //             Material mat = AssetDatabase.LoadAssetAtPath<Material>(depRes);
            //             if(mat != null)
            //                 buildDependencyList.Add(mat);
            //         }
            //     }
            // }
        }

        //获取资源间接引用的资源
        EditorUtility.DisplayProgressBar("材质收集", "正在获取间接资源，这可能需要一段时间", 0);
        resIndex = 0;
        var indirectDependRes = AssetDatabase.GetDependencies(resList.ToArray());
        
        foreach (string res in indirectDependRes)
        {
            EditorUtility.DisplayProgressBar("材质收集", $"正在收集间接依赖资源",
                (float)resIndex++ / (float)indirectDependRes.Length);
            if (res.EndsWith(".mat"))
            {
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(res);
                if(mat != null)
                    buildDependencyList.Add(mat);
            }
        }
        
        EditorUtility.ClearProgressBar();
    }
}
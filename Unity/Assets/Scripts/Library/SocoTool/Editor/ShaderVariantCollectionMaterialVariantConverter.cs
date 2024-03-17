using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Soco.ShaderVariantsCollection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class ShaderVariantCollectionMaterialVariantConverter
{
    //Key-材质
    //Value-材质被哪个收集器采集
    private Dictionary<Material, List<IMaterialCollector>> mMaterials = new Dictionary<Material, List<IMaterialCollector>>();
    
    //用于缓存Shader包含哪些keyword
    private Dictionary<Shader, string[]> mCachedKeywords = new Dictionary<Shader, string[]>();

    //存储已经添加的变体
    //列表存放的是，去重、排序后组合成的字符串，当字符串加入到列表中，代表这一keyword序列所有可能的变体都被加入到变体列表中
    private Dictionary<Shader, List<string>> mInsertedVariants = new Dictionary<Shader, List<string>>();

    private List<ShaderVariantCollection.ShaderVariant> mVariants = new List<ShaderVariantCollection.ShaderVariant>();
    
    //获取shader keyword方法，需要反射获取
    private static MethodInfo _GetShaderGlobalKeywordsMethod = null;
    private static MethodInfo _GetShaderLocalKeywordsMethod = null;

    private static MethodInfo GetShaderGlobalKeywordsMethod
    {
        get
        {
            if (_GetShaderGlobalKeywordsMethod == null)
                _GetShaderGlobalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);

            return _GetShaderGlobalKeywordsMethod;
        }
    }

    private static MethodInfo GetShaderLocalKeywordsMethod
    {
        get
        {
            if (_GetShaderLocalKeywordsMethod == null)
                _GetShaderLocalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);

            return _GetShaderLocalKeywordsMethod;
        }
    }

    private static string[] GetShaderKeywords(Shader shader)
    {
        string[] globalKeywords = GetShaderGlobalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
        string[] localKeywords = GetShaderLocalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];

        return globalKeywords.Concat(localKeywords).ToArray();
    }

    public bool CollectMaterial(IEnumerable<IMaterialCollector> collectors)
    {
        mMaterials.Clear();
        List<Material> materials = new List<Material>();

        bool success = false;
        
        try
        {
            int collectorIndex = 0;
            int collectorListCount = collectors.Count();
            foreach (IMaterialCollector collector in collectors)
            {
                EditorUtility.DisplayProgressBar("材质收集", $"正在处理第{collectorIndex + 1}/{collectorListCount}个 --{collector.name}",
                    (float)collectorIndex / (float)collectorListCount);
                materials.Clear();
                collector.AddMaterialBuildDependency(materials);
                foreach (Material material in materials)
                {
                    if (!mMaterials.TryGetValue(material, out List<IMaterialCollector> fromCollect))
                    {
                        fromCollect = new List<IMaterialCollector>();
                        mMaterials.Add(material, fromCollect);
                    }

                    fromCollect.Add(collector);
                }

                collectorIndex++;
            }
            
            Debug.Log($"[材质收集]共收集到{mMaterials}个材质");

            success = true;
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }

        return success;
    }

    public bool FilterMaterial(IEnumerable<IMaterialFilter> materialFilters)
    {
        var deleteList = mMaterials
            .Where(kv => materialFilters.Any(mf => !mf.Filter(kv.Key, kv.Value)))
            .Select(kv => kv.Key);

        foreach (var deleteMat in deleteList)
            mMaterials.Remove(deleteMat);

        return true;
    }
    
    public bool FilterMaterial(IEnumerable<IVariantFilter> variantFilters)
    {
        mVariants.RemoveAll(variant => variantFilters.Any(vf => !vf.Filter(variant)));
        return true;
    }

    //查看keyword是否属于shader，用于去除无效keyword
    private bool IsKeywordBelongToShader(Shader shader, string keyword)
    {
        if (!mCachedKeywords.TryGetValue(shader, out string[] keywords))
        {
            keywords = GetShaderKeywords(shader);
            mCachedKeywords.Add(shader, keywords);
        }

        return keywords.Contains(keyword);
    }

    private bool AddVariant(ShaderVariantCollection.ShaderVariant variant)
    {
        int findIndex = mVariants.FindIndex(v =>
        {
            return v.shader == variant.shader
                   && v.passType == variant.passType
                   && v.keywords.SequenceEqual(variant.keywords);
        });

        if (findIndex < 0)
        {
            mVariants.Add(variant);
        }

        return findIndex < 0;
    }

    private bool IsValidVariant(Shader shader, PassType passType, string[] keywords)
    {
        try
        {
            ShaderVariantCollection.ShaderVariant testVariant
                = new ShaderVariantCollection.ShaderVariant(shader, passType, keywords);

            return true;
        }
        catch
        {
            return false;
        }
    }
    
    private readonly string[] mSingleKeywords = new string[1];
    private bool IsKeywordInPass(Shader shader, PassType passType, string keyword)
    {
        mSingleKeywords[0] = keyword;
        return IsValidVariant(shader, passType, mSingleKeywords);
    }
    
    private readonly string[] mEmptyKeywords = new string[0];
    private bool IsPassExist(Shader shader, PassType passType)
    {
        return IsValidVariant(shader, passType, mEmptyKeywords);
    }


    private List<string> mTempInsertKeywords = new List<string>();
    private List<string> mTempSortedKeywords = new List<string>();

    private void AddVariantFromKeywords(Shader shader, List<string> validKeywords)
    {
        //如果keyword为空，直接尝试加入变体
        if (validKeywords.Count == 0)
        {
            //循环尝试所有Pass是否能加入
            for (PassType passType = (PassType)Enum.GetValues(typeof(PassType)).Cast<int>().Min();
                 passType <= (PassType)Enum.GetValues(typeof(PassType)).Cast<int>().Max();
                 ++passType)
            {
                if (!IsPassExist(shader, passType))
                    continue;

                AddVariant(new ShaderVariantCollection.ShaderVariant(shader, passType, mEmptyKeywords));
            }

            return;
        }

        
        foreach (string keyword in validKeywords)
        {
            for (PassType passType = (PassType)Enum.GetValues(typeof(PassType)).Cast<int>().Min();
                 passType <= (PassType)Enum.GetValues(typeof(PassType)).Cast<int>().Max();
                 ++passType)
            {
                //如果当前pass没有这个keyword，跳过
                if (!IsKeywordInPass(shader, passType, keyword))
                    continue;

                mTempInsertKeywords.Clear();
                mTempInsertKeywords.Add(keyword);

                //遍历其他keyword，逐个添加进组合，找到对于这个keyword的最长的有效组合
                foreach (string currentKeyword in validKeywords)
                {
                    if (currentKeyword == keyword)
                        continue;
                    
                    mTempInsertKeywords.Add(currentKeyword);
                    
                    //需要有序来检查Sequence Equal
                    mTempSortedKeywords.Clear();
                    mTempSortedKeywords.AddRange(mTempInsertKeywords);
                    mTempSortedKeywords.Sort();
                    
                    //如果加入当前keyword后，变体无效，则将当前keyword去掉
                    if (!IsValidVariant(shader, passType, mTempSortedKeywords.ToArray()))
                        mTempInsertKeywords.RemoveAt(mTempInsertKeywords.Count - 1);
                }

                mTempInsertKeywords.Sort();
                AddVariant(new ShaderVariantCollection.ShaderVariant(shader, passType, mTempInsertKeywords.ToArray()));
            }
        }
    }
    
    public bool CollectVariant()
    {
        mCachedKeywords.Clear();
        mInsertedVariants.Clear();
        mVariants.Clear();
        
        List<string> validKeywords = new List<string>();

        int materialIndex = 0;

        bool success = false;
        
        try
        {
            foreach (Material material in mMaterials.Keys)
            {
                EditorUtility.DisplayProgressBar("变体收集-材质与变体转换",
                    $"正在处理第{materialIndex}/{mMaterials.Keys.Count}个材质 --{material.name}",
                    (float)materialIndex / (float)mMaterials.Keys.Count);

                Shader shader = material.shader;

                validKeywords.Clear();
                validKeywords.AddRange(material.shaderKeywords.Distinct()); //去重
                validKeywords.RemoveAll(keyword => !IsKeywordBelongToShader(shader, keyword)); //去除没有的变体
                validKeywords.Sort();

                //检查keyword序列是否已添加，如果是则跳过
                string keywordsKey = string.Join(" ", validKeywords);
                if (mInsertedVariants.TryGetValue(shader, out List<string> collectedVariant))
                {
                    if (collectedVariant.Contains(keywordsKey))
                        continue;
                }
                else
                {
                    collectedVariant = new List<string>();
                    mInsertedVariants.Add(shader, collectedVariant);
                }

                AddVariantFromKeywords(shader, validKeywords);
                materialIndex++;
            }

            success = true;
        }
        catch (Exception e)
        {
            Debug.LogError(
                $"[材质变体转换]正在处理第{materialIndex}个材质{mMaterials.ElementAt(materialIndex).Key.name}\n报错信息为:{e.Message}\nStackTrace:{e.StackTrace}");
        }
        finally
        {
            EditorUtility.ClearProgressBar();
        }
        
        Debug.Log($"[材质变体转换]总共加入{mVariants.Count}个变体");
        return success;
    }

    public IEnumerable<Material> GetMaterials()
    {
        return mMaterials.Keys;
    }

    public int GetVariantCount()
    {
        return mVariants.Count;
    }

    public IEnumerable<IMaterialCollector> GetMaterialFrom(Material material)
    {
        if (mMaterials.TryGetValue(material, out var fromList))
        {
            return fromList;
        }

        return new IMaterialCollector[0];
    }

    public void WriteToShaderVariantCollectionFile(ShaderVariantCollection collection)
    {
        foreach (var variant in mVariants)
        {
            collection.Add(variant);
        }
    }
}

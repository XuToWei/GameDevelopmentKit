using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using System.Linq;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperCode : IPreprocessShaders
    {
        public int callbackOrder { get { return 0; } }

        private static string[] sAllPath = { "Assets", "Packages" };
        private static ShaderVariantsStripperConfig[] sConfigs;

        private List<(ConditionPair conditionPair, ShaderVariantsStripperConfig config)> mConditionList = new List<(ConditionPair condition, ShaderVariantsStripperConfig config)>();
        public static ShaderVariantsStripperConfig[] LoadConfigs()
        {
            string[] guids = AssetDatabase.FindAssets("t:ShaderVariantsStripperConfig", sAllPath);

            sConfigs = (from guid in guids
                    select AssetDatabase.LoadAssetAtPath<ShaderVariantsStripperConfig>(
                        AssetDatabase.GUIDToAssetPath(guid)))
                .ToArray();

            return sConfigs;
        }

        public static void ClearConfigs()
        {
            sConfigs = null;
        }
        
        public void OnProcessShader(Shader shader, ShaderSnippetData snippet, IList<ShaderCompilerData> data)
        {
            if(data.Count < 1)
                return;

            LoadConfigs();

            ShaderCompilerData workaround = data[0];

            int stripCount = 0;
            
            for (int i = data.Count - 1; i >= 0 ; --i)
            {
                mConditionList.Clear();

                StripVariant(shader, snippet, data[i], sConfigs, mConditionList);

                bool isStrip = false;
                foreach (var conditionPair_fromConfig in mConditionList)
                {
                    isStrip |= conditionPair_fromConfig.conditionPair.strip;
                    if (conditionPair_fromConfig.config.mIsWhiteList)
                    {
                        isStrip = false;
                        break;
                    }
                }
                
                if (isStrip)
                {
                    data.RemoveAt(i);
                    stripCount++;
                }
            }

            Debug.Log($"Shader:{shader.name} Pass:{snippet.passType} 剔除个数:{stripCount}");
            
            if (data.Count == 0)
            {
                Debug.Log($"Shader:{shader.name} Pass:{snippet.passType} 因剔除全部保留变体一个");
                data.Add(workaround);
            }
        }
        
        //对外开放接口，用于检查keyword是否需要被剔除
        private static List<(ConditionPair conditionPair, ShaderVariantsStripperConfig config)> sConditionList = new List<(ConditionPair condition, ShaderVariantsStripperConfig config)>();
        public static bool IsVariantStrip(Shader shader, ShaderSnippetData snippet, ShaderCompilerData data, ShaderVariantsStripperConfig[] configs)
        {
            sConditionList.Clear();
            StripVariant(shader, snippet, data, sConfigs, sConditionList);

            return sConditionList.Any(conditionPair_fromConfig =>
                       conditionPair_fromConfig.conditionPair.strip)
                   && sConditionList.All(conditionPair_fromConfig =>
                       !conditionPair_fromConfig.config.mIsWhiteList);
        }
        
        public static void StripVariant(Shader shader, ShaderSnippetData snippet, ShaderCompilerData data,
            ShaderVariantsStripperConfig[] configs,
            List<(ConditionPair conditionPair, ShaderVariantsStripperConfig config)> conditionList)
        {
            StripVariant(shader, ShaderVariantsData.GetShaderVariantsData(snippet, data), configs, conditionList);
        }
        
        public static void StripVariant(Shader shader, ShaderVariantsData variantData, ShaderVariantsStripperConfig[] configs, List<(ConditionPair conditionPair, ShaderVariantsStripperConfig config)> conditionList)
        {
            int FindConditionEqual(ConditionPair pair, out int index)
            {
                for (int condList_i = 0; condList_i < conditionList.Count; ++condList_i)
                {
                    if (pair.condition.EqualTo(conditionList[condList_i].conditionPair.condition, variantData))
                    {
                        index = condList_i;
                        return condList_i;
                    }
                }

                index = -1;
                return -1;
            }
                
            foreach (ShaderVariantsStripperConfig config in configs)
            {
                if (!config.mEnable)
                    continue;
                
                bool applyGlobalConfig = true;

                // 如果这个配置文件中能找到当前shader，则应用配置文件中“应用global config选项”
                if (config.mShaderConditions.TryGetValue(shader, out ShaderVariantsItem item))
                    applyGlobalConfig = item.applyGlobalConfig;
                // 如果Shader View中没有Shader，则Global Setting应用于全体Shader
                else if (config.mShaderConditions.Count == 0 && !config.mIsWhiteList)
                    applyGlobalConfig = true;
                else
                    applyGlobalConfig = false;
                    
                //Global condition
                if (applyGlobalConfig)
                {
                    foreach (ConditionPair pair in config.mGlobalConditions)
                    {
                        if (pair.condition.Completion(shader, variantData))
                        {
                            if (!config.mIsWhiteList)
                            {
                                //如果有相同的条件，
                                if (FindConditionEqual(pair, out int findIndex) != -1)
                                {
                                    //且优先级更高
                                    if(pair.priority > conditionList[findIndex].conditionPair.priority)
                                        conditionList[findIndex] = (pair, config);
                                    //优先级更低则直接丢弃
                                }
                                else//否则加入列表
                                    conditionList.Add((pair, config));
                            }
                            else//白名单变体无需判断优先级和相等条件
                            {
                                conditionList.Add((pair, config));
                            }
                        }
                    }
                }
                //Shader local condition
                if (item != null)
                {
                    foreach (ConditionPair pair in item.conditionPairs)
                    {
                        if (pair.condition.Completion(shader, variantData))
                        {
                            if (!config.mIsWhiteList)
                            {
                                if (FindConditionEqual(pair, out int findIndex) != -1)
                                {
                                    if (pair.priority > conditionList[findIndex].conditionPair.priority)
                                        conditionList[findIndex] = (pair, config);
                                }
                                else
                                    conditionList.Add((pair, config));
                            }
                            else
                            {
                                conditionList.Add((pair, config));
                            }
                        }
                    }
                }
            }
        }
    }
}


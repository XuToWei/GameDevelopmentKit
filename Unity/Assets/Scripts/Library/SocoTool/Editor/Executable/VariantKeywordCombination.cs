using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Soco.ShaderVariantsCollection
{
    [System.Serializable]
    public struct KeywordDeclareGroup
    {
        public List<string> keywords;
    }
    
    public class VariantKeywordCombination : IExecutable
    {
        public Shader mShader;
        public List<KeywordDeclareGroup> mKeywordDeclareGroups = new List<KeywordDeclareGroup>();
        
        public override void Execute(ShaderVariantCollectionMapper mapper)
        {
            if (mShader == null || mKeywordDeclareGroups.Count == 0)
            {
                Debug.LogError($"执行器{name}无法执行，原因是成员参数不符合规格，可能shader{mShader}为null，或keyword声明组长度为0(当前长度为{mKeywordDeclareGroups.Count})");
                return;
            }

            if (!mapper.HasShader(mShader))
            {
                Debug.LogError($"执行器{name}无法执行，原因是收集文件中不包含当前shader{mShader}");
                return;
            }
            
            var shaderVariants = mapper.GetShaderVariants(mShader);
            List<string[]> combinationList = new List<string[]>();
            combinationList.Add(new string[0]);

            foreach (KeywordDeclareGroup declareGroup in mKeywordDeclareGroups)
            {
                foreach (var keyword in declareGroup.keywords)
                {
                    if (keyword == "_")
                    {
                        continue;
                    }
                    else
                    {
                        int combinationLimit = combinationList.Count;
                        for (int i = 0; i < combinationLimit; ++i)
                        {
                            List<string> combination = combinationList[i].ToList();
                            if (!combination.Contains(keyword))
                                combination.Add(keyword);
                            
                            combinationList.Add(combination.ToArray());
                        }
                    }
                }
            }

            foreach (ShaderVariantCollection.ShaderVariant variant in shaderVariants.ToArray())
            {
                //去除已有变体中包含的multi_compile声明
                var removeMultiCompileKeywordList = variant.keywords.Where((string k) =>
                {
                    foreach (KeywordDeclareGroup declareGroup in mKeywordDeclareGroups)
                    {
                        foreach (var keyword in declareGroup.keywords)
                        {
                            if (k == keyword)
                                return false;
                        }
                    }
                    
                    return true;
                });
                
                foreach (string[] combination in combinationList)
                {
                    try
                    {
                        ShaderVariantCollection.ShaderVariant newVariant =
                            new ShaderVariantCollection.ShaderVariant(variant.shader, variant.passType,
                                removeMultiCompileKeywordList.Concat(combination).ToArray());

                        mapper.AddVariant(newVariant);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }
    }

    [ShaderVariantCollectionToolEditor(typeof(VariantKeywordCombination))]
    class VariantKeywordCombinationEditor : ShaderVariantCollectionToolEditor
    {
        private static MethodInfo _GetShaderGlobalKeywordsMethod = null;
        private static MethodInfo _GetShaderLocalKeywordsMethod = null;

        private static MethodInfo GetShaderGlobalKeywordsMethod
        {
            get
            {
                if(_GetShaderGlobalKeywordsMethod == null)
                    _GetShaderGlobalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords",
                        BindingFlags.NonPublic | BindingFlags.Static);

                return _GetShaderGlobalKeywordsMethod;
            }
        }
        
        private static MethodInfo GetShaderLocalKeywordsMethod
        {
            get
            {
                if(_GetShaderLocalKeywordsMethod == null)
                    _GetShaderLocalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords",
                        BindingFlags.NonPublic | BindingFlags.Static);

                return _GetShaderLocalKeywordsMethod;
            }
        }

        private string[] mShaderKeywords = new string[0];
        private int mSelectedKeywordIndex = 0;
        
        private enum KeywordMode
        {
            CustomKeyword,
            DefaultKeyword,
            DeclareStatement
        }

        private KeywordMode mKeywordMode = KeywordMode.CustomKeyword;
        private string mDeclareStatement = "";

        private const string cDeclarePattern = @"#pragma\s+(multi_compile(?:_\w+)?)+(?:\s+(\w+))*";
        
        public void OnEnable()
        {
            VariantKeywordCombination obj = target as VariantKeywordCombination;

            if (obj.mShader != null)
            {
                string[] globalKeywords = GetShaderGlobalKeywordsMethod.Invoke(null, new object[] { obj.mShader }) as string[];
                string[] localKeywords = GetShaderLocalKeywordsMethod.Invoke(null, new object[] { obj.mShader }) as string[];
                mShaderKeywords = globalKeywords.Concat(localKeywords).ToArray();
                Array.Sort(mShaderKeywords);
                mSelectedKeywordIndex = 0;
            }
        }

        public override void OnInspectorGUI()
        {
            VariantKeywordCombination obj = target as VariantKeywordCombination;
            
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();
            Shader newShader = EditorGUILayout.ObjectField("Shader", obj.mShader, typeof(Shader), true) as Shader;
            if (EditorGUI.EndChangeCheck() && newShader != obj.mShader)
            {
                Undo.RecordObject(obj, $"{obj.GetType().Name} change shader");
                
                obj.mShader = newShader;
                if (obj.mShader == null)
                    mShaderKeywords = new string[0];
                else
                {
                    string[] globalKeywords = GetShaderGlobalKeywordsMethod.Invoke(null, new object[] { obj.mShader }) as string[];
                    string[] localKeywords = GetShaderLocalKeywordsMethod.Invoke(null, new object[] { obj.mShader }) as string[];
                    mShaderKeywords = globalKeywords.Concat(localKeywords).ToArray();
                    Array.Sort(mShaderKeywords);
                }
                obj.mKeywordDeclareGroups.Clear();

                mSelectedKeywordIndex = 0;
            }

            if (obj.mShader != null && GUILayout.Button("尝试收集声明组"))
            {
                Undo.RecordObject(obj, $"{obj.GetType().Name} add  keyword throw declare statement");
                obj.mKeywordDeclareGroups.Clear();
                TryParseShader(obj.mShader, obj.mKeywordDeclareGroups);
                
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            mKeywordMode = (KeywordMode)EditorGUILayout.EnumPopup("选择模式", mKeywordMode);
            if (mKeywordMode == KeywordMode.CustomKeyword)
            {
                mSelectedKeywordIndex = EditorGUILayout.Popup(mSelectedKeywordIndex, mShaderKeywords);
            }
            else if (mKeywordMode == KeywordMode.DeclareStatement)
            {
                mDeclareStatement = EditorGUILayout.TextField("声明语句", mDeclareStatement);
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < obj.mKeywordDeclareGroups.Count; ++i)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < obj.mKeywordDeclareGroups[i].keywords.Count; ++j)
                {
                    string keyword = obj.mKeywordDeclareGroups[i].keywords[j];
                    if (GUILayout.Button(new GUIContent(keyword, keyword)))
                    {
                        Undo.RecordObject(obj, $"{obj.GetType().Name} delete declareGroup's keyword");
                        obj.mKeywordDeclareGroups[i].keywords.RemoveAt(j);
                    }
                }

                if (GUILayout.Button(new GUIContent("+", "将keyword添加到当前组合中"), GUILayout.Width(20)))
                {
                    if (mKeywordMode == KeywordMode.CustomKeyword)
                    {
                        if (!obj.mKeywordDeclareGroups[i].keywords.Contains(mShaderKeywords[mSelectedKeywordIndex]))
                        {
                            Undo.RecordObject(obj, $"{obj.GetType().Name} add keyword to declareGroup");
                            obj.mKeywordDeclareGroups[i].keywords.Add(mShaderKeywords[mSelectedKeywordIndex]);
                        }
                    }
                    else if(mKeywordMode == KeywordMode.DefaultKeyword)
                    {
                        if (!obj.mKeywordDeclareGroups[i].keywords.Contains("_"))
                        {
                            Undo.RecordObject(obj, $"{obj.GetType().Name} add default keyword to declareGroup");
                            obj.mKeywordDeclareGroups[i].keywords.Add("_");
                        }
                    }
                    else if (mKeywordMode == KeywordMode.DeclareStatement)
                    {
                        Undo.RecordObject(obj, $"{obj.GetType().Name} add  keyword throw declare statement");
                        foreach (string keyword in ParseDeclareStatement(mDeclareStatement))
                        {
                            if (!obj.mKeywordDeclareGroups[i].keywords.Contains(keyword))
                                obj.mKeywordDeclareGroups[i].keywords.Add(keyword);
                        }
                    }
                }

                if (GUILayout.Button(new GUIContent("-", "将当前变体组删除"), GUILayout.Width(20)))
                {
                    Undo.RecordObject(obj, $"{obj.GetType().Name} delete declareGroup");
                    obj.mKeywordDeclareGroups.RemoveAt(i);
                    EditorGUILayout.EndHorizontal();
                    break;
                }
                
                EditorGUILayout.EndHorizontal();
            }

            if (GUILayout.Button(new GUIContent("+", "添加变体声明组"), GUILayout.Width(20)))
            {
                Undo.RecordObject(obj, $"{obj.GetType().Name} add  keywords");
                if (mKeywordMode == KeywordMode.DeclareStatement)
                {
                    obj.mKeywordDeclareGroups.Add(new KeywordDeclareGroup(){keywords = ParseDeclareStatement(mDeclareStatement).ToList()});
                }
                else
                {
                    obj.mKeywordDeclareGroups.Add(new KeywordDeclareGroup(){keywords = new List<string>()});
                }
            }

            EditorGUILayout.EndVertical();
        }

        private void TryParseShader(Shader shader, List<KeywordDeclareGroup> targetList)
        {
            string path = AssetDatabase.GetAssetPath(shader);

            if (!File.Exists(path))
            {
                Debug.LogError($"{shader}所在路径{path}不存在，可能Shader是Unity内置Shader");
                return;
            }
            
            using (var sr = new StreamReader(path))
            {
                string content = sr.ReadToEnd();
                //正则前加行首和空字符，这样去除注释的声明
                MatchCollection matches = Regex.Matches(content, @"^\s+"+cDeclarePattern, RegexOptions.Multiline);

                foreach (Match match in matches)
                {
                    string[] declareGroup = ParseDeclareStatement(match.Groups[0].Value);
                    if (declareGroup.Length == 0)
                        continue;

                    bool hasDeclared = targetList.FindIndex(
                        kdg => kdg.keywords.OrderBy(k => k).SequenceEqual(declareGroup.OrderBy(k => k))) > 0;
                    
                    if (!hasDeclared)
                        targetList.Add(new KeywordDeclareGroup(){keywords = declareGroup.ToList()});
                }
            }
        }
        
        private string[] ParseDeclareStatement(string statement)
        {
            MatchCollection matches = Regex.Matches(statement, cDeclarePattern);
        
            if (matches.Count == 0)
            {
                Debug.LogError($"当前声明语句{statement}无法匹配出字段");
                return new string[0];
            }
            else
            {
                string multiCompileDeclare = matches[0].Groups[1].Value;
                multiCompileDeclare = multiCompileDeclare.Replace("_vertex", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_fragment", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_hull", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_domain", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_geometry", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_raytracing", "");
                multiCompileDeclare = multiCompileDeclare.Replace("_local", "");

                if (multiCompileDeclare == "multi_compile")
                {
                    List<string> keywords = new List<string>();
                    foreach (Capture capture in matches[0].Groups[2].Captures)
                        //如果是全下划线
                        if (capture.Value.All(cr=>cr=='_'))
                            keywords.Add("_");
                        else
                            keywords.Add(capture.Value);

                    if (keywords.Count == 1)
                    {
                        if (keywords[0] != "_")
                        {
                            keywords.Add("_");
                        }
                        else
                        {
                            Debug.LogError($"{statement}只声明了一个全下划线变体");
                            return new string[0];
                        }
                    }
                    
                    return keywords.ToArray();
                }
                else if (multiCompileDeclare == "multi_compile_instancing")
                {
                    return new string[] { "_", "INSTANCING_ON" };
                }
                else if (multiCompileDeclare == "multi_compile_particles")
                {
                    return new string[] { "_", "SOFTPARTICLES_ON" };
                }
                else if (multiCompileDeclare == "multi_compile_fog")
                {
                    return new string[] { "_", "FOG_LINEAR", "FOG_EXP", "FOG_EXP2" };
                }
                //TODO: Other BuildIn keyword declare, eg. multi_compile_fwdbase
                else
                {
                    Debug.LogError($"未实现当前{multiCompileDeclare}变体声明，请根据需要到VariantKeywordCombination中实现");
                    return new string[0];
                }
            }
            
        }
    }
}
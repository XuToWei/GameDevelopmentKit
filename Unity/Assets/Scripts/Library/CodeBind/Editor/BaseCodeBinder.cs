using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace CodeBind.Editor
{
    internal abstract class BaseCodeBinder
    {
        private readonly char m_SeparatorChar;

        private readonly string m_BindScriptFullPath;

        protected readonly string m_ScriptNameSpace;

        protected readonly string m_ScriptClassName;

        private readonly Transform m_RootTransform;

        protected readonly List<CodeBindData> m_BindDatas;
        protected readonly List<CodeBindData> m_BindArrayDatas;
        protected readonly SortedDictionary<string, List<CodeBindData>> m_BindArrayDataDict;

        private readonly Regex m_ArrayIndexRegex;
        private readonly Regex m_VariableNameRegex;
        
        protected BaseCodeBinder(MonoScript script, Transform rootTransform, char separatorChar)
        {
            if (!script.text.Contains("partial"))
            {
                throw new Exception($"please add key word 'partial' into {script.GetClass().FullName}!");
            }
            
            this.m_RootTransform = rootTransform;
            this.m_BindDatas = new List<CodeBindData>();
            this.m_BindArrayDatas = new List<CodeBindData>();
            this.m_BindArrayDataDict = new SortedDictionary<string, List<CodeBindData>>();
            string scriptFullPath = AssetDatabase.GetAssetPath(script);
            this.m_BindScriptFullPath = scriptFullPath.Insert(scriptFullPath.LastIndexOf('.'), ".Bind");
            this.m_ScriptNameSpace = script.GetClass().Namespace;
            this.m_ScriptClassName = script.GetClass().Name;
            this.m_SeparatorChar = separatorChar;
            this.m_ArrayIndexRegex = new Regex(@"(\d+)");
            this.m_VariableNameRegex = new Regex(@"^([A-Za-z0-9\._-]+/)*[A-Za-z0-9\._-]+$");
        }

        private bool TryGenerateNameMapTypeData()
        {
            bool TryGetBindComponents(Transform child, out List<CodeBindData> bindDatas)
            {
                bindDatas = new List<CodeBindData>();
                if (!child.name.Contains(m_SeparatorChar))
                {
                    return false;
                }
                string[] strArray = child.name.Split(this.m_SeparatorChar);
                string bindName = strArray[0];
                
                string lastStr = strArray[^1];
                MatchCollection matchCollection = this.m_ArrayIndexRegex.Matches(lastStr);
                if (matchCollection.Count > 0)
                {
                    return false;
                }
                
                Transform parent = child.parent;
                while (parent != null)
                {
                    bool canNext = false;
                    MonoBehaviour[] components = parent.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour component in components)
                    {
                        //检查父节点有没有bind，支持bind嵌套
                        if (component.GetType().GetCustomAttributes(typeof (CodeBindAttribute), true).Length > 0 && parent != this.m_RootTransform)
                        {
                            return false;
                        }
                        else
                        {
                            canNext = true;
                            break;
                        }
                    }
                    if (canNext)
                    {
                        break;
                    }
                    parent = parent.parent;
                }
                
                for (int i = 1; i < strArray.Length; i++)
                {
                    string typeStr = strArray[i];
                    if (string.Equals(typeStr, "*", StringComparison.OrdinalIgnoreCase))
                    {
                        //自动补齐所有存在的脚本
                        foreach (var kv in CodeBindNameTypeCollection.BindNameTypeDict)
                        {
                            if (child.GetComponent(kv.Value) != null)
                            {
                                CodeBindData bindData = new CodeBindData(bindName, kv.Value, kv.Key, child);
                                bindDatas.Add(bindData);
                            }
                        }
                    }
                    else if (CodeBindNameTypeCollection.BindNameTypeDict.TryGetValue(typeStr, out Type type) &&
                             child.GetComponent(type) != null)
                    {
                        CodeBindData bindData = new CodeBindData(bindName, type, typeStr, child);
                        bindDatas.Add(bindData);
                    }
                    else
                    {
                        throw new Exception($"{child.name}的命名中{typeStr}不存在对应的组件类型，绑定失败");
                    }
                }

                if (bindDatas.Count <= 0)
                {
                    throw new Exception("获取的Bind对象个数为0，绑定失败！");
                }
                return true;
            }
            
            bool TryGetBindArrayComponents(Transform child, out List<CodeBindData> bindDatas)
            {
                bindDatas = new List<CodeBindData>();
                if (!child.name.Contains(m_SeparatorChar))
                {
                    return false;
                }
                string[] strArray = child.name.Split(this.m_SeparatorChar);
                string bindName = strArray[0];
                
                string lastStr = strArray[^1];
                MatchCollection matchCollection = this.m_ArrayIndexRegex.Matches(lastStr);
                if (matchCollection.Count < 1)
                {
                    return false;
                }
                strArray[^1] = lastStr.Substring(0, lastStr.IndexOf(" (", StringComparison.Ordinal));
                
                Transform parent = child.parent;
                while (parent != null)
                {
                    bool canNext = false;
                    MonoBehaviour[] components = parent.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour component in components)
                    {
                        //检查父节点有没有bind，支持bind嵌套
                        if (component.GetType().GetCustomAttributes(typeof (CodeBindAttribute), true).Length > 0 && parent != this.m_RootTransform)
                        {
                            return false;
                        }
                        else
                        {
                            canNext = true;
                            break;
                        }
                    }
                    if (canNext)
                    {
                        break;
                    }
                    parent = parent.parent;
                }
                
                for (int i = 1; i < strArray.Length; i++)
                {
                    string typeStr = strArray[i];
                    if (string.Equals(typeStr, "*", StringComparison.OrdinalIgnoreCase))
                    {
                        //自动补齐所有存在的脚本
                        foreach (var kv in CodeBindNameTypeCollection.BindNameTypeDict)
                        {
                            if (child.GetComponent(kv.Value) != null)
                            {
                                CodeBindData bindData = new CodeBindData(bindName, kv.Value, kv.Key, child);
                                bindDatas.Add(bindData);
                            }
                        }
                    }
                    else if (CodeBindNameTypeCollection.BindNameTypeDict.TryGetValue(typeStr, out Type type) && child.GetComponent(type) != null)
                    {
                        CodeBindData bindData = new CodeBindData(bindName, type, typeStr, child);
                        bindDatas.Add(bindData);
                    }
                    else
                    {
                        throw new Exception($"{child.name}的命名中{typeStr}不存在对应的组件类型，绑定失败");
                    }
                }

                if (bindDatas.Count <= 0)
                {
                    throw new Exception("获取的Bind对象个数为0，绑定失败！");
                }
                return true;
            }
            
            this.m_BindDatas.Clear();
            this.m_BindArrayDatas.Clear();
            this.m_BindArrayDataDict.Clear();
            foreach (Transform child in this.m_RootTransform.GetComponentsInChildren<Transform>(true))
            {
                if(child == this.m_RootTransform)
                    continue;
                if (TryGetBindComponents(child, out List<CodeBindData> bindDatas))
                {
                    foreach (CodeBindData bindData in bindDatas)
                    {
                        if (this.m_BindDatas.Find(data => data.BindName == bindData.BindName && data.BindPrefix == bindData.BindPrefix) != null)
                        {
                            this.m_BindDatas.Clear();
                            throw new Exception($"绑定对象中存在同名{bindData.BindName}-{bindData.BindPrefix}-{bindData.BindTransform},请修改后重新生成。");
                        }
                        this.m_BindDatas.Add(bindData);
                    }
                }
                if (TryGetBindArrayComponents(child, out List<CodeBindData> bindArrayDatas))
                {
                    foreach (CodeBindData bindData in bindArrayDatas)
                    {
                        if (this.m_BindArrayDatas.Find(data => data.BindName == bindData.BindName && data.BindPrefix == bindData.BindPrefix && data.BindTransform == bindData.BindTransform) != null)
                        {
                            this.m_BindArrayDatas.Clear();
                            throw new Exception($"绑定数组对象中存在重复{bindData.BindName}-{bindData.BindPrefix}-{bindData.BindTransform},请修改后重新生成。");
                        }
                        this.m_BindArrayDatas.Add(bindData);
                    }
                }
            }
            if (this.m_BindDatas.Count < 1 && this.m_BindArrayDatas.Count < 1)
            {
                throw new Exception("绑定数量为0，生成失败。");
            }
            this.m_BindDatas.Sort((a, b) => String.CompareOrdinal(a.BindName + a.BindPrefix, b.BindName + b.BindPrefix));
            this.m_BindArrayDatas.Sort((a, b) => String.CompareOrdinal(a.BindName + a.BindPrefix, b.BindName + b.BindPrefix));
            for (int i = 0; i < this.m_BindArrayDatas.Count - 1; i++)
            {
                CodeBindData firstBindData = this.m_BindArrayDatas[i];
                string arrayName = firstBindData.BindName + firstBindData.BindPrefix;
                if (this.m_BindArrayDataDict.TryGetValue(arrayName, out List<CodeBindData> bindDatas))
                {
                    continue;
                }
                bindDatas = new List<CodeBindData>() { firstBindData };
                this.m_BindArrayDataDict.Add(arrayName, bindDatas);
                for (int j = i + 1; j < this.m_BindArrayDatas.Count; j++)
                {
                    CodeBindData bindData = this.m_BindArrayDatas[j];
                    if (!string.Equals(bindData.BindName + bindData.BindPrefix, arrayName, StringComparison.Ordinal))
                    {
                        continue;
                    }
                    bindDatas.Add(bindData);
                }
            }
            return true;
        }

        private void AutoFixChildBindName()
        {
            Dictionary<Transform, string> transformNameDict = new Dictionary<Transform, string>();
            Dictionary<string, List<Transform>> arrayTransformDict = new Dictionary<string, List<Transform>>();
            foreach (Transform child in this.m_RootTransform.GetComponentsInChildren<Transform>(true))
            {
                if(child == this.m_RootTransform)
                    continue;
                if (child.name.Contains(this.m_SeparatorChar))
                {
                    List<string> strList = child.name.Split(this.m_SeparatorChar).ToList();
                    if(string.IsNullOrEmpty(strList[0]))
                    {
                        throw new Exception($"变量名为空：{child.name}");
                    }
                    if (!this.m_VariableNameRegex.IsMatch(strList[0]))
                    {
                        throw new Exception($"{child.name}的变量名格式不对：{strList[0]}");
                    }
                    
                    //(xxx)结尾的识别为数组，方便复制
                    string lastStr = strList[^1];
                    MatchCollection matchCollection = this.m_ArrayIndexRegex.Matches(lastStr);
                    if (matchCollection.Count > 0)
                    {
                        if (arrayTransformDict.TryGetValue(strList[0], out List<Transform> transforms))
                        {
                            transforms.Add(child);
                        }
                        else
                        {
                            arrayTransformDict[strList[0]] = new List<Transform>() { child };
                        }
                        for (int i = 0; i < matchCollection.Count; i++)
                        {
                            lastStr = lastStr.Replace($"({matchCollection[i].Value})", string.Empty);
                        }
                        strList[^1] = lastStr.Replace(" ", string.Empty);
                    }
                    
                    bool hasAll = false;
                    for (int i = 1; i < strList.Count; i++)
                    {
                        if (string.IsNullOrEmpty(strList[i]))
                        {
                            throw new Exception($"不支持自动补齐名字为空的脚本：{child.name}");
                        }
                        if (string.Equals(strList[1], "*", StringComparison.OrdinalIgnoreCase))
                        {
                            hasAll = true;
                        }
                    }

                    if (hasAll)
                    {
                        List<string> newStrList = new List<string>();
                        newStrList.Add(strList[0]);
                        newStrList.Add("*");
                        strList = newStrList;
                    }
                    else
                    {
                        //自动补齐名字残缺的
                        for (int i = 1; i < strList.Count; i++)
                        {
                            string typeStr = strList[i];
                        
                            foreach (var kv in CodeBindNameTypeCollection.BindNameTypeDict)
                            {
                                if ((kv.Key.Contains(typeStr, StringComparison.OrdinalIgnoreCase) || typeStr.Contains(kv.Key, StringComparison.OrdinalIgnoreCase)) && child.GetComponent(kv.Value) != null)
                                {
                                    strList[i] = kv.Key;
                                    break;
                                }
                            }
                        }
                    }
                    
                    for (int i = 1; i < strList.Count; i++)
                    {
                        for (int j = i + 1; j < strList.Count; j++)
                        {
                            if (string.Equals(strList[i], strList[j], StringComparison.OrdinalIgnoreCase))
                            {
                                throw new Exception($"Child:{child} component name is repeated or auto fix repeated!");
                            }
                        }
                    }
                    
                    transformNameDict.Add(child, string.Join(m_SeparatorChar, strList));
                }
            }
            //处理Array
            foreach (KeyValuePair<string, List<Transform>> kv in arrayTransformDict)
            {
                if (kv.Value.Count < 2)
                {
                    continue;
                }
                Transform first = kv.Value[0];
                string firstName = transformNameDict[first];
                for (int i = 1; i < kv.Value.Count; i++)
                {
                    if (transformNameDict[kv.Value[i]] != firstName)
                    {
                        throw new Exception($"Child:{kv.Value[i]} has different component ({transformNameDict[kv.Value[i]]}:{firstName}) in array!");
                    }
                }
                transformNameDict[first] = $"{firstName} ({0})";
                for (int i = 1; i < kv.Value.Count; i++)
                {
                    string name = transformNameDict[kv.Value[i]];
                    transformNameDict[kv.Value[i]] = $"{name} ({i})";
                }
            }

            foreach (KeyValuePair<Transform, string> kv in transformNameDict)
            {
                kv.Key.name = kv.Value;
            }
        }

        public void TryGenerateBindCode()
        {
            this.AutoFixChildBindName();
            if (!TryGenerateNameMapTypeData())
            {
                return;
            }
            string codeStr = GetGeneratorCode().Replace("\t", "    ");
            if (File.Exists(this.m_BindScriptFullPath) && string.Equals(codeStr, File.ReadAllText(this.m_BindScriptFullPath)))
            {
                Debug.Log("文件内容相同。不需要重新生成。");
                return;
            }
            using StreamWriter sw = new StreamWriter(this.m_BindScriptFullPath);
            sw.Write(codeStr);
            sw.Close();
            AssetDatabase.ImportAsset(this.m_BindScriptFullPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log($"代码生成成功,生成路径: {this.m_BindScriptFullPath}");
        }

        public void TrySetSerialization()
        {
            if (!TryGenerateNameMapTypeData())
            {
                return;
            }
            SetSerialization();
            EditorUtility.SetDirty(this.m_RootTransform);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        protected abstract string GetGeneratorCode();

        protected abstract void SetSerialization();
    }
}

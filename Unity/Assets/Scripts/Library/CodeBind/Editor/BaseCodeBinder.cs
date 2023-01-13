using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        //private readonly Component m_BindSelfMonoComponent;

        protected readonly List<CodeBindData> m_BindDatas;

        private readonly Dictionary<string, Type> m_BindNameTypeDict;

        protected BaseCodeBinder(MonoScript script, Transform rootTransform, char separatorChar)
        {
            if (!script.text.Contains("partial"))
            {
                throw new Exception($"please add key word 'partial' into {script.GetClass().FullName}!");
            }
            
            this.m_RootTransform = rootTransform;
            this.m_BindDatas = new List<CodeBindData>();
            this.m_BindNameTypeDict = new Dictionary<string, Type>();
            string scriptFullPath = AssetDatabase.GetAssetPath(script);
            this.m_BindScriptFullPath = scriptFullPath.Insert(scriptFullPath.LastIndexOf('.'), ".Bind");
            this.m_ScriptNameSpace = script.GetClass().Namespace;
            this.m_ScriptClassName = script.GetClass().Name;
            this.m_SeparatorChar = separatorChar;
            GetAllBindNameTypes();
        }

        private void GetAllBindNameTypes()
        {
            m_BindNameTypeDict.Clear();
            var types = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(x => x.GetTypes())
                    .Where(x => x.IsClass && !x.IsAbstract && typeof(ICodeBindNameTypeConfig).IsAssignableFrom(x));
            foreach (Type rType in types)
            {
                ICodeBindNameTypeConfig config = Activator.CreateInstance(rType) as ICodeBindNameTypeConfig;
                foreach (var kv in config.BindNameTypeDict)
                {
                    if (kv.Value == null || !kv.Value.IsSubclassOf(typeof(Component)))
                        throw new Exception($"Type:{kv.Value} error! Only can bind sub class of 'Component'!");
                    
                    if (this.m_BindNameTypeDict.TryGetValue(kv.Key, out Type type))
                    {
                        throw new Exception($"Type name:{kv.Key}({type}) exist!");
                    }
                
                    this.m_BindNameTypeDict.Add(kv.Key, kv.Value);
                }
            }
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
                Transform parent = child.parent;
               
                while (parent != null)
                {
                    bool canNext = false;
                    MonoBehaviour[] components = parent.GetComponents<MonoBehaviour>();
                    foreach (MonoBehaviour component in components)
                    {
                        //检查父节点有没有bind，支持bind嵌套
                        if (component.GetType().GetCustomAttributes(typeof (BaseCodeBindAttribute), true).Length > 0 && parent != this.m_RootTransform)
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

                string[] strArray = child.name.Split(this.m_SeparatorChar);
                string bindName = strArray[0];
                for (int i = 1; i < strArray.Length; i++)
                {
                    string typeStr = strArray[i];
                    if (this.m_BindNameTypeDict.TryGetValue(typeStr, out Type type))
                    {
                        CodeBindData bindData = new CodeBindData(bindName, type, typeStr, child);
                        bindDatas.Add(bindData);
                        continue;
                    }
                    throw new Exception($"{child.name}的命名中{typeStr}不存在对应的组件类型，绑定失败");
                }

                if (bindDatas.Count <= 0)
                {
                    throw new Exception($"获取的Bind对象个数为0，绑定失败！");
                }
                return true;
            }
            
            this.m_BindDatas.Clear();
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
                            throw new Exception($"绑定对象中存在同名{bindData.BindName}-{bindData.BindPrefix},请修改后重新生成。");
                        }
                        this.m_BindDatas.Add(bindData);
                    }
                }
            }
            if (this.m_BindDatas.Count < 1)
            {
                throw new Exception($"绑定数量为0，生成失败。");
            }
            return true;
        }

        private void AutoFixChildBindName()
        {
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

                    if (strList.Count == 2 && (string.IsNullOrEmpty(strList[1]) || string.Equals(strList[1], "*", StringComparison.OrdinalIgnoreCase)))
                    {
                        strList.RemoveAt(1);
                        //自动补齐所有存在的脚本
                        foreach (var kv in this.m_BindNameTypeDict)
                        {
                            if (child.GetComponent(kv.Value) != null)
                            {
                                strList.Add(kv.Key);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 1; i < strList.Count; i++)
                        {
                            if (string.IsNullOrEmpty(strList[i]))
                            {
                                throw new Exception($"不支持自动补齐名字为空的脚本：{child.name}");
                            }
                        }
                        
                        //自动补齐名字残缺的
                        for (int i = 1; i < strList.Count; i++)
                        {
                            string typeStr = strList[i];
                        
                            foreach (var kv in this.m_BindNameTypeDict)
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
                    
                    child.name = string.Join(m_SeparatorChar, strList);
                }
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

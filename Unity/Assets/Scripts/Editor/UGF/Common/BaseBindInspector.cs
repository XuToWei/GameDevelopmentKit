using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityGameFramework.Editor;
using Type = System.Type;

namespace UGF.Editor
{
    public abstract class BaseBindInspector : GameFrameworkInspector
    {
        protected abstract List<(string, Type)> DefaultBindTypeList
        {
            get;
        }
        
        /// <summary>
        /// bind脚本（这里父节点不会绑定到子节点）,越后面的自动补全名字时候优先级越高
        /// </summary>
        protected abstract List<(string, Type)> CustomBindTypeList
        {
            get;
        }

        private class BindData
        {
            public string BindName
            {
                get;
            }

            public Type BindType
            {
                get;
            }

            public string BindPrefix
            {
                get;
            }

            public Transform BindTransform
            {
                get;
            }

            public BindData(string bindName, Type bindType, string bindPrefix, Transform bindTransform)
            {
                this.BindName = bindName;
                this.BindType = bindType;
                this.BindPrefix = bindPrefix;
                this.BindTransform = bindTransform;
            }
            
            
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("生成绑定代码"))
            {
                this.AutoFixChildBindName();
                this.TryGenerateBindCode();
            }
            if (GUILayout.Button("设置绑定数据"))
            {
                this.SetGeneratorSerializeField();
            }
            this.DrawDefaultInspector();
            serializedObject.Update();
        }

        private string ScriptFullPath
        {
            get
            {
                MonoScript monoScript = MonoScript.FromMonoBehaviour(this.target as MonoBehaviour);
                return AssetDatabase.GetAssetPath(monoScript);
            }
        }

        private string BindScriptFullPath
        {
            get
            {
                string scriptFullPath = ScriptFullPath;
                int lastIndexOf = scriptFullPath.LastIndexOf('.');
                return scriptFullPath.Insert(lastIndexOf, ".Bind");
            }
        }

        private string ScriptNameSpace
        {
            get
            {
                MonoBehaviour mono = this.target as MonoBehaviour;
                Type type = mono.GetType();
                return type.Namespace;
            }
        }

        private string ScriptClassName
        {
            get
            {
                MonoBehaviour mono = this.target as MonoBehaviour;
                Type type = mono.GetType();
                return type.Name;
            }
        }

        private Transform rootTransform
        {
            get
            {
                MonoBehaviour mono = this.target as MonoBehaviour;
                return mono.transform;
            }
        }

        private readonly List<BindData> m_BindDatas = new();

        private bool TryGenerateNameMapTypeData()
        {
            bool TryGetBindComponents(Transform child, out List<BindData> bindDatas)
            {
                bindDatas = new List<BindData>();
                if (!child.name.Contains('_'))
                {
                    return false;
                }
                Transform parent = child.parent;
                while (parent != null)
                {
                    bool canNext = false;
                    foreach (var pair in this.CustomBindTypeList)
                    {
                        Component component = parent.GetComponent(pair.Item2);
                        if (component == null)
                        {
                            continue;
                        }
                        if (component != this.target)
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

                string[] strArray = child.name.Split('_');
                string bindName = strArray[0];
                for (int i = 1; i < strArray.Length; i++)
                {
                    string typeStr = strArray[i];
                    (string, Type) value = this.DefaultBindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                    if (value.Item2 != null)
                    {
                        BindData bindData = new BindData(bindName, value.Item2, value.Item1, child);
                        bindDatas.Add(bindData);
                        continue;
                    }
                    value = this.CustomBindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                    if (value.Item2 != null)
                    {
                        BindData bindData = new BindData(bindName, value.Item2, value.Item1, child);
                        bindDatas.Add(bindData);
                        continue;
                    }
                    throw new Exception($"{child.name}的命名中{typeStr}不存在对应的组件类型，绑定失败");
                }

                return bindDatas.Count > 0;
            }
            
            this.m_BindDatas.Clear();
            foreach (Transform child in rootTransform.GetComponentsInChildren<Transform>(true))
            {
                if(child == this.rootTransform)
                    continue;
                if (TryGetBindComponents(child, out List<BindData> bindDatas))
                {
                    foreach (BindData bindData in bindDatas)
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
            foreach (Transform child in rootTransform.GetComponentsInChildren<Transform>(true))
            {
                if (child.name.Contains('_'))
                {
                    List<string> strList = child.name.Split('_').ToList();
                    if (strList.Count >= 3)
                    {
                        for (int i = 1; i < strList.Count - 1; i++)
                        {
                            if (string.IsNullOrEmpty(strList[i]))
                            {
                                throw new Exception($"不支持自动补齐超过数量为1的脚本：{child.name}");
                            }
                        }
                    }
                    
                    if (string.IsNullOrEmpty(strList[^1]))
                    {
                        bool TryFixEmptyBindName(List<(string, Type)> bindTypeList)
                        {
                            for (int i = bindTypeList.Count - 1; i >= 0; i--)
                            {
                                (string, Type) value = bindTypeList[i];
                                bool isExist = false;
                                for (int j = 0; j < strList.Count - 1; j++)
                                {
                                    string typeSrt = strList[j];
                                    if (typeSrt == value.Item1)
                                    {
                                        isExist = true;
                                        break;
                                    }
                                }

                                if (!isExist)
                                {
                                    if (child.GetComponent(value.Item2) != null)
                                    {
                                        strList[^1] = value.Item1;
                                        return true;
                                    }
                                }
                            }
                            return false;
                        }
                        
                        if (!TryFixEmptyBindName(this.CustomBindTypeList))
                        {
                            TryFixEmptyBindName(this.DefaultBindTypeList);
                        }
                    }
                    
                    for (int i = 1; i < strList.Count - 1; i++)
                    {
                        string typeStr = strList[i];
                        
                        bool TryFixErrorBindName(List<(string, Type)> bindTypeList)
                        {
                            (string, Type) value = bindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                            if (value.Item2 == null || child.GetComponent(value.Item2) == null)
                            {
                                for (int j = bindTypeList.Count - 1; j >= 0; j--)
                                {
                                    value = bindTypeList[j];
                                    if ((value.Item1.Contains(typeStr, StringComparison.OrdinalIgnoreCase) || typeStr.Contains(value.Item1, StringComparison.OrdinalIgnoreCase))
                                        && child.GetComponent(value.Item2) != null)
                                    {
                                        strList[i] = value.Item1;
                                        return true;
                                    }
                                }
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }
                        
                        if (!TryFixErrorBindName(this.CustomBindTypeList))
                        {
                            TryFixErrorBindName(this.DefaultBindTypeList);
                        }
                    }

                    child.name = string.Join('_', strList);
                }
            }
        }

        private void TryGenerateBindCode()
        {
            if (!TryGenerateNameMapTypeData())
            {
                return;
            }
            string codeStr = GetGeneratorCode().Replace("\t", "    ");
            if (File.Exists(this.BindScriptFullPath) && string.Equals(codeStr, File.ReadAllText(this.BindScriptFullPath)))
            {
                Debug.Log("文件内容相同。不需要重新生成。");
                return;
            }
            using StreamWriter sw = new StreamWriter(this.BindScriptFullPath);
            sw.Write(codeStr);
            sw.Close();
            AssetDatabase.ImportAsset(this.BindScriptFullPath);
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            Debug.Log($"代码生成成功,生成路径: {this.BindScriptFullPath}");
        }

        private string GetGeneratorCode()
        {
            StringBuilder stringBuilder = new StringBuilder(2048);
            string indentation = string.Empty;
            //命名空间
            string nameSpace = this.ScriptNameSpace;
            if (!string.IsNullOrEmpty(nameSpace))
            {
                stringBuilder.AppendLine($"namespace {nameSpace}");
                stringBuilder.AppendLine("{");
                indentation = "\t";
            }
            //类名
            stringBuilder.AppendLine($"{indentation}public partial class {this.ScriptClassName}");
            stringBuilder.AppendLine($"{indentation}{{");
            //组件字段
            foreach (BindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}\t[UnityEngine.SerializeField] private {bindData.BindType.FullName} m_{bindData.BindName}{bindData.BindPrefix};");
            }
            foreach (BindData bindData in this.m_BindDatas)
            {
                stringBuilder.AppendLine($"{indentation}\tpublic {bindData.BindType.FullName} {bindData.BindName}{bindData.BindPrefix} => m_{bindData.BindName}{bindData.BindPrefix};");
            }
            stringBuilder.AppendLine($"{indentation}}}");
            if (!string.IsNullOrEmpty(nameSpace))
            {
                stringBuilder.AppendLine("}");
            }
            return stringBuilder.ToString();
        }

        private void SetGeneratorSerializeField()
        {
            if (!TryGenerateNameMapTypeData())
            {
                return;
            }
            foreach (BindData bindData in m_BindDatas)
            {
                SerializedProperty serializedProperty = this.serializedObject.FindProperty($"m_{bindData.BindName}{bindData.BindPrefix}");
                serializedProperty.objectReferenceValue = bindData.BindTransform.GetComponent(bindData.BindType);
            }
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}

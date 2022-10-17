using System;
using System.Collections.Generic;
using System.IO;
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

        private readonly Dictionary<string, BindData> m_BindDataDict = new();

        private bool TryGenerateNameMapTypeData()
        {
            bool TryGetBindComponent(Transform child, out BindData bindData)
            {
                bindData = null;
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
                
                int lastIndex = child.name.LastIndexOf('_');
                string typeStr = child.name.Substring(lastIndex + 1, child.name.Length - lastIndex - 1);
                (string, Type) value = this.DefaultBindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                if (value.Item2 != null)
                {
                    bindData = new BindData(child.name.Substring(0, lastIndex), value.Item2, value.Item1, child);
                    return true;
                }
                value = this.CustomBindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                if (value.Item2 != null)
                {
                    bindData = new BindData(child.name.Substring(0, lastIndex), value.Item2, value.Item1, child);
                    return true;
                }
                throw new Exception($"{child.name}的命名中{typeStr}不存在对应的组件类型，绑定失败");
            }
            
            this.m_BindDataDict.Clear();
            foreach (Transform child in rootTransform.GetComponentsInChildren<Transform>(true))
            {
                if(child == this.rootTransform)
                    continue;
                if (TryGetBindComponent(child, out BindData bindData))
                {
                    if (this.m_BindDataDict.ContainsKey(bindData.BindName))
                    {
                        this.m_BindDataDict.Clear();
                        throw new Exception($"绑定对象中存在同名{bindData.BindName},请修改后重新生成。");
                    }
                    this.m_BindDataDict.Add(bindData.BindName, bindData);
                }
            }
            if (m_BindDataDict.Count < 1)
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
                    int lastIndex = child.name.LastIndexOf('_');
                    if (lastIndex >= child.name.Length - 1)
                    {
                        bool TryFixChildName(List<(string, Type)> bindTypeList)
                        {
                            for (int i = bindTypeList.Count - 1; i >= 0; i--)
                            {
                                (string, Type) value = bindTypeList[i];
                                if (child.GetComponent(value.Item2) != null)
                                {
                                    child.name = $"{child.name}{value.Item1}";
                                    return true;
                                }
                            }
                            return false;
                        }
                        
                        if (!TryFixChildName(this.CustomBindTypeList))
                        {
                            TryFixChildName(this.DefaultBindTypeList);
                        }
                    }
                    else
                    {
                        string typeStr = child.name.Substring(lastIndex + 1, child.name.Length - lastIndex - 1);

                        bool TryFixChildName(List<(string, Type)> bindTypeList)
                        {
                            (string, Type) value = bindTypeList.Find(p => p.Item1.Equals(typeStr, StringComparison.OrdinalIgnoreCase));
                            if (value.Item2 == null || child.GetComponent(value.Item2) == null)
                            {
                                for (int i = bindTypeList.Count - 1; i >= 0; i--)
                                {
                                    value = bindTypeList[i];
                                    if ((value.Item1.Contains(typeStr, StringComparison.OrdinalIgnoreCase) || typeStr.Contains(value.Item1, StringComparison.OrdinalIgnoreCase))
                                        && child.GetComponent(value.Item2) != null)
                                    {
                                        child.name = $"{child.name.Substring(0, lastIndex + 1)}{value.Item1}";
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

                        if (!TryFixChildName(this.CustomBindTypeList))
                        {
                            TryFixChildName(this.DefaultBindTypeList);
                        }
                    }
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
            foreach (var pair in this.m_BindDataDict)
            {
                stringBuilder.AppendLine($"{indentation}\t[UnityEngine.SerializeField] private {pair.Value.BindType.FullName} m_{pair.Key}{pair.Value.BindPrefix};");
            }
            foreach (var pair in this.m_BindDataDict)
            {
                stringBuilder.AppendLine($"{indentation}\tpublic {pair.Value.BindType.FullName} {pair.Key}{pair.Value.BindPrefix} => m_{pair.Key}{pair.Value.BindPrefix};");
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
            foreach (var pair in m_BindDataDict)
            {
                SerializedProperty serializedProperty = this.serializedObject.FindProperty($"m_{pair.Key}{pair.Value.BindPrefix}");
                BindData bindData = pair.Value;
                serializedProperty.objectReferenceValue = bindData.BindTransform.GetComponent(bindData.BindType);
            }
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}

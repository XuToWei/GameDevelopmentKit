using System;
using System.Collections.Generic;
using System.Linq;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    [CustomEditor(typeof(ReferenceBindComponent))]
    public class ReferenceBindComponentEditor : UnityEditor.Editor
    {
        private enum InitError
        {
            None,
            NotExistSettingData,
            Other
        }

        private ReferenceBindComponent m_Target;
        private Page m_Page;
        private SerializedProperty m_Searchable;
        private CodeGeneratorSettingConfig m_CodeGeneratorSettingConfig;
        private bool m_SettingDataExpanded = true;
        private int m_LastSettingDataNameIndex;
        private bool m_SettingDataError;

        private RuleHelperInfo<IBindComponentsRuleHelper> m_BindComponentsRuleHelperInfo;
        private RuleHelperInfo<IBindAssetOrPrefabRuleHelper> m_BindAssetOrPrefabRuleHelperInfo;
        private RuleHelperInfo<ICodeGeneratorRuleHelper> m_CodeGeneratorRuleHelperInfo;

        // private bool m_IsInitError = false;
        private InitError m_InitError = InitError.None;

        private void OnEnable()
        {
            try
            {
                m_Target = (ReferenceBindComponent)target;
                m_Page = new Page(10, m_Target.GetAllBindObjectsCount());
                if (!CheckCodeGeneratorSettingData())
                {
                    m_InitError = InitError.NotExistSettingData;
                }

                if (m_InitError != InitError.NotExistSettingData)
                {
                    InitSearchable();
                }
                InitHelperInfos();
                m_Target.SetClassName(string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                    ? m_Target.gameObject.name
                    : m_Target.GeneratorCodeName);
                serializedObject.ApplyModifiedProperties();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_InitError = InitError.Other;
            }
        }


        public override void OnInspectorGUI()
        {
            if (m_InitError == InitError.NotExistSettingData)
            {
                if (CheckCodeGeneratorSettingData(false))
                {
                    m_InitError = InitError.None;
                    InitSearchable();
                }
            }
            
            if (m_InitError != InitError.None)
            { 
                return;
            }

            if (m_IsCompiling && !EditorApplication.isCompiling)
            {
                m_IsCompiling = false;
                OnCompileComplete();
            }
            else if (!m_IsCompiling && EditorApplication.isCompiling)
            {
                m_IsCompiling = true;
            }

            serializedObject.Update();
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                DrawTopButton();
                EditorGUILayout.Space();
                DrawHelperSelect();
                EditorGUILayout.Space();
                DrawBindAssetOrPrefab();
                EditorGUILayout.Space();
                DrawSetting();
                EditorGUILayout.Space();
                DrawBindObjects();
                m_Page.SetAllCount(m_Target.GetAllBindObjectsCount());
                m_Page.Draw();
            }

            serializedObject.ApplyModifiedProperties();
        }

        #region 规则帮助类

        private bool m_IsCompiling = false;

        private void InitHelperInfos()
        {
            m_Target.SetBindAssetOrPrefabRuleHelperTypeName(
                string.IsNullOrEmpty(m_Target.BindAssetOrPrefabRuleHelperTypeName)
                    ? typeof(DefaultBindAssetOrPrefabRuleHelper).FullName
                    : m_Target.BindAssetOrPrefabRuleHelperTypeName);

            m_BindAssetOrPrefabRuleHelperInfo =
                new RuleHelperInfo<IBindAssetOrPrefabRuleHelper>("m_BindAssetOrPrefabRule", null);

            m_BindAssetOrPrefabRuleHelperInfo.Init(m_Target.BindAssetOrPrefabRuleHelperTypeName, typeName =>
            {
                m_Target.SetBindAssetOrPrefabRuleHelperTypeName(typeName);
                return m_Target.BindAssetOrPrefabRuleHelperTypeName;
            });

            m_Target.SetBindComponentsRuleHelperTypeName(string.IsNullOrEmpty(m_Target.BindComponentsRuleHelperTypeName)
                ? typeof(DefaultBindComponentsRuleHelper).FullName
                : m_Target.BindComponentsRuleHelperTypeName);

            m_BindComponentsRuleHelperInfo = new RuleHelperInfo<IBindComponentsRuleHelper>("m_BindComponentsRule", null);

            m_BindComponentsRuleHelperInfo.Init(m_Target.BindComponentsRuleHelperTypeName, typeName =>
            {
                m_Target.SetBindComponentsRuleHelperTypeName(typeName);
                return m_Target.BindComponentsRuleHelperTypeName;
            });
            m_Target.SetCodeGeneratorRuleHelperTypeName(string.IsNullOrEmpty(m_Target.CodeGeneratorRuleHelperTypeName)
                ? typeof(DefaultCodeGeneratorRuleHelper).FullName
                : m_Target.CodeGeneratorRuleHelperTypeName);

            m_CodeGeneratorRuleHelperInfo = new RuleHelperInfo<ICodeGeneratorRuleHelper>("m_CodeGeneratorRule", new[]
            {
                typeof(TransformFindCodeGeneratorRuleHelper).FullName
            });

            m_CodeGeneratorRuleHelperInfo.Init(m_Target.CodeGeneratorRuleHelperTypeName, typeName =>
            {
                m_Target.SetCodeGeneratorRuleHelperTypeName(typeName);
                return m_Target.CodeGeneratorRuleHelperTypeName;
            });

            RefreshHelperTypeNames();
        }

        void OnCompileComplete()
        {
            RefreshHelperTypeNames();
        }

        void RefreshHelperTypeNames()
        {
            m_BindAssetOrPrefabRuleHelperInfo.Refresh();
            m_BindComponentsRuleHelperInfo.Refresh();
            m_CodeGeneratorRuleHelperInfo.Refresh();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制辅助器选择框
        /// </summary>
        private void DrawHelperSelect()
        {
            m_BindAssetOrPrefabRuleHelperInfo.Draw();
            m_BindComponentsRuleHelperInfo.Draw();
            m_CodeGeneratorRuleHelperInfo.Draw();
        }

        #endregion

        #region 设置项

        /// <summary>
        /// 初始化代码生成配置查找工具
        /// </summary>
        private void InitSearchable()
        {
            if (m_Target.CodeGeneratorSettingData == null || m_Target.CodeGeneratorSettingData.IsEmpty())
            {
                m_Target.SetSettingData(m_CodeGeneratorSettingConfig.Default);
                m_LastSettingDataNameIndex = 0;
            }
            else
            {
                int index = m_CodeGeneratorSettingConfig.GetSettingDataIndex(m_Target.CodeGeneratorSettingData.Name);
                if (index == -1)
                {
                    Debug.LogError(
                        $"不存在名为‘{m_Target.CodeGeneratorSettingData.Name}’的{nameof(CodeGeneratorSettingData)}");
                    m_SettingDataError = true;
                    return;
                }

                m_Target.SetSettingData(m_CodeGeneratorSettingConfig.GetSettingData(index));
                m_LastSettingDataNameIndex = index;
            }
            string [] settingDataNames = m_CodeGeneratorSettingConfig.GetAllSettingNames().ToArray();

            m_Searchable = serializedObject.FindProperty("m_SettingDataSearchable");
            m_Target.SetSearchable(settingDataNames, m_LastSettingDataNameIndex);
        }

        /// <summary>
        /// 绘制设置项
        /// </summary>
        private void DrawSetting()
        {
            m_SettingDataExpanded = EditorGUILayout.Foldout(m_SettingDataExpanded, "SettingData", true);

            if (!m_SettingDataExpanded)
            {
                return;
            }

            if (m_SettingDataError)
            {
                EditorGUILayout.HelpBox($"不存在名为‘{m_Target.CodeGeneratorSettingData.Name}’的AutoBindSettingData",
                    MessageType.Error);
                if (!string.IsNullOrEmpty(m_Target.CodeGeneratorSettingData.Name))
                {
                    if (GUILayout.Button($"创建 {m_Target.CodeGeneratorSettingData.Name} 绑定配置"))
                    {
                        bool result =
                            ReferenceBindUtility.AddAutoBindSetting(m_Target.CodeGeneratorSettingData.Name, "", "");
                        if (!result)
                        {
                            EditorUtility.DisplayDialog("创建配置", "创建代码自动生成配置失败，请检查配置信息！", "确定");
                            return;
                        }

                        m_Target.SetSettingData(m_Target.CodeGeneratorSettingData.Name);
                        m_SettingDataError = false;
                    }
                }

                if (GUILayout.Button("使用默认配置"))
                {
                    m_Target.SetSettingData(m_CodeGeneratorSettingConfig.Default);
                    m_SettingDataError = false;
                }

                return;
            }

            m_Searchable = m_Searchable ?? serializedObject.FindProperty("m_SettingDataSearchable");
            EditorGUILayout.PropertyField(m_Searchable);
            if (m_Target.SettingDataSearchable.Select != m_LastSettingDataNameIndex)
            {
                if (m_Target.SettingDataSearchable.Select >= m_CodeGeneratorSettingConfig.GetCount())
                {
                    m_SettingDataError = true;
                    return;
                }

                m_Target.SetSettingData(m_CodeGeneratorSettingConfig.GetSettingData(m_Target.SettingDataSearchable.Select));
                m_Target.SetClassName(string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                    ? m_Target.gameObject.name
                    : m_Target.GeneratorCodeName);
                m_LastSettingDataNameIndex = m_Target.SettingDataSearchable.Select;
            }


            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("命名空间：");
            EditorGUILayout.LabelField(m_Target.CodeGeneratorSettingData.Namespace);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_Target.SetClassName(EditorGUILayout.TextField(new GUIContent("类名："), m_Target.GeneratorCodeName));

            if (GUILayout.Button("物体名"))
            {
                m_Target.SetClassName(m_Target.gameObject.name);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("代码保存路径：");
            EditorGUILayout.LabelField(m_Target.CodeGeneratorSettingData.CodePath);
            EditorGUILayout.EndHorizontal();
            if (string.IsNullOrEmpty(m_Target.CodeGeneratorSettingData.CodePath))
            {
                EditorGUILayout.HelpBox("代码保存路径不能为空!", MessageType.Error);
            }
        }

        /// <summary>
        /// 检查代码生成配置数据
        /// </summary>
        /// <returns></returns>
        private bool CheckCodeGeneratorSettingData(bool isDebug = true)
        {
            string[] paths = AssetDatabase.FindAssets($"t:{nameof(CodeGeneratorSettingConfig)}");
            if (paths.Length == 0)
            {
                if (isDebug)
                {
                    Debug.LogError($"不存在{nameof(CodeGeneratorSettingConfig)}");
                }

                return false;
            }

            if (paths.Length > 1)
            {
                if (isDebug)
                {
                    Debug.LogError($"{nameof(CodeGeneratorSettingConfig)}数量大于1");
                }

                return false;
            }

            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            m_CodeGeneratorSettingConfig = AssetDatabase.LoadAssetAtPath<CodeGeneratorSettingConfig>(path);
            if (m_CodeGeneratorSettingConfig.GetCount() == 0)
            {
                if (isDebug)
                {
                    Debug.LogError($"不存在{nameof(CodeGeneratorSettingData)}");
                }
                return false;
            }

            return true;
        }

        #endregion

        #region 顶部功能按钮

        /// <summary>
        /// 绘制顶部按钮
        /// </summary>
        private void DrawTopButton()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("排序"))
            {
                Sort();
            }

            if (GUILayout.Button("刷新对象"))
            {
                Refresh();
            }

            if (GUILayout.Button("重置组建字段名"))
            {
                ResetAllFieldName();
            }

            if (GUILayout.Button("删除空对象"))
            {
                RemoveNull();
            }

            if (GUILayout.Button("全部删除"))
            {
                RemoveAll();
            }

            if (GUILayout.Button("绑定组件"))
            {
                RuleBindComponent();
            }

            if (GUILayout.Button("生成绑定代码"))
            {
                string className = !string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                    ? m_Target.GeneratorCodeName
                    : m_Target.gameObject.name;


                var bindDataList = new List<ReferenceBindComponent.BindObjectData>(m_Target.GetAllBindObjectsCount());
                bindDataList.AddRange(m_Target.BindAssetsOrPrefabs);
                bindDataList.AddRange(m_Target.BindComponents);
                m_Target.GetCodeGeneratorRuleHelper().GeneratorCodeAndWriteToFile(bindDataList,
                    m_Target.CodeGeneratorSettingData.Namespace, className, m_Target.CodeGeneratorSettingData.CodePath,
                    null);
            }

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 重置所有字段名
        /// </summary>
        private void ResetAllFieldName()
        {
            m_Target.ResetAllFieldName();
        }

        /// <summary>
        /// 刷新数据
        /// </summary>
        private void Refresh()
        {
            m_Target.Refresh();
        }

        /// <summary>
        /// 排序
        /// </summary>
        private void Sort()
        {
            m_Target.Sort();
        }

        /// <summary>
        /// 全部删除
        /// </summary>
        private void RemoveAll()
        {
            m_Target.RemoveAll();
        }

        /// <summary>
        /// 删除Missing Or Null
        /// </summary>
        private void RemoveNull()
        {
            m_Target.RemoveNull();
        }

        /// <summary>
        /// 规则绑定组件
        /// </summary>
        private void RuleBindComponent()
        {
            m_Target.RuleBindComponents();
        }

        #endregion

        #region 绑定资源或预制体编辑器

        private UnityEngine.Object m_NeedBindObject = null;

        /// <summary>
        /// 绘制绑定资源或预制体编辑器
        /// </summary>
        private void DrawBindAssetOrPrefab()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("绑定资源或预制体");
            m_NeedBindObject = EditorGUILayout.ObjectField(m_NeedBindObject, typeof(UnityEngine.Object), false);
            GUI.enabled = m_NeedBindObject != null;
            if (GUILayout.Button("绑定", GUILayout.Width(50)))
            {
                m_Target.RuleBindAssetsOrPrefabs(
                    m_Target.GetBindAssetOrPrefabRuleHelper().GetDefaultFieldName(m_NeedBindObject),
                    m_NeedBindObject);
                m_NeedBindObject = null;
            }

            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region 绑定对象信息

        private void DrawBindObjects()
        {
            int bindAopNeedDeleteIndex = -1;
            int bindComNeedDeleteIndex = -1;

            int i = m_Page.CurrentPage * m_Page.ShowCount;
            int count = i + m_Page.ShowCount;

            if (count > m_Target.GetAllBindObjectsCount())
            {
                count = m_Target.GetAllBindObjectsCount();
            }

            if (count == 0)
            {
                return;
            }

            EditorGUILayout.BeginVertical();

            int bindAssetShowCount = m_Target.BindAssetsOrPrefabs.Count - i;

            if (bindAssetShowCount > 0)
            {
                EditorGUILayout.LabelField("绑定的资源或预制体");
                for (; i < bindAssetShowCount; i++)
                {
                    if (DrawBindObjectData(m_Target.BindAssetsOrPrefabs[i], i))
                    {
                        bindAopNeedDeleteIndex = i;
                    }
                }
            }

            int bindComponentShowCount = count - i;
            if (bindComponentShowCount > 0)
            {
                EditorGUILayout.LabelField("绑定的组件");
                int index = i > m_Target.BindAssetsOrPrefabs.Count ? 0 : m_Target.BindAssetsOrPrefabs.Count - i;
                int startIndex = i - m_Target.BindAssetsOrPrefabs.Count;
                for (; index < bindComponentShowCount; index++, i++)
                {
                    if (DrawBindObjectData(m_Target.BindComponents[index + startIndex], i))
                    {
                        bindComNeedDeleteIndex = index;
                    }
                }
            }

            //删除data
            if (bindAopNeedDeleteIndex != -1)
            {
                m_Target.BindAssetsOrPrefabs.RemoveAt(bindAopNeedDeleteIndex);
                m_Target.SyncBindObjects();
            }

            if (bindComNeedDeleteIndex != -1)
            {
                m_Target.BindComponents.RemoveAt(bindComNeedDeleteIndex);
                m_Target.SyncBindObjects();
            }

            EditorGUILayout.EndVertical();
        }

        private bool DrawBindObjectData(ReferenceBindComponent.BindObjectData bindObjectData, int index)
        {
            bool isDelete = false;
            Rect rect = EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"[{index}]", GUILayout.Width(40));

            EditorGUI.BeginChangeCheck();
            string fieldName = EditorGUILayout.TextField(bindObjectData.FieldName);
            if (EditorGUI.EndChangeCheck())
            {
                bindObjectData.FieldName = fieldName;
                Refresh();
            }

            GUI.enabled = false;
            EditorGUILayout.ObjectField(bindObjectData.BindObject, typeof(UnityEngine.Object), true);
            GUI.enabled = true;

            if (GUILayout.Button("X", GUILayout.Width(20)))
            {
                //将元素下标添加进删除list
                isDelete = true;
            }

            EditorGUILayout.EndHorizontal();
            OnBindObjectDataClick(rect, bindObjectData);

            if (bindObjectData.FieldNameIsInvalid)
            {
                EditorGUILayout.HelpBox("绑定对象命名无效 不符合规则。 请修改!", MessageType.Error);
            }

            if (bindObjectData.IsRepeatName)
            {
                EditorGUILayout.HelpBox("绑定对象命名不能相同 请修改!", MessageType.Error);
            }

            return isDelete;
        }

        private void OnBindObjectDataClick(Rect contextRect, ReferenceBindComponent.BindObjectData bindObjectData)
        {
            Event evt = Event.current;
            if (evt.type == EventType.ContextClick)
            {
                Vector2 mousePos = evt.mousePosition;
                if (contextRect.Contains(mousePos))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Refresh FieldName"), false, data =>
                    {
                        bool isComponent = bindObjectData.BindObject is Component;
                        bindObjectData.FieldName = isComponent
                            ? m_Target.GetBindComponentsRuleHelper().GetDefaultFieldName(
                                (Component)bindObjectData.BindObject)
                            : m_Target.GetBindAssetOrPrefabRuleHelper().GetDefaultFieldName(bindObjectData.BindObject);

                        Refresh();
                    }, bindObjectData);
                    menu.ShowAsContext();

                    evt.Use();
                }
            }
        }

        #endregion
    }
}
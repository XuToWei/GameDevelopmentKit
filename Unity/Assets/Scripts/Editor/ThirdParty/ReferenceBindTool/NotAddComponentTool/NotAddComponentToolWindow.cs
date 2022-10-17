using System;
using System.Linq;
using ReferenceBindTool.Runtime;
using UnityEditor;
using UnityEngine;

namespace ReferenceBindTool.Editor
{
    public class NotAddComponentToolWindow : EditorWindow
    {
        [MenuItem("Tools/ReferenceBindTools/NotAddComponentToolWindow")]
        private static void ShowWindow()
        {
            var window = GetWindow<NotAddComponentToolWindow>();
            window.titleContent = new GUIContent("NotAddComponentToolWindow");
            window.minSize = new Vector2(600, 450);
            window.Show();
        }

        private NotAddComponentData m_Target;
        private Page m_Page;
        [SerializeField] private SearchableData m_SettingDataSearchable;
        private SerializedProperty m_SettingDataSearchableProperty;
        private CodeGeneratorSettingConfig m_CodeGeneratorSettingConfig;
        private bool m_SettingDataExpanded = true;
        private int m_LastSettingDataNameIndex;
        private bool m_SettingDataError;
        private RuleHelperInfo<IBindComponentsRuleHelper> m_BindComponentsRuleHelperInfo;
        private RuleHelperInfo<ICodeGeneratorRuleHelper> m_CodeGeneratorRuleHelperInfo;

        private bool m_IsInitError = false;

        private void OnEnable()
        {
            try
            {
                m_Target = new NotAddComponentData();
                m_Page = new Page(10, m_Target.BindComponents.Count);
                if (!CheckCodeGeneratorSettingData())
                {
                    m_IsInitError = true;
                    return;
                }

                InitSearchable();
                InitHelperInfos();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                m_IsInitError = true;
            }
        }

        private GameObject m_CurrentGameObject;

        public void OnGUI()
        {
            if (m_IsInitError)
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

            m_CurrentGameObject =
                (GameObject) EditorGUILayout.ObjectField("GameObject", m_CurrentGameObject, typeof(GameObject), true);

            if (m_Target.GameObject != m_CurrentGameObject)
            {
                SetGameObject(m_CurrentGameObject);
            }

            if (m_Target.GameObject == null)
            {
                return;
            }

            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
            {
                DrawButtons();
                EditorGUILayout.Space();
                DrawHelperSelect();
                EditorGUILayout.Space();
                DrawSetting();
                EditorGUILayout.Space();
                DrawBindObjects();
                m_Page.SetAllCount(m_Target.BindComponents.Count);
                m_Page.Draw();
            }
        }

        private void SetGameObject(GameObject gameObject)
        {
            m_Target.GameObject = gameObject;
            m_SettingDataError = false;

            if (m_Target.CodeGeneratorSettingData == null || m_Target.CodeGeneratorSettingData.IsEmpty())
            {
                m_Target.SetSettingData(m_CodeGeneratorSettingConfig.Default);
            }
            else
            {
                var data = m_CodeGeneratorSettingConfig.GetSettingData(m_Target.CodeGeneratorSettingData.Name);
                if (data == null)
                {
                    Debug.LogError($"不存在名为‘{m_Target.CodeGeneratorSettingData.Name}’的AutoBindSettingData");
                    m_SettingDataError = true;
                    return;
                }

                m_Target.SetSettingData(
                    m_CodeGeneratorSettingConfig.GetSettingData(m_Target.CodeGeneratorSettingData.Name));
            }

            m_Target.SetClassName(string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                ? m_Target.GameObject.name
                : m_Target.GeneratorCodeName);


            m_Target.BindComponents.Clear();
        }

        #region 规则帮助类

        private bool m_IsCompiling = false;

        private void InitHelperInfos()
        {
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
                ? typeof(TransformFindCodeGeneratorRuleHelper).FullName
                : m_Target.CodeGeneratorRuleHelperTypeName);
            m_CodeGeneratorRuleHelperInfo = new RuleHelperInfo<ICodeGeneratorRuleHelper>("m_CodeGeneratorRule", new[]
            {
                typeof(DefaultCodeGeneratorRuleHelper).FullName,
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
            m_BindComponentsRuleHelperInfo.Refresh();
            m_CodeGeneratorRuleHelperInfo.Refresh();
        }

        /// <summary>
        /// 绘制辅助器选择框
        /// </summary>
        private void DrawHelperSelect()
        {
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
            var settingDataNames = m_CodeGeneratorSettingConfig.GetAllSettingNames().ToArray();
            m_SettingDataSearchable = new SearchableData()
            {
                Select = 0,
                Names = settingDataNames,
            };

            m_SettingDataSearchableProperty = new SerializedObject(this).FindProperty("m_SettingDataSearchable");
        }

        private void ReloadSearchableData(string currentName)
        {
            string[] paths = AssetDatabase.FindAssets($"t:{nameof(CodeGeneratorSettingConfig)}");
            if (paths.Length == 0)
            {
                Debug.LogError($"不存在{nameof(CodeGeneratorSettingConfig)}");
                return;
            }

            if (paths.Length > 1)
            {
                Debug.LogError($"{nameof(CodeGeneratorSettingConfig)}数量大于1");
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            m_CodeGeneratorSettingConfig = AssetDatabase.LoadAssetAtPath<CodeGeneratorSettingConfig>(path);
            if (m_CodeGeneratorSettingConfig.GetCount() == 0)
            {
                Debug.LogError($"不存在{nameof(CodeGeneratorSettingData)}");
                return;
            }

            var settingDataNames = m_CodeGeneratorSettingConfig.GetAllSettingNames().ToList();

            int index = m_CodeGeneratorSettingConfig.GetSettingDataIndex(m_Target.CodeGeneratorSettingData.Name);
            if (index == -1)
            {
                m_SettingDataError = true;
            }
            else
            {
                m_SettingDataSearchable.Select = index;
                m_SettingDataSearchable.Names = settingDataNames.ToArray();
                m_SettingDataSearchableProperty = new SerializedObject(this).FindProperty("m_SettingDataSearchable");
            }
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

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(m_SettingDataSearchableProperty);
            if (m_SettingDataSearchable.Select != m_LastSettingDataNameIndex)
            {
                if (m_SettingDataSearchable.Select >= m_CodeGeneratorSettingConfig.GetCount())
                {
                    m_SettingDataError = true;
                    return;
                }

                m_Target.SetSettingData(m_CodeGeneratorSettingConfig.GetSettingData(m_SettingDataSearchable.Select));
                m_Target.SetClassName(string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                    ? m_Target.GameObject.name
                    : m_Target.GeneratorCodeName);
                m_LastSettingDataNameIndex = m_SettingDataSearchable.Select;
            }

            if (GUILayout.Button("Reload", GUILayout.Width(80)))
            {
                ReloadSearchableData(m_SettingDataSearchable.Names[m_SettingDataSearchable.Select]);
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.PrefixLabel("命名空间：");
            EditorGUILayout.LabelField(m_Target.CodeGeneratorSettingData.Namespace);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            m_Target.SetClassName(EditorGUILayout.TextField(new GUIContent("类名："), m_Target.GeneratorCodeName));

            if (GUILayout.Button("物体名"))
            {
                m_Target.SetClassName(m_Target.GameObject.name);
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
        private bool CheckCodeGeneratorSettingData()
        {
            string[] paths = AssetDatabase.FindAssets($"t:{nameof(CodeGeneratorSettingConfig)}");
            if (paths.Length == 0)
            {
                Debug.LogError($"不存在{nameof(CodeGeneratorSettingConfig)}");
                return false;
            }

            if (paths.Length > 1)
            {
                Debug.LogError($"{nameof(CodeGeneratorSettingConfig)}数量大于1");
                return false;
            }

            string path = AssetDatabase.GUIDToAssetPath(paths[0]);
            m_CodeGeneratorSettingConfig = AssetDatabase.LoadAssetAtPath<CodeGeneratorSettingConfig>(path);
            if (m_CodeGeneratorSettingConfig.GetCount() == 0)
            {
                Debug.LogError($"不存在{nameof(CodeGeneratorSettingData)}");
                return false;
            }

            return true;
        }

        #endregion

        #region 按钮

        /// <summary>
        /// 绘制功能按钮
        /// </summary>
        private void DrawButtons()
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
                RuleBindComponents();
            }

            if (GUILayout.Button("生成绑定代码"))
            {
                string className = !string.IsNullOrEmpty(m_Target.GeneratorCodeName)
                    ? m_Target.GeneratorCodeName
                    : m_Target.GameObject.name;

                m_Target.CodeGeneratorRuleHelper.GeneratorCodeAndWriteToFile(m_Target.BindComponents,
                    m_Target.CodeGeneratorSettingData.Namespace, className, m_Target.CodeGeneratorSettingData.CodePath,
                    m_Target.GameObject.transform);
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
        /// 自动绑定组件
        /// </summary>
        private void RuleBindComponents()
        {
            m_Target.RuleBindComponents();
        }

        #endregion

        #region 绑定对象信息

        private void DrawBindObjects()
        {
            int bindComNeedDeleteIndex = -1;

            int i = m_Page.CurrentPage * m_Page.ShowCount;
            int count = i + m_Page.ShowCount;
            if (count > m_Target.BindComponents.Count)
            {
                count = m_Target.BindComponents.Count;
            }

            EditorGUILayout.BeginVertical();
            for (; i < count; i++)
            {
                if (DrawBindObjectData(m_Target.BindComponents[i], i))
                {
                    bindComNeedDeleteIndex = i;
                }
            }

            if (bindComNeedDeleteIndex != -1)
            {
                m_Target.BindComponents.RemoveAt(bindComNeedDeleteIndex);
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
                        bindObjectData.FieldName = m_Target.BindComponentsRuleHelper.GetDefaultFieldName(
                            (Component) bindObjectData.BindObject);

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
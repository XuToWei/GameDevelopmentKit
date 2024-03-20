using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperWindow : EditorWindow
    {
        private enum ContentState
        {
            None,
            StripCheck,
            GlobalSetting,
            ShaderSetting
        }
        
        private static ShaderVariantsStripperWindow m_window;
        private static ShaderVariantsStripperWindow Window
        {
            get
            {
                if (m_window == null)
                {
                    m_window = EditorWindow.GetWindow<ShaderVariantsStripperWindow>("ShaderVariantsStripper");
                    m_window.minSize = cMinWindowSize;
                }
                return m_window;
            }
        }

        private GUIStyle mBlackStyle, mItemStyle;

        private ShaderVariantsStripperConfig mConfig;
        private string mReadJsonPath = "";

        //右侧菜单滚动条、状态
        private ContentState mCurrentContentState = ContentState.None;
        private Vector2 mContentScrollViewPos = Vector2.zero;
        
        private int mSelectedCondition = 0;
        private Type[] mConditionTypes = new Type[0];
        private string[] mConditionNames = null;
        
        //Shader View变量
        private Vector2 mShaderViewScrollViewPos = Vector2.zero;
        private Shader mShaderViewSelectedShader = null;
        private bool mTextShaderInput = false;
        private string mTextShaderInputStr = "";
        private Shader mWillInsertShader = null;
        private string mFilterShaderName = "";
        private Dictionary<Shader, ShaderVariantsItem> mFilterShaders = new Dictionary<Shader, ShaderVariantsItem>();
        
        //剔除检查
        private Shader mStripCheckShader;
        private ShaderVariantsData mStripCheckData;
        private static MethodInfo sGetShaderGlobalKeywordsMethod = null;
        private static MethodInfo sGetShaderLocalKeywordsMethod = null;
        private string[] mStripCheckShaderGlobalKeywords;
        private string[] mStripCheckShaderLocalKeywords;
        private enum AccessLevel
        {
            Global,
            Local
        }
        private AccessLevel mStripCheckAccessLevel = AccessLevel.Global;
        private int mStripCheckSelectedKeywordIndex;
        private List<(ConditionPair conditionPair, ShaderVariantsStripperConfig config)> mStripCheckConditionList = new List<(ConditionPair condition, ShaderVariantsStripperConfig config)>();
        private Vector2 mStripCheckConditionListScrollViewPos = Vector2.zero;

        private static int cBorderWidth = 10;
        private static Vector2 cMinWindowSize = new Vector2(1200, 600);
        private static int cLeftWidth = 400;
        private static int cLeftTopHeight = 100;
        private static int cLeftMiddleHeight = 100;
        private static int cMiddleWidth = 20;

        [MenuItem("Tools/Soco/ShaderVariantsStripper/OpenStripperWindow", priority = 200)]
        public static void OpenWindow()
        {
            Window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.Space(cBorderWidth);
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));

            #region 左半部分
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(cLeftWidth));

            #region 上半部分 读取配置文件
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(cLeftTopHeight));
            EditorGUILayout.LabelField("配置文件：");
            ShaderVariantsStripperConfig newConfig = EditorGUILayout.ObjectField(mConfig, typeof(ShaderVariantsStripperConfig), false) as ShaderVariantsStripperConfig;

            if (newConfig != mConfig)
            {
                SaveObject(mConfig);
                mConfig = newConfig;
            }
            
            if (mConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                Color oriColor = GUI.color;
                GUI.color = mConfig.mEnable ? Color.green : Color.red;
                if (GUILayout.Button(new GUIContent((mConfig.mEnable ? "已启用" : "已禁用"), "启用或禁用当前配置文件")))
                {
                    mConfig.mEnable = !mConfig.mEnable;
                }

                GUI.color = mConfig.mIsWhiteList ? Color.white : Color.grey;
                if (GUILayout.Button(new GUIContent((mConfig.mIsWhiteList ? "白名单" : "剔除列表"), "当前配置文件是否为白名单")))
                {
                    mConfig.mIsWhiteList = !mConfig.mIsWhiteList;
                }
                GUI.color = oriColor;
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("序列化Config为Json", GUILayout.Width(cLeftWidth * 0.5f)))
                {
                    string configPath = AssetDatabase.GetAssetPath(mConfig);
                    string jsonPath = configPath.Replace(".asset", ".json");

                    string json = JsonUtility.ToJson(mConfig);

                    using (StreamWriter writer = new StreamWriter(jsonPath, false))
                    {
                        writer.Write(json);
                    }
                    
                    ShowNotification(new GUIContent($"成功保存至{jsonPath}"));
                }
                
                if (GUILayout.Button("选择读取路径", GUILayout.Width(cLeftWidth * 0.25f)))
                {
                    string configPath = AssetDatabase.GetAssetPath(mConfig);
                    
                    mReadJsonPath = EditorUtility.OpenFilePanel("选择JSON路径",
                        (mReadJsonPath == "" ? Path.GetDirectoryName(configPath) : mReadJsonPath), "json");
                }
                if (GUILayout.Button("读取JSON", GUILayout.Width(cLeftWidth * 0.25f)) && mReadJsonPath != ""
                    && EditorUtility.DisplayDialog("注意", "是否读取Json覆盖到当前配置文件，确认后不可恢复", "确认", "停，停！"))
                {
                    if (System.IO.File.Exists(mReadJsonPath))
                    {
                        using (StreamReader reader = new StreamReader(mReadJsonPath))
                        {
                            string json = reader.ReadToEnd();
                            JsonUtility.FromJsonOverwrite(json, mConfig);
                        }
                    } 
                    else
                        Debug.LogError($"Json文件{mReadJsonPath}不存在");
                }
                EditorGUILayout.EndHorizontal();
                if (mReadJsonPath != "")
                {
                    EditorGUILayout.LabelField(new GUIContent(mReadJsonPath, mReadJsonPath));
                }
            }
            else//mConfig == null
            {
                Color oriColor = GUI.color;
                GUI.color = mCurrentContentState == ContentState.StripCheck ? Color.green : oriColor;
                if (GUILayout.Button(new GUIContent("剔除检查", "给定一组变体和条件，检查该变体是否会被剔除")))
                {
                    mCurrentContentState = ContentState.StripCheck;
                }
                GUI.color = oriColor;
            }
            
            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.Space(cBorderWidth);
            
            #region 中部 全局设置
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(cLeftMiddleHeight));
            EditorGUILayout.LabelField("Global Setting View");
            if (mConfig != null)
            {
                string globalText = "全局设置" + 
                                    (mConfig.mGlobalConditions.Count == 0 ? " " : $"({mConfig.mGlobalConditions.Count}条)");

                string applyText = "";
                if (mConfig.mIsWhiteList)
                {
                    applyText = (mConfig.mShaderConditions.Count == 0 ? "(当前无效)" : "(应用于ShaderView中Shader)");
                }
                else
                {
                    applyText = (mConfig.mShaderConditions.Count == 0 ? "(应用于全体Shader)" : "(应用于ShaderView中Shader)");
                }
                    
                if (GUILayout.Button(new GUIContent(globalText + applyText, "浏览全局设置"), GUILayout.ExpandHeight(true),
                        GUILayout.ExpandWidth(true)))
                {
                    mCurrentContentState = ContentState.GlobalSetting;
                }
            }
            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.Space(cBorderWidth);
            
            #region 下半部分 选择Shader
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(position.height - cLeftTopHeight - cLeftMiddleHeight - 4 * cBorderWidth));
            EditorGUILayout.LabelField("Shader View");

            if (mConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                if (mTextShaderInput)
                {
                    mTextShaderInputStr = EditorGUILayout.TextField(mTextShaderInputStr);
                }
                else
                {
                    mWillInsertShader = EditorGUILayout.ObjectField(mWillInsertShader, typeof(Shader), true) as Shader;
                }

                Color oriColor = GUI.color;
                if (mTextShaderInput)
                    GUI.color = Color.red;
                
                if (GUILayout.Button("文本输入"))
                {
                    mTextShaderInput = !mTextShaderInput;
                }

                GUI.color = oriColor;
                
                if (GUILayout.Button("添加"))
                {
                    if (mTextShaderInput)
                    {
                        mWillInsertShader = Shader.Find(mTextShaderInputStr);
                    }

                    if (mWillInsertShader != null && !mConfig.mShaderConditions.ContainsKey(mWillInsertShader))
                    {
                        var item = new ShaderVariantsItem();
                        item.conditionPairs = new List<ConditionPair>();
                        item.applyGlobalConfig = true;
                        mConfig.mShaderConditions.Add(mWillInsertShader, item);
                        if (mFilterShaderName != "" && mWillInsertShader.name.IndexOf(mFilterShaderName, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            mFilterShaders.Add(mWillInsertShader, item);
                        }
                    }

                }
                EditorGUILayout.EndHorizontal();

                string prevFilterShaderName = mFilterShaderName;
                mFilterShaderName = EditorGUILayout.TextField("过滤", mFilterShaderName);
                if (mFilterShaderName == "")
                {
                    mFilterShaders.Clear();
                }
                else if (prevFilterShaderName != mFilterShaderName)
                {
                    mFilterShaders.Clear();
                    foreach (var kvp in mConfig.mShaderConditions)
                    {
                        Shader shader = kvp.Key;
                        if (shader.name.IndexOf(mFilterShaderName, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            mFilterShaders.Add(kvp.Key, kvp.Value);
                        }
                    }
                }
                
                mShaderViewScrollViewPos = EditorGUILayout.BeginScrollView(mShaderViewScrollViewPos);
                Shader willRemoveInDict = null;

                Dictionary<Shader, ShaderVariantsItem> displayList =
                    mFilterShaderName == "" ? mConfig.mShaderConditions : mFilterShaders;

                foreach (var kvp in displayList)
                {
                    Shader shader = kvp.Key;
                    ShaderVariantsItem item = kvp.Value;
                    
                    Color oriGUIColor = GUI.color;
                    if (shader == mShaderViewSelectedShader)
                        GUI.color = Color.green;
                
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button(new GUIContent(shader.name, shader.name), GUILayout.MinWidth(cLeftWidth - 100)))
                    {
                        if (mShaderViewSelectedShader == shader)//选中状态下再次选择,在项目中定位
                        {
                            Selection.activeObject = shader;
                            EditorGUIUtility.PingObject(shader);
                        }
                    
                        mShaderViewSelectedShader = shader;
                        mCurrentContentState = ContentState.ShaderSetting;
                    }

                    GUI.color = item.applyGlobalConfig ? Color.green : Color.red;
                    if (GUILayout.Button(new GUIContent("G", "应用全局设置"), GUILayout.MinWidth(45)))
                    {
                        item.applyGlobalConfig =
                            !item.applyGlobalConfig;
                    }

                    GUI.color = oriGUIColor;
                    if (GUILayout.Button("删除", GUILayout.MinWidth(45)))
                    {
                        willRemoveInDict = shader;
                        if (mCurrentContentState == ContentState.ShaderSetting && mShaderViewSelectedShader == shader)
                        {
                            mCurrentContentState = ContentState.None;
                            mShaderViewSelectedShader = null;
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }

                if (willRemoveInDict != null)
                {
                    Undo.RegisterCompleteObjectUndo(mConfig, "ShaderVariantsStripper config delete shader");
                    mConfig.mShaderConditions.Remove(willRemoveInDict);
                    mFilterShaders.Remove(willRemoveInDict);
                }
                    
                
                EditorGUILayout.EndScrollView();
            }
  
            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.EndVertical();
            #endregion
            
            #region 中间分隔线
            EditorGUILayout.BeginVertical(GUILayout.MinWidth(cMiddleWidth));
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
            #endregion

            #region 右半部分
            int rightWidth = (int)(position.width - cLeftWidth - cMiddleWidth - 20);
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinWidth(rightWidth), GUILayout.MinHeight(position.height - cBorderWidth * 2));
            if (mConfig != null)
            {
                #region 标题状态+获取列表
                List<ConditionPair> conditionList = null;
                switch (mCurrentContentState)
                {
                    case ContentState.GlobalSetting:
                        EditorGUILayout.LabelField("全局设置");
                        conditionList = mConfig.mGlobalConditions;
                        break;
                    case ContentState.ShaderSetting:
                        EditorGUILayout.LabelField($"Shader<{mShaderViewSelectedShader.name}>设置");
                        if (mShaderViewSelectedShader != null && mConfig.mShaderConditions.TryGetValue(mShaderViewSelectedShader, out ShaderVariantsItem svi))
                        {
                            conditionList = svi.conditionPairs;
                        }
                        break;
                    default:
                        EditorGUILayout.LabelField("未选择，请选择<全局设置>或<Shader>");
                        break;
                }
                #endregion
                
                if (conditionList != null)
                {
                    #region 添加条件
                    EditorGUILayout.BeginHorizontal();
                    mSelectedCondition = EditorGUILayout.Popup(mSelectedCondition, mConditionNames, GUILayout.MinWidth(rightWidth * 0.7f));
                    if (GUILayout.Button("添加", GUILayout.MinWidth(rightWidth * 0.3f)))
                    {
                        if (mSelectedCondition >= mConditionTypes.Length)
                        {
                            Debug.LogError("程序有改变，请重新打开窗口");
                            return;
                        }
                        
                        ShaderVariantsStripperCondition condition =
                            Activator.CreateInstance(mConditionTypes[mSelectedCondition]) as
                                ShaderVariantsStripperCondition;
                        conditionList.Add(new ConditionPair(){condition = condition, strip = false, priority = 0});
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion

                    #region 浏览条件
                    mContentScrollViewPos = EditorGUILayout.BeginScrollView(mContentScrollViewPos);

                    for (int i = 0; i < conditionList.Count; ++i)
                    {
                        //var (globalCondition, strip) = conditionList[i];
                        ConditionPair conditionPair = conditionList[i];
                        // var condition = conditionList[i].condition;
                        // bool strip = conditionList[i].strip;
                        // uint priority = conditionList[i].priority;

                        EditorGUILayout.BeginHorizontal(mItemStyle, GUILayout.Width(rightWidth), GUILayout.Height(50));

                        if (!mConfig.mIsWhiteList)
                        {
                            Color oriGUIColor = GUI.color;
                            if (conditionPair.strip)
                                GUI.color = Color.red;
                            else
                                GUI.color = Color.green;
                            if (GUILayout.Button((conditionPair.strip ? "剔除" : "保留"), GUILayout.Width(50),
                                    GUILayout.Height(50)))
                            {
                                //conditionList[i] = new ConditionPair(){condition = condition, strip = !strip, priority = priority};
                                conditionPair.strip = !conditionPair.strip;
                                conditionList[i] = conditionPair;
                            }
                            GUI.color = oriGUIColor;

                            uint newPriority = (uint)Mathf.Max(0,
                                EditorGUILayout.IntField((int)conditionPair.priority, GUILayout.Width(50),
                                    GUILayout.Height(50)));
                            if (newPriority != conditionPair.priority)
                            {
                                conditionPair.priority = newPriority;
                                conditionList[i] = conditionPair;
                            }
                        }

                        if (conditionPair.condition == null)
                            conditionList.RemoveAt(i);

                        if (GUILayout.Button(conditionPair.condition.Overview(), GUILayout.Width(rightWidth - 3 * 50 - 40), GUILayout.Height(50)))
                        {
                            ShaderVariantsStripperConditionWindow.Window.mCondition = conditionPair.condition;
                            ShaderVariantsStripperConditionOnGUIContext context = new ShaderVariantsStripperConditionOnGUIContext()
                            {
                                shader = mCurrentContentState == ContentState.ShaderSetting ? mShaderViewSelectedShader : null
                            };
                            context.Init();
                            ShaderVariantsStripperConditionWindow.Window.mContext = context;
                            
                            ShaderVariantsStripperConditionWindow.Window.Show();
                            ShowNotification(new GUIContent("已打开条件窗口，如未发现请检查窗口是否被当前窗口覆盖"));
                        }

                        if (GUILayout.Button("删除", GUILayout.Width(50), GUILayout.Height(50)))
                        {
                            Undo.RegisterCompleteObjectUndo(mConfig, "ShaderVariantsStripper config delete condition");
                            conditionList.RemoveAt(i);
                        }

                        EditorGUILayout.BeginVertical(GUILayout.Width(25), GUILayout.Height(50));
                        if (GUILayout.Button("↑", GUILayout.Width(25), GUILayout.Height(25)) && i != 0)
                            (conditionList[i - 1], conditionList[i]) = (conditionList[i], conditionList[i - 1]);
                        if (GUILayout.Button("↓", GUILayout.Width(25), GUILayout.Height(25)) && i != conditionList.Count - 1)
                            (conditionList[i + 1], conditionList[i]) = (conditionList[i], conditionList[i + 1]);
                            
                        EditorGUILayout.EndVertical();
                        
                        EditorGUILayout.EndHorizontal();
                        EditorGUILayout.Space(5);
                    }
                    EditorGUILayout.EndScrollView();
                    #endregion
                }
            }
            else// mConfig == null
            {
                #region 剔除检查
                if (mCurrentContentState == ContentState.StripCheck)
                {
                    Shader nextStripCheckShader =
                        EditorGUILayout.ObjectField("剔除检查目标Shader", mStripCheckShader, typeof(Shader), true) as Shader;

                    if (nextStripCheckShader != mStripCheckShader)
                    {
                        mStripCheckShader = nextStripCheckShader;
                        mStripCheckData = ShaderVariantsData.GetDefaultShaderVariantsData();
                        mStripCheckAccessLevel = AccessLevel.Global;
                        mStripCheckSelectedKeywordIndex = 0;
                        if (nextStripCheckShader != null)
                        {
                            mStripCheckShaderGlobalKeywords = sGetShaderGlobalKeywordsMethod.Invoke(null, new object[] { nextStripCheckShader }) as string[];
                            mStripCheckShaderLocalKeywords = sGetShaderLocalKeywordsMethod.Invoke(null, new object[] { nextStripCheckShader }) as string[];
                        }
                    }
                    
                    if (mStripCheckShader != null)
                    {
                        EditorGUILayout.LabelField("变体条件");
                        mStripCheckData.shaderType = (ShaderVariantsDataShaderType) EditorGUILayout.EnumPopup("ShaderType", mStripCheckData.shaderType);
                        mStripCheckData.shaderType = (ShaderVariantsDataShaderType)Mathf.Clamp((int)mStripCheckData.shaderType,
                            (int)ShaderVariantsDataShaderType.Vertex, (int)ShaderVariantsDataShaderType.RayTracing + 1);
                        mStripCheckData.passType = (UnityEngine.Rendering.PassType) EditorGUILayout.EnumPopup("PassType", mStripCheckData.passType);
                        mStripCheckData.passName = EditorGUILayout.TextField("PassName", mStripCheckData.passName);
                        
                        string[] keywords = mStripCheckData.GetShaderKeywords();
                        
                        EditorGUILayout.LabelField($"Keyword,共{keywords.Length}个");
                        
                        EditorGUILayout.BeginHorizontal();
                        AccessLevel newAccessLevel = (AccessLevel)EditorGUILayout.Popup((int) mStripCheckAccessLevel, new string[] { "Global", "Local" }, GUILayout.Width(rightWidth * 0.3f));
                        if (newAccessLevel != mStripCheckAccessLevel)
                        {
                            mStripCheckSelectedKeywordIndex = 0;
                            mStripCheckAccessLevel = newAccessLevel;
                        }
                        
                        if (mStripCheckAccessLevel == AccessLevel.Global && mStripCheckShaderGlobalKeywords.Length > 0 ||
                            mStripCheckAccessLevel == AccessLevel.Local && mStripCheckShaderLocalKeywords.Length > 0)
                        {
                            string[] keywordList = mStripCheckAccessLevel == AccessLevel.Global
                                ? mStripCheckShaderGlobalKeywords
                                : mStripCheckShaderLocalKeywords;
                            mStripCheckSelectedKeywordIndex = EditorGUILayout.Popup(mStripCheckSelectedKeywordIndex,
                                keywordList, GUILayout.Width(rightWidth * 0.3f));
                
                            string selectedKeyword = keywordList[mStripCheckSelectedKeywordIndex];
                            UnityEngine.Rendering.ShaderKeyword newKeyword =
                                mStripCheckAccessLevel == AccessLevel.Global
                                    ? new ShaderKeyword(selectedKeyword)
                                    : new ShaderKeyword(mStripCheckShader, selectedKeyword);

                            if (GUILayout.Button("添加", GUILayout.Width(rightWidth * 0.3f)) && !mStripCheckData.IsKeywordEnabled(newKeyword))
                            {
                                mStripCheckData.EnableKeyword(newKeyword);
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        int keywordCount = 0;
                        foreach (var keyword in keywords)
                        {
                            if (GUILayout.Button(new GUIContent(keyword, keyword), GUILayout.Width(rightWidth * 0.2f)))
                            {
                                mStripCheckData.DisableKeyword(keyword);
                            }
                            keywordCount++;
                            if (keywordCount % 4 == 0)
                            {
                                EditorGUILayout.EndHorizontal();
                                EditorGUILayout.BeginHorizontal();
                            }
                            else
                                EditorGUILayout.Space();
                        }
                        EditorGUILayout.EndHorizontal();
                        
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("测试剔除"))
                        {
                            mStripCheckConditionList.Clear();
                            
                            string[] guids = AssetDatabase.FindAssets("t:ShaderVariantsStripperConfig", new string[]{"Assets", "Packages"});

                            ShaderVariantsStripperConfig[] allConfigs = (from guid in guids
                                    select AssetDatabase.LoadAssetAtPath<ShaderVariantsStripperConfig>(
                                        AssetDatabase.GUIDToAssetPath(guid)))
                                .ToArray();
                            ShaderVariantsStripperCode.StripVariant(mStripCheckShader, mStripCheckData, allConfigs, mStripCheckConditionList);
                            
                            bool strip = mStripCheckConditionList.Any(conditionPair_fromConfig =>
                                conditionPair_fromConfig.conditionPair.strip);
                            bool hasWhiteList = mStripCheckConditionList.Any(conditionPair_fromConfig =>
                                conditionPair_fromConfig.config.mIsWhiteList);
                            string res = hasWhiteList ? "白名单" : (strip ? "剔除" : "保留");
                            ShowNotification(new GUIContent("检测结果为：" + res));
                            mStripCheckConditionListScrollViewPos = Vector2.zero;
                        }

                        if (mStripCheckConditionList.Count != 0 && GUILayout.Button("清空结果"))
                        {
                            mStripCheckConditionList.Clear();
                        }
                        EditorGUILayout.EndHorizontal();

                        if (mStripCheckConditionList.Count != 0)
                        {
                            bool strip = mStripCheckConditionList.Any(conditionPair_fromConfig =>
                                conditionPair_fromConfig.conditionPair.strip);
                            
                            bool hasWhiteList = mStripCheckConditionList.Any(conditionPair_fromConfig =>
                                conditionPair_fromConfig.config.mIsWhiteList);
                            
                            string res = hasWhiteList ? "白名单" : (strip ? "剔除" : "保留");
                            EditorGUILayout.LabelField("检测结果为：" + res);
                            
                            mStripCheckConditionListScrollViewPos =
                                EditorGUILayout.BeginScrollView(mStripCheckConditionListScrollViewPos);

                            foreach (var conditionPair_fromConfig in mStripCheckConditionList)
                            {
                                string conditionRes = "";
                                if (conditionPair_fromConfig.config.mIsWhiteList)
                                {
                                    conditionRes = " 白名单";
                                }
                                else
                                {
                                    conditionRes = (conditionPair_fromConfig.conditionPair.strip ? " 剔除" : " 保留") + $" 优先级:{conditionPair_fromConfig.conditionPair.priority}";
                                }
                                
                                if (GUILayout.Button(conditionPair_fromConfig.config.name + conditionRes + $" 条件:{conditionPair_fromConfig.conditionPair.condition.Overview()}"))
                                {
                                    Selection.activeObject = conditionPair_fromConfig.config;
                                    EditorGUIUtility.PingObject(conditionPair_fromConfig.config);
                                }
                            }
                            EditorGUILayout.EndScrollView();
                        }
                    }
                }
                #endregion
            }
            EditorGUILayout.EndVertical();
            #endregion
            
            GUILayout.EndHorizontal();
        }

        void SaveObject(Object obj)
        {
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        
        Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
        
        private void SetupStyle()
        {
            if (mBlackStyle == null)
            {
                Color backColor = EditorGUIUtility.isProSkin ? new Color(0.18f, 0.18f, 0.18f) : new Color(0.7f, 0.7f, 0.7f);
                Texture2D _blackTexture;
                _blackTexture = MakeTex(4, 4, backColor);
                _blackTexture.hideFlags = HideFlags.DontSave;
                mBlackStyle = new GUIStyle();
                mBlackStyle.normal.background = _blackTexture;
            }

            if (mItemStyle == null)
            {
                Color itemColor = EditorGUIUtility.isProSkin ? new Color(0.3f, 0.3f, 0.3f) : new Color(0.9f, 0.9f, 0.9f);
                Texture2D _itemColorTexture;
                _itemColorTexture = MakeTex(4, 4, itemColor);
                _itemColorTexture.hideFlags = HideFlags.DontSave;
                mItemStyle = new GUIStyle();
                mItemStyle.normal.background = _itemColorTexture;
            }
        }

        private void SetupConditionType()
        {
            mConditionTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                assembly => assembly.GetTypes()).Where(
                type => typeof(ShaderVariantsStripperCondition).IsAssignableFrom(type) && !type.IsAbstract
            ).ToArray();

            mConditionNames = mConditionTypes.Select(type => (System.Activator.CreateInstance(type) as ShaderVariantsStripperCondition).GetName()).ToArray();
        }

        private static void InitGetKeywordMethod()
        {
            if (sGetShaderGlobalKeywordsMethod == null)
                sGetShaderGlobalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);

            if (sGetShaderLocalKeywordsMethod == null)
                sGetShaderLocalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);
        }
        
        public void Awake()
        {
            SetupStyle();
            SetupConditionType();
            InitGetKeywordMethod();
        }

        public void OnDisable()
        {
            SaveObject(mConfig);
            ShaderVariantsStripperConditionWindow.Window.Close();
        }
    }
}
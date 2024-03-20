using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace Soco.ShaderVariantsCollection
{

    public class ShaderVariantCollectionToolsWindow : EditorWindow
    {
        private enum FeatureViewState
        {
            None,
            ShaderVariantIndex,
            CollectionTool,
            BatchTool,
        }

        private static Vector2 cMinWindowSize = new Vector2(1200, 600);
        private static ShaderVariantCollectionToolsWindow mwindow;
        public static ShaderVariantCollectionToolsWindow Window
        {
            get
            {
                if (mwindow == null)
                {
                    mwindow = EditorWindow.GetWindow<ShaderVariantCollectionToolsWindow>("ShaderVariantCollectionTools");
                    mwindow.minSize = cMinWindowSize;
                }
                return mwindow;
            }
        }

        private ShaderVariantCollection mCollectionFile;
        [SerializeField]
        private ShaderVariantCollectionMapper mCollectionMapper;

        private ShaderVariantCollectionMapper collectionMapper
        {
            get
            {
                if (mCollectionMapper == null || mCollectionMapper.mCollection != mCollectionFile)
                {
                    mCollectionMapper = ScriptableObject.CreateInstance<ShaderVariantCollectionMapper>();
                    mCollectionMapper.Init(mCollectionFile);
                    if(mShaderViewSelectedShader != null)
                        CollectPassKeywordMap(collectionMapper.GetShaderVariants(mShaderViewSelectedShader));
                }

                return mCollectionMapper;
            }
        }

        private void OnDestroy()
        {
            if (mCollectionMapper != null)
            {
                ScriptableObject.DestroyImmediate(mCollectionMapper);
                mCollectionMapper = null;
            }
        }
        
        private ShaderVariantCollectionToolConfig mConfig;
        
        private FeatureViewState mCurrentFeatureState;
        private Vector2 mFeatureViewScrollViewPos = Vector2.zero;
        private Vector2 mWorkViewScrollViewPos = Vector2.zero;

        private void ResetFeatureView()
        {
            mCurrentFeatureState = FeatureViewState.None;
        }
        
        #region ShaderVariantIndex

        private Shader mWillInsertShader;
        [SerializeField]
        private Shader mShaderViewSelectedShader;
        private string mFilterShaderName = "";
        [SerializeField]
        private List<Shader> mFilterShaders = new List<Shader>();
        
        [Serializable]
        private class CachePassData
        {
            public PassType passType;
            public List<SerializableShaderVariant> variants;
            public bool toggleValue;
        }

        [SerializeField] private string mVariantFilterString = "";
        [SerializeField] private string[] mVariantFilterArray = null;
        private bool mVariantFilterAllMatch = false;

        [SerializeField]
        private List<CachePassData> mPassVariantCacheData = new List<CachePassData>();

        private void ResetShaderView()
        {
            mShaderViewSelectedShader = null;
            mFeatureViewScrollViewPos = Vector2.zero;
            mWorkViewScrollViewPos = Vector2.zero;
            mPassVariantCacheData.Clear();
        }

        private void CollectPassKeywordMap(IEnumerable<UnityEngine.ShaderVariantCollection.ShaderVariant> variants)
        {
            mPassVariantCacheData.Clear();

            foreach (var variant in variants)
            {
                int findRes = mPassVariantCacheData.FindIndex(data => data.passType == variant.passType);
                CachePassData pass;
                if (findRes < 0)
                {
                    pass = new CachePassData()
                    {
                        passType = variant.passType,
                        variants = new List<SerializableShaderVariant>(),
                        toggleValue = false
                    };
                    mPassVariantCacheData.Add(pass);
                }
                else
                {
                    pass = mPassVariantCacheData[findRes];
                }
                
                pass.variants.Add(new SerializableShaderVariant(variant));
            }
        }
        
        //这个方法不会导致Pass重新折叠
        public void RefreshPassKeywordMap(Shader currentShader)
        {
            //如果当前Shader已经变了，则不需要操作
            if (currentShader != mShaderViewSelectedShader)
                return;
            
            //否则刷新数据
            Dictionary<PassType, bool> toggleData = new Dictionary<PassType, bool>();
            foreach (CachePassData data in mPassVariantCacheData)
            {
                toggleData.Add(data.passType, data.toggleValue);
            }

            CollectPassKeywordMap(collectionMapper.GetShaderVariants(currentShader));
            
            foreach (CachePassData data in mPassVariantCacheData)
            {
                if (toggleData.TryGetValue(data.passType, out bool toggleValue))
                    data.toggleValue = toggleValue;
            }
        }
        #endregion
        
        #region CollectionTool

        private enum CollectionViewState
        {
            CollectorList,
            MaterialFilter,
            VariantFilter,
            MaterialFrom
        }
        
        private int mSelectedInterfaceImplIndex;
        private CollectionViewState mCollectionViewState = CollectionViewState.CollectorList;
        private bool mOverrideFile = false;

        private ShaderVariantCollectionMaterialVariantConverter mConverter =
            new ShaderVariantCollectionMaterialVariantConverter();

        private Material mTestMaterial = null;
        
        #endregion
        
        #region BatchTool
        private enum BatchToolViewState
        {
            ExecutorList,
            MergeFile,
            SplitFile,
            MaxCount
        }

        private static GUIContent[] sBatchToolViewStateStyles = new GUIContent[]
        {
            new GUIContent("执行器列表", "打开执行器列表"),
            new GUIContent("合并文件", "将另一个变体收集文件的内容合并到当前文件"),
            new GUIContent("分割文件", "自动将变体收集文件分割为数个文件"),
        };

        private BatchToolViewState mBatchToolViewState = BatchToolViewState.ExecutorList;
        private ShaderVariantCollection mOtherCollectionFile;

        private enum SplitMode
        {
            VariantCount,
            FixedFileCount
        }

        private SplitMode mSplitMode = SplitMode.VariantCount;

        private static string[] sSplitModeStr = new string[]
        {
            "按照变体数量切割",
            "固定变体收集文件",
        };
        
        private string mSplitFileOutputPath = "";
        private bool mSplitFileNoSplitShader = true;
        private bool mSplitFileNoSplitPass = true;
        private int mSplitFileMaxVariantLimit = 50;
        private int mSplitFileFixedCount = 10;
        #endregion
        
        private GUIStyle mBlackStyle, mItemStyle;
        
        private static int cBorderWidth = 10;
        private static int cLeftWidth = 400;
        private static int cLeftTopHeight = 100;
        private static int cLeftMiddleHeight = 100;
        private static int cMiddleWidth = 20;

        [MenuItem("Tools/Soco/ShaderVariantCollectionTools/OpenWindow", priority = 200)]
        public static void OpenWindow()
        {
            Window.Show();
        }

        public void OnGUI()
        {
            EditorGUILayout.Space(cBorderWidth);
            GUILayout.BeginHorizontal(GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            
            #region 左半部分
            EditorGUILayout.BeginVertical(GUILayout.Width(cLeftWidth));
            
            #region 左上部分 收集文件选择 配置文件选择
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(cLeftTopHeight));
            
            EditorGUILayout.LabelField("变体收集文件：");

            Color oriColor = GUI.color;
            if (mCollectionFile == null)
                GUI.color = Color.red;
            ShaderVariantCollection newCollectionFile = EditorGUILayout.ObjectField(mCollectionFile, typeof(ShaderVariantCollection), false) as ShaderVariantCollection;
            GUI.color = oriColor;
            
            if (newCollectionFile != mCollectionFile)
            {
                SaveObject(mCollectionFile);
                mCollectionFile = newCollectionFile;

                ResetShaderView();
            }

            EditorGUILayout.LabelField("工具配置文件：");
            
            var newConfig = EditorGUILayout.ObjectField(mConfig, typeof(ShaderVariantCollectionToolConfig), false) as ShaderVariantCollectionToolConfig;
            if (newConfig != mConfig)
            {
                SaveObject(mConfig);
                mConfig = newConfig;
                
            }

            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.Space(cBorderWidth);
            
            #region 左中部分 功能选择
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(cLeftMiddleHeight));
            EditorGUILayout.LabelField("功能选择");
            if (mCollectionFile != null)
            {
                if (GUILayout.Button(new GUIContent("快速浏览", "快速浏览变体收集文件内容"),GUILayout.ExpandWidth(true)))
                {
                    mCurrentFeatureState = FeatureViewState.ShaderVariantIndex;
                    ResetShaderView();
                }
                
                if (GUILayout.Button(new GUIContent("项目收集工具", "自动收集项目打包所需变体"), GUILayout.ExpandWidth(true)))
                {
                    mCurrentFeatureState = FeatureViewState.CollectionTool;
                    mFeatureViewScrollViewPos = Vector2.zero;
                    mWorkViewScrollViewPos = Vector2.zero;
                    mSelectedInterfaceImplIndex = 0;
                    mCollectionViewState = CollectionViewState.CollectorList;
                }
                
                if (GUILayout.Button(new GUIContent("批处理工具", "批量处理变体收集文件"), GUILayout.ExpandWidth(true)))
                {
                    mCurrentFeatureState = FeatureViewState.BatchTool;
                    mFeatureViewScrollViewPos = Vector2.zero;
                    mWorkViewScrollViewPos = Vector2.zero;
                }
            }
            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.Space(cBorderWidth);
            
            #region 左下部分 次级选项
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinHeight(position.height - cLeftTopHeight - cLeftMiddleHeight - 4 * cBorderWidth));

            if (mCollectionFile != null)
            {
                if (mCurrentFeatureState == FeatureViewState.ShaderVariantIndex)
                {
                    EditorGUILayout.LabelField("Shader View");

                    EditorGUILayout.BeginHorizontal();
                    mWillInsertShader = EditorGUILayout.ObjectField(mWillInsertShader, typeof(Shader), true) as Shader;
                    if (GUILayout.Button("添加"))
                    {
                        if (!collectionMapper.HasShader(mWillInsertShader))
                        {
                            UndoShaderVariantCollectionTool();
                            collectionMapper.AddShader(mWillInsertShader);
                            //添加Shader后，更新Filter列表
                            if (mFilterShaderName != "" &&
                                mWillInsertShader.name.IndexOf(mFilterShaderName, StringComparison.OrdinalIgnoreCase) >= 0 &&
                                !mFilterShaders.Contains(mWillInsertShader))
                            {
                                mFilterShaders.Add(mWillInsertShader);
                            }
                        }
                        else
                            ShowNotification(new GUIContent($"Shader:{mWillInsertShader}已存在于当前变体收集文件"));
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    #region 过滤名称
                    string prevFilterShaderName = mFilterShaderName;
                    mFilterShaderName = EditorGUILayout.TextField("过滤", mFilterShaderName);
                    if (mFilterShaderName == "")
                    {
                        mFilterShaders.Clear();
                    }
                    else if (prevFilterShaderName != mFilterShaderName)
                    {
                        FilterShader();
                    }
                    #endregion

                    if (collectionMapper.shaders.Count > 0 &&  GUILayout.Button(new GUIContent("Clear", "清空变体收集文件"), GUILayout.Width(cLeftWidth)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否确认清空文件", "是", "否"))
                        {
                            UndoShaderVariantCollectionTool();
                            mCollectionFile.Clear();
                            collectionMapper.Refresh();
                            mShaderViewSelectedShader = null;
                            mPassVariantCacheData.Clear();
                            mFilterShaders.Clear();
                        }
                    }
                    
                    //分割线
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    
                    mFeatureViewScrollViewPos = EditorGUILayout.BeginScrollView(mFeatureViewScrollViewPos);
                    
                    IEnumerable<Shader> displayList =
                        (mFilterShaderName == "" ? (collectionMapper.shaders as IEnumerable<Shader>): mFilterShaders);

                    Shader removeShader = null;
                    
                    Color oriGUIColor = GUI.color;
                    foreach (var shader in displayList)
                    {
                        EditorGUILayout.BeginHorizontal(GUILayout.Width(cLeftWidth));
                        
                        if (shader == mShaderViewSelectedShader)
                            GUI.color = Color.green;

                        if (GUILayout.Button(new GUIContent(shader.name, shader.name),
                                GUILayout.Width(cLeftWidth - 30)))
                        {
                            if (mShaderViewSelectedShader == shader)//选中状态下再次选择,在项目中定位
                            {
                                Selection.activeObject = shader;
                                EditorGUIUtility.PingObject(shader);
                            }
                            
                            mShaderViewSelectedShader = shader;
                            CollectPassKeywordMap(collectionMapper.GetShaderVariants(shader));
                        }
                        GUI.color = oriGUIColor;

                        if (GUILayout.Button("-", GUILayout.Width(20)))
                        {
                            removeShader = shader;
                        }
                        EditorGUILayout.EndHorizontal();
                    }

                    if (removeShader != null)
                    {
                        UndoShaderVariantCollectionTool();
                        collectionMapper.RemoveShader(removeShader);
                        mFilterShaders.Remove(removeShader);

                        if (removeShader == mShaderViewSelectedShader)
                        {
                            mShaderViewSelectedShader = null;
                            mPassVariantCacheData.Clear();
                        }
                        
                    }

                    EditorGUILayout.EndScrollView();
                }

                if (mCurrentFeatureState == FeatureViewState.BatchTool)
                {
                    EditorGUILayout.LabelField("Tools View");

                    for (BatchToolViewState state = (BatchToolViewState)0; state < BatchToolViewState.MaxCount; ++state)
                    {
                        Color oriStateColor = GUI.color;
                        if (state == mBatchToolViewState)
                            GUI.color = Color.green;
                        
                        if (GUILayout.Button(sBatchToolViewStateStyles[(int)state], GUILayout.ExpandWidth(true)))
                        {
                            mBatchToolViewState = state;
                        }

                        GUI.color = oriStateColor;
                    }
                        
                    // if (GUILayout.Button(, GUILayout.ExpandWidth(true)))
                    // {
                    //     mBatchToolViewState = BatchToolViewState.ExecutorList;
                    // }
                    //
                    // if (GUILayout.Button(, GUILayout.ExpandWidth(true)))
                    // {
                    //     mBatchToolViewState = BatchToolViewState.MergeFile;
                    // }
                    //
                    // if (GUILayout.Button(, GUILayout.ExpandWidth(true)))
                    // {
                    //     mBatchToolViewState = BatchToolViewState.SplitFile;
                    // }
                }
                
                if (mCurrentFeatureState == FeatureViewState.CollectionTool)
                {
                    EditorGUILayout.LabelField("Collection View");
                    
                    mOverrideFile = EditorGUILayout.Toggle("是否覆盖源文件内容", mOverrideFile);
                    if (GUILayout.Button(new GUIContent("一键收集变体", "一键收集变体"), GUILayout.ExpandWidth(true)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否开始一键收集变体", "是", "否") && ConfirmOverride())
                        {
                            var _ = CollectMaterial()
                                     && FilterMaterial()
                                     && mConverter.CollectVariant()
                                     && FilterVariant()
                                     && WriteCollectedVariantToFile();
                        }
                    }
                    
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    int materialCount = mConverter.GetMaterials().Count();
                    string materialCountStr = materialCount > 0 ? $"({materialCount})" : "";
                    if (GUILayout.Button(new GUIContent("收集材质" + materialCountStr, "利用有效收集器收集文件"), GUILayout.ExpandWidth(true)))
                    {
                        var _ = CollectMaterial() && FilterMaterial();
                    }

                    int variantCount = mConverter.GetVariantCount();
                    string variantCountStr = variantCount > 0 ? $"({variantCount})" : "";
                    if (GUILayout.Button(new GUIContent("材质变体收集" + variantCountStr, "将收集到材质的keywords转换为有效变体"), GUILayout.ExpandWidth(true)))
                    {
                        var _ = mConverter.CollectVariant() && FilterVariant();
                    }
                    
                    if (GUILayout.Button(new GUIContent("写入收集文件", "将收集到的变体写入变体收集文件"), GUILayout.ExpandWidth(true)))
                    {
                        if (EditorUtility.DisplayDialog("确认", "是否确认写入变体收集文件", "是", "否") && ConfirmOverride())
                        {
                            WriteCollectedVariantToFile();
                        }
                    }
                    
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    
                    if (GUILayout.Button(new GUIContent("材质收集器列表", "当前配置文件下的材质收集器"), GUILayout.ExpandWidth(true)))
                    {
                        mCollectionViewState = CollectionViewState.CollectorList;
                        mSelectedInterfaceImplIndex = 0;
                    }
                    
                    if (GUILayout.Button(new GUIContent("材质过滤", "根据条件排除收集到的材质"), GUILayout.ExpandWidth(true)))
                    {
                        mCollectionViewState = CollectionViewState.MaterialFilter;
                        mSelectedInterfaceImplIndex = 0;
                    }
                    
                    if (GUILayout.Button(new GUIContent("变体过滤", "根据条件排除收集到的变体"), GUILayout.ExpandWidth(true)))
                    {
                        mCollectionViewState = CollectionViewState.VariantFilter;
                        mSelectedInterfaceImplIndex = 0;
                    }
                    
                    if (GUILayout.Button(new GUIContent("材质来源检查", "查找材质被哪个收集器采集"), GUILayout.ExpandWidth(true)))
                    {
                        mCollectionViewState = CollectionViewState.MaterialFrom;
                    }
                    
                    
                }
            }
            
            EditorGUILayout.EndVertical();
            #endregion
            
            EditorGUILayout.EndVertical();
            #endregion
            
            #region 中间分隔线
            EditorGUILayout.BeginVertical(GUILayout.Width(cMiddleWidth));
            EditorGUILayout.Space(10);
            EditorGUILayout.EndVertical();
            #endregion
            
            #region 右半部分
            int rightWidth = (int)(position.width - cLeftWidth - cMiddleWidth - 10);
            EditorGUILayout.BeginVertical(mBlackStyle, GUILayout.MinWidth(rightWidth), GUILayout.MinHeight(position.height - cBorderWidth * 2));

            if (mCollectionFile != null)
            {
                #region 变体浏览
                if (mCurrentFeatureState == FeatureViewState.ShaderVariantIndex && mShaderViewSelectedShader != null)
                {
                    if (GUILayout.Button("+"))
                    {
                        OpenAddVariantWindow();
                    }
                    
                    EditorGUILayout.BeginHorizontal();
                    string newVariantFilterString =
                        EditorGUILayout.TextField(new GUIContent("keyword过滤", "用空格分割多个keyword"), mVariantFilterString);
                    if (newVariantFilterString != mVariantFilterString)
                    {
                        mVariantFilterString = newVariantFilterString;
                        if (newVariantFilterString == "<no keywords>")
                            mVariantFilterArray = new string[0];
                        else if (string.IsNullOrWhiteSpace(newVariantFilterString))
                            mVariantFilterArray = null;
                        else
                            mVariantFilterArray = newVariantFilterString.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);
                    }

                    if (mVariantFilterString != "<no keywords>")
                        mVariantFilterAllMatch = EditorGUILayout.Toggle("完全匹配", mVariantFilterAllMatch);
                    EditorGUILayout.EndHorizontal();
                    
                    //if (mPassKeywordsMap.Count == 0)
                    if (mPassVariantCacheData.Count == 0)
                    {
                        EditorGUILayout.LabelField("当前Shader没有变体被收集");
                    }
                
                    //bool modify = false;
                    //PassType modifyKey = PassType.Normal;
                    //(List<string[]> list, bool toggle) modifyValue = (null, false);
                
                    int keyowrdWidth = rightWidth - 20;
                    int minusWidth = 20;
                
                    bool removeVariant = false;
                    // PassType removePassType = PassType.Normal;
                    // string[] removeKeywords = null;
                    ShaderVariantCollection.ShaderVariant removedVariant = default;
                
                    mWorkViewScrollViewPos = EditorGUILayout.BeginScrollView(mWorkViewScrollViewPos);
                    //foreach (KeyValuePair<PassType, (List<string[]> list, bool toggle)> pair in mPassKeywordsMap)
                    foreach (CachePassData cacheData in mPassVariantCacheData)
                    {
                        //var passType = pair.Key;
                        //var keywordsListTuple = pair.Value;
                
                        cacheData.toggleValue = EditorGUILayout.Foldout(cacheData.toggleValue, $"{cacheData.passType.ToString()}({cacheData.variants.Count})");
                        // if (newToggle != keywordsListTuple.toggle)
                        // {
                        //     modify = true;
                        //     modifyKey = passType;
                        //     modifyValue = (keywordsListTuple.list, newToggle);
                        // }
                
                        if (cacheData.toggleValue)
                        {
                            foreach (SerializableShaderVariant variant in cacheData.variants)
                            {
                                //过滤功能
                                if (mVariantFilterArray != null)
                                {
                                    bool needSkipDisplay = false;
                                    foreach (var needVariant in mVariantFilterArray)
                                    {
                                        if (!variant.keywords.Contains(needVariant))
                                        {
                                            needSkipDisplay = true;
                                            break;
                                        }
                                    }

                                    needSkipDisplay |= (mVariantFilterAllMatch &&
                                                        variant.keywords.Length != mVariantFilterArray.Length);
                                    needSkipDisplay |= mVariantFilterArray.Length == 0 && variant.keywords.Length != 0;//no keywords
                                    
                                    if (needSkipDisplay)
                                        continue;
                                }

                                EditorGUILayout.BeginHorizontal(GUILayout.Width(rightWidth));
                                if(variant.keywords.Length == 0)
                                    EditorGUILayout.LabelField("<no keywords>", GUILayout.Width(keyowrdWidth));
                                else
                                    EditorGUILayout.LabelField(string.Join(", ", variant.keywords), GUILayout.Width(keyowrdWidth));
                
                                if (GUILayout.Button("-", GUILayout.Width(minusWidth)))
                                {
                                    removeVariant = true;
                                    removedVariant = variant.Deserialize();
                                    // removePassType = cacheData.passType;
                                    // removeKeywords = variant.keywords;
                                }
                                EditorGUILayout.EndHorizontal();
                            }
                            
                            if (GUILayout.Button("+"))
                            {
                                OpenAddVariantWindow(cacheData.passType);
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                
                    // if (modify)
                    //     mPassKeywordsMap[modifyKey] = modifyValue;
                
                    if (removeVariant)
                    {
                        UndoShaderVariantCollectionTool();
                        // collectionMapper.RemoveVariant(
                        //     new ShaderVariantCollection.ShaderVariant(mShaderViewSelectedShader, removePassType,
                        //         removeKeywords));
                        collectionMapper.RemoveVariant(removedVariant);
                        
                        //CollectPassKeywordMap(collectionMapper.GetShaderVariants(mShaderViewSelectedShader));
                        RefreshPassKeywordMap(mShaderViewSelectedShader);
                    }
                        
                }
                #endregion
                
                #region 项目变体收集工具F

                if (mCurrentFeatureState == FeatureViewState.CollectionTool)
                {
                    
                    #region 材质收集器列表
                    if (mCollectionViewState == CollectionViewState.CollectorList)
                    {
                        DrawInterfaceList<IMaterialCollector>("材质收集器", rightWidth);
                    }
                    #endregion
                    #region 材质收集过滤
                    else if (mCollectionViewState == CollectionViewState.MaterialFilter)
                    {
                        DrawInterfaceList<IMaterialFilter>("材质过滤器", rightWidth);
                    }
                    #endregion
                    #region 变体收集剔除
                    else if (mCollectionViewState == CollectionViewState.VariantFilter)
                    {
                        DrawInterfaceList<IVariantFilter>("变体过滤器", rightWidth);
                    }
                    #endregion
                    #region 材质来源
                    else if (mCollectionViewState == CollectionViewState.MaterialFrom)
                    {
                        mTestMaterial = EditorGUILayout.ObjectField("待检测材质", mTestMaterial, typeof(Material), true) as Material;

                        if (mConverter.GetMaterials().Count() == 0)
                        {
                            EditorGUILayout.LabelField("当前未收集到材质，请收集材质，或检查收集代码后重新收集材质");
                        }
                        else if (mTestMaterial != null)
                        {
                            var materialFrom = mConverter.GetMaterialFrom(mTestMaterial);
                            if(materialFrom.Count() == 0)
                                EditorGUILayout.LabelField("当前材质没有被收集到");
                            else
                            {
                                EditorGUILayout.LabelField("来自收集器：");
                                foreach (IMaterialCollector collector in materialFrom)
                                {
                                    if (GUILayout.Button(collector.name))
                                    {
                                        Selection.activeObject = collector;
                                        EditorGUIUtility.PingObject(collector);
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    
                }
                #endregion

                #region 批处理工具

                if (mCurrentFeatureState == FeatureViewState.BatchTool)
                {
                    #region 执行器列表
                    if (mBatchToolViewState == BatchToolViewState.ExecutorList)
                    {
                        DrawInterfaceList<IExecutable>("批处理执行器", rightWidth);
                    }
                    #endregion
                    #region 合并文件
                    else if (mBatchToolViewState == BatchToolViewState.MergeFile)
                    {
                        mOtherCollectionFile = EditorGUILayout.ObjectField("需要合并的内容文件", mOtherCollectionFile, typeof(ShaderVariantCollection), true) as ShaderVariantCollection;

                        if (GUILayout.Button("合并")
                            && mOtherCollectionFile != null
                            && mOtherCollectionFile != mCollectionFile
                            && EditorUtility.DisplayDialog("合并确认",
                                $"是否确认将{mOtherCollectionFile.name}的内容合并到{mCollectionFile.name}", "确认", "返回"))
                        {
                            collectionMapper.Merge(mOtherCollectionFile);
                        }
                    }
                    #endregion
                    #region 分割文件
                    else if (mBatchToolViewState == BatchToolViewState.SplitFile)
                    {
                        EditorGUILayout.BeginHorizontal();
                        
                        if (mSplitFileOutputPath == "")
                            EditorGUILayout.LabelField($"选择输出路径，默认不选为当前变体收集文件路径[{Path.GetDirectoryName(AssetDatabase.GetAssetPath(mCollectionFile))}]");
                        else
                            EditorGUILayout.LabelField($"当前输出路径为[{mSplitFileOutputPath}]");

                        if (GUILayout.Button("选择/改变路径"))
                        {
                            string currentPath = string.IsNullOrEmpty(mSplitFileOutputPath)
                                ? Path.GetDirectoryName(AssetDatabase.GetAssetPath(mCollectionFile))
                                : mSplitFileOutputPath;
                            mSplitFileOutputPath = EditorUtility.OpenFolderPanel("选择输出文件夹", currentPath, "") ?? mSplitFileOutputPath;

                            if (!string.IsNullOrEmpty(mSplitFileOutputPath))
                            {
                                string projectPath =
                                    Application.dataPath.Substring(0, Application.dataPath.LastIndexOf("/"));
                                string relativePath = mSplitFileOutputPath.Substring(projectPath.Length + 1);
                                mSplitFileOutputPath = relativePath;
                            }

                        }
                        EditorGUILayout.EndHorizontal();
                        
                        mSplitMode = (SplitMode)EditorGUILayout.Popup("切割模式", (int)mSplitMode, sSplitModeStr);

                        if (mSplitMode == SplitMode.VariantCount)//按照每个文件一定变体数分割
                        {
                            mSplitFileMaxVariantLimit = EditorGUILayout.IntField("每个文件最多变体数", mSplitFileMaxVariantLimit);
                        
                            EditorGUILayout.BeginHorizontal();
                            mSplitFileNoSplitShader = !EditorGUILayout.ToggleLeft(new GUIContent("是否分割同一Shader的变体",
                                "如不分割，遇到新的Shader的变体数量超标时，会另起一个变体收集文件"), !mSplitFileNoSplitShader);

                            //如果可以分割Shader，那么询问是否可以分割Pass
                            if (!mSplitFileNoSplitShader)
                            {
                                mSplitFileNoSplitPass = !EditorGUILayout.ToggleLeft("是否分割Shader下的同一Pass", !mSplitFileNoSplitPass);
                            }
                            EditorGUILayout.EndHorizontal();
                        }
                        else if (mSplitMode == SplitMode.FixedFileCount)//按照固定变体收集文件数量分割
                        {
                            mSplitFileFixedCount = EditorGUILayout.IntField("需要分割的文件数", mSplitFileFixedCount);
                        }

                        if (GUILayout.Button("开始分割"))
                        {
                            SplitCollectionFile();
                        }
                    }
                    #endregion
                }

                #endregion
            }
            
            EditorGUILayout.EndVertical();
            #endregion
            
            GUILayout.EndHorizontal();
        }

        private Dictionary<Type, Type[]> mCachedImplements = new Dictionary<Type, Type[]>();
        private Type[] GetInterfaceImplements<T>() where T : ScriptableObject
        {
            if (!mCachedImplements.TryGetValue(typeof(T), out var impls))
            {
                impls = AppDomain.CurrentDomain.GetAssemblies().SelectMany(
                    assembly => assembly.GetTypes().Where(type => 
                        type.IsSubclassOf(typeof(T)))).ToArray();
                mCachedImplements.Add(typeof(T), impls);
            }

            return impls;
        }

        private void DrawInterfaceList<T>(string objectName, float uiWidth) where T: ScriptableObject
        {
            Type[] implements = GetInterfaceImplements<T>();
            if (implements.Length == 0)
            {
                EditorGUILayout.LabelField($"未找到{typeof(T).Name}的实现类");
            }
            else
            {
                #region 添加对象
                EditorGUILayout.BeginHorizontal();
                mSelectedInterfaceImplIndex = EditorGUILayout.Popup(mSelectedInterfaceImplIndex, implements.Select(i=>i.Name).ToArray(), GUILayout.Width(uiWidth * 0.7f));
                if (GUILayout.Button($"添加{objectName}", GUILayout.Width(uiWidth * 0.3f)))
                {
                    T newObject = CreateInstance(implements[mSelectedInterfaceImplIndex]) as T;
                    newObject.name = implements[mSelectedInterfaceImplIndex].Name;
                    Undo.RegisterCreatedObjectUndo(newObject, "Create SCV tool impl object");
                    Undo.RecordObject(mConfig, "Impl object list insert");
                    mConfig.AddToggleObject(new ToggleObject()
                    {
                        obj = newObject,
                        use = true
                    });
                    AssetDatabase.AddObjectToAsset(newObject, mConfig);
                    SaveObject(mConfig);
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                
                #region 全部执行
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                var objectList = mConfig.GetToggleObjectList(typeof(T));
                bool isExecutor = (typeof(T) == typeof(IExecutable) || typeof(T).IsSubclassOf(typeof(IExecutable)));
                
                if (objectList.Count > 1 && isExecutor)
                {
                    if (GUILayout.Button(new GUIContent("全部执行", "执行所有执行器"))
                        && EditorUtility.DisplayDialog("执行确认", "是否执行全部执行器", "确认", "返回"))
                    {
                        UndoShaderVariantCollectionTool();
                        foreach (ToggleObject to in objectList)
                            if (to.use)
                                (to.obj as IExecutable).Execute(collectionMapper);
                    }
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
                #endregion

                #region 对象列表
                mWorkViewScrollViewPos = EditorGUILayout.BeginScrollView(mWorkViewScrollViewPos);
                for (int i = 0; i < objectList.Count; ++i)
                {
                    ToggleObject toggleObject = objectList[i];
                    
                    EditorGUILayout.BeginHorizontal();
                    if (GUILayout.Button("删除"))
                    {
                        Undo.RecordObject(mConfig, "Impl object remove");
                        mConfig.RemoveToggleObject(toggleObject);
                        AssetDatabase.RemoveObjectFromAsset(toggleObject.obj);
                        SaveObject(mConfig);
                    }
                    
                    toggleObject.use = EditorGUILayout.ToggleLeft("使用", toggleObject.use);
                    
                    EditorGUI.BeginChangeCheck();
                    string newName = EditorGUILayout.TextField($"{objectName}名称", toggleObject.obj.name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RecordObject(toggleObject.obj, "Change object name");
                        toggleObject.obj.name = newName;
                    }

                    if (isExecutor && GUILayout.Button("执行") && EditorUtility.DisplayDialog("执行确认", "是否执行当前执行器", "确认", "返回"))
                    {
                        UndoShaderVariantCollectionTool();
                        (toggleObject.obj as IExecutable).Execute(collectionMapper);
                    }

                    if (i != 0 && GUILayout.Button("↑", GUILayout.Width(20)))
                    {
                        Undo.RecordObject(mConfig, "Swap object list");
                        (objectList[i - 1], objectList[i]) = (objectList[i], objectList[i - 1]);
                    }
                    if (i != objectList.Count && GUILayout.Button("↓", GUILayout.Width(20)))
                    {
                        Undo.RecordObject(mConfig, "Swap object list");
                        (objectList[i], objectList[i + 1]) = (objectList[i + 1], objectList[i]);
                    }
                    
                    EditorGUILayout.EndHorizontal();
                    
                    Editor editor = GetEditor(toggleObject.obj as ScriptableObject);
                    editor.OnInspectorGUI();
                    
                    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                }
                EditorGUILayout.EndScrollView();
                #endregion
            }
        }

        private bool CollectMaterial()
        {
            return mConverter.CollectMaterial(mConfig.GetToggleObjectList(typeof(IMaterialCollector))
                .Where(co => co.use)
                .Select(co => co.obj as IMaterialCollector));
        }
        
        private bool FilterMaterial()
        {
            var materialFilters = mConfig.GetToggleObjectList(typeof(IMaterialFilter))
                .Where(mf => mf.use)
                .Select(mf => mf.obj as IMaterialFilter);

            return mConverter.FilterMaterial(materialFilters);
        }
        
        private bool FilterVariant()
        {
            var variantFilters = mConfig.GetToggleObjectList(typeof(IVariantFilter))
                .Where(vf => vf.use)
                .Select(vf => vf.obj as IVariantFilter);

            return mConverter.FilterMaterial(variantFilters);
        }

        private void OpenAddVariantWindow(PassType passType = PassType.Normal)
        {
            var window = ShaderVariantCollectionAddVariantWindow.Window;
            
            window.Setup(mShaderViewSelectedShader, passType, collectionMapper);
            window.Show();
            ShowNotification(new GUIContent("已打开添加变体窗口，如未发现请检查窗口是否被当前窗口覆盖"));
        }

        private bool ConfirmOverride()
        {
            if (!mOverrideFile)
                return true;

            return EditorUtility.DisplayDialog("覆盖确认", "是否确认覆盖原有收集文件内容", "确认", "返回");
        }

        private void FilterShader()
        {
            mFilterShaders.Clear();

            if (mFilterShaderName != "")
            {
                foreach (var shader in collectionMapper.shaders)
                {
                    if (shader.name.IndexOf(mFilterShaderName, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        mFilterShaders.Add(shader);
                    }
                }
            }
        }
        
        internal void UndoShaderVariantCollectionTool()
        {
            collectionMapper.SetSerializeFlag(true);
            Undo.RecordObject(mCollectionFile, "Change SVC tool");
            Undo.RegisterCompleteObjectUndo(collectionMapper, "Change SVC tool");
            Undo.RegisterCompleteObjectUndo(this, "Change SVC tool");
            collectionMapper.SetSerializeFlag(false);
            //Undo.FlushUndoRecordObjects();
        }

        private bool WriteCollectedVariantToFile()
        {
            try
            {
                UndoShaderVariantCollectionTool();
                
                if (mOverrideFile)
                {
                    mCollectionFile.Clear();
                }

                mConverter.WriteToShaderVariantCollectionFile(mCollectionFile);
                collectionMapper.Refresh();
                
                //收集完毕写入文件后，更新Filter列表
                FilterShader();
                
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        private Dictionary<Type, Type> mCachedEditorTypes = new Dictionary<Type, Type>();
        private Type ImplementGetEditor(Type type)
        {
            if (!mCachedEditorTypes.TryGetValue(type, out var editor))
            {
                editor = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes()
                        .Where(t => t.IsSubclassOf(typeof(ShaderVariantCollectionToolEditor))))
                    .FirstOrDefault(t => !t.IsAbstract
                                         && t.IsDefined(typeof(ShaderVariantCollectionToolEditorAttribute), false)
                                         && ((ShaderVariantCollectionToolEditorAttribute)t.GetCustomAttributes(
                                             typeof(ShaderVariantCollectionToolEditorAttribute), false)[0]).componentType ==
                                         type);

                editor = editor ?? typeof(ToolDefaultEditor);
                mCachedEditorTypes.Add(type, editor);
            }

            return editor;
        }

        private Dictionary<ScriptableObject, Editor> mCachedEditor = new Dictionary<ScriptableObject, Editor>();
        private Editor GetEditor(ScriptableObject obj)
        {
            if (!mCachedEditor.TryGetValue(obj, out var editor))
            {
                editor = Editor.CreateEditor(obj, ImplementGetEditor(obj.GetType()));
                mCachedEditor.Add(obj, editor);
            }

            return editor;
        }
        
        private void SaveObject(Object obj)
        {
            if (obj != null)
            {
                EditorUtility.SetDirty(obj);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        private Texture2D MakeTex(int width, int height, Color col)
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

        private void CreateIfDefaultConfigIsNull()
        {
            ShaderVariantCollectionToolConfig newConfig = ScriptableObject.CreateInstance<ShaderVariantCollectionToolConfig>();
            MonoScript ms = MonoScript.FromScriptableObject(newConfig);
            string scriptFilePath = AssetDatabase.GetAssetPath(ms);
            string scriptDirectoryPath = System.IO.Path.GetDirectoryName(scriptFilePath);
            string[] findResultGUID = AssetDatabase.FindAssets("t:ShaderVariantCollectionToolConfig", new string[] { scriptDirectoryPath });

            if (findResultGUID.Length == 0)
            {
                AssetDatabase.CreateAsset(newConfig, scriptDirectoryPath +"\\Default ShaderVariantCollection Tool Config.asset");
                AssetDatabase.SaveAssets();
                mConfig = newConfig;
            }
            else
            {
                ScriptableObject.DestroyImmediate(newConfig);
                mConfig = AssetDatabase.LoadAssetAtPath<ShaderVariantCollectionToolConfig>(
                    AssetDatabase.GUIDToAssetPath(findResultGUID[0]));
            }
        }

        
        private void SplitCollectionFile()
        {
            List<ShaderVariantCollection.ShaderVariant[]> splitItems =
                new List<ShaderVariantCollection.ShaderVariant[]>();

            bool splitFileNoSplitShader = mSplitFileNoSplitShader;
            bool splitFileNoSplitPass = mSplitFileNoSplitPass;

            //固定输出文件数量下，可以自由切割变体
            if (mSplitMode == SplitMode.FixedFileCount)
            {
                splitFileNoSplitShader = false;
                splitFileNoSplitPass = false;
            }

            
            //将收集文件拆分成最小的不可分割项
            foreach (Shader shader in collectionMapper.shaders)
            {
                var variants = collectionMapper.GetShaderVariants(shader);
                //如果Shader不可拆分，则Shader作为一个分割整体
                if (splitFileNoSplitShader)
                {
                    splitItems.Add(variants.ToArray());
                }
                else
                {
                    var passes = variants.GroupBy(variant => variant.passType);

                    foreach (var pass in passes)
                    {
                        //如果Pass不可拆分，则Pass作为一个分割整体
                        if (splitFileNoSplitPass)
                        {
                            splitItems.Add(pass.ToArray());
                        }
                        else
                        {
                            foreach (var variant in pass)
                            {
                                splitItems.Add(new ShaderVariantCollection.ShaderVariant[1] { variant });
                            }
                        }
                    }
                }
            }
            
            //根据长度排序
            splitItems.Sort((itemX, itemY) => itemX.Length.CompareTo(itemY.Length));

            int splitFileMaxVariantLimit = mSplitFileMaxVariantLimit;
            if (mSplitMode == SplitMode.FixedFileCount)
            {
                splitFileMaxVariantLimit = Mathf.CeilToInt((float)splitItems.Count / (float)mSplitFileFixedCount);
            }

            List<ShaderVariantCollection> outputList = new List<ShaderVariantCollection>();
            ShaderVariantCollection currentCollection = null;

            void CreateCollection()
            {
                outputList.Add(new ShaderVariantCollection());
                currentCollection = outputList.Last();
            }

            void AddToCollection(ShaderVariantCollection.ShaderVariant[] variants)
            {
                foreach (var variant in variants)
                {
                    currentCollection.Add(variant);
                }
            }

            ShaderVariantCollection.ShaderVariant[] PopItem(int index)
            {
                var item = splitItems[index];
                splitItems.RemoveAt(index);
                return item;
            }

            while (splitItems.Count > 0)
            {
                if (outputList.Count == 0)
                {
                    CreateCollection();
                }

                //如果当前收集文件是空的，将最长的不可分割项加入收集文件
                if (outputList.Last().variantCount == 0)
                {
                    //Pop last item
                    var item = PopItem(splitItems.Count - 1);
                    AddToCollection(item);
                }
                else
                {
                    int remainCapacity = splitFileMaxVariantLimit - outputList.Last().variantCount;

                    //剩余容量中能装下的最大的Item
                    int largestItemForRemainCapacity
                        = splitItems.FindIndex(item => item.Length <= remainCapacity);

                    //如果剩余的item中没有能装在当前收集文件中的，则新建收集文件
                    if (largestItemForRemainCapacity < 0)
                    {
                        CreateCollection();
                        var item = PopItem(splitItems.Count - 1);
                        AddToCollection(item);
                    }
                    else
                    {
                        var item = PopItem(largestItemForRemainCapacity);
                        AddToCollection(item);
                    }
                }
            }
            
            string originFilePath = AssetDatabase.GetAssetPath(mCollectionFile);

            string targetDirectory = string.IsNullOrEmpty(mSplitFileOutputPath)
                ? Path.GetDirectoryName(originFilePath)
                : mSplitFileOutputPath;
            string filename = Path.GetFileNameWithoutExtension(originFilePath);
            string extension = Path.GetExtension(originFilePath);

            int numberAlign = Mathf.FloorToInt(Mathf.Log10(outputList.Count)) + 1;
            string alignStr = $"_{{0:D{numberAlign}}}";

            int collectionIndex = 0;
            foreach (var newCollection in outputList)
            {
                string number = string.Format(alignStr, collectionIndex++);
                string outputPath = targetDirectory + "\\" + filename + number + extension;

                // 保存变体收集文件
                AssetDatabase.CreateAsset(newCollection, outputPath);
            }

            //当固定变体文件数量时，不足的文件数量也需要补足
            if (mSplitMode == SplitMode.FixedFileCount)
            {
                for (; collectionIndex < mSplitFileFixedCount; ++collectionIndex)
                {
                    string number = string.Format(alignStr, collectionIndex);
                    string outputPath = targetDirectory + "\\" + filename + number + extension;
                    
                    AssetDatabase.CreateAsset(new ShaderVariantCollection(), outputPath);
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            if (outputList.Count != 0)
            {
                Selection.activeObject = outputList.Last();
                EditorGUIUtility.PingObject(outputList.Last());
            }
        }
        
        public void Awake()
        {
            SetupStyle();
            CreateIfDefaultConfigIsNull();
        }

        public void OnDisable()
        {
            ShaderVariantCollectionAddVariantWindow.Window.Close();
            SaveObject(mConfig);
        }
    }
}

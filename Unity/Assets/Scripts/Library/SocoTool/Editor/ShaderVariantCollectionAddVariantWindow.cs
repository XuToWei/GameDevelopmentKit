using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Soco.ShaderVariantsCollection
{
    public class ShaderVariantCollectionAddVariantWindow : EditorWindow
    {
        private static ShaderVariantCollectionAddVariantWindow m_window;
        public static ShaderVariantCollectionAddVariantWindow Window
        {
            get
            {
                if (m_window == null)
                {
                    m_window = EditorWindow.GetWindow<ShaderVariantCollectionAddVariantWindow>("AddVariantWindow");
                    m_window.minSize = new Vector2(480, 320);
                }
                return m_window;
            }
        }

        private Shader mShader;
        private PassType mPassType;
        private ShaderVariantCollectionMapper mMapper;

        private static MethodInfo sGetShaderGlobalKeywordsMethod = null;
        private static MethodInfo sGetShaderLocalKeywordsMethod = null;

        private string[] mShaderKeywords;
        private List<string> mSelectedShaderKeywords = new List<string>();
        // private int mSelectedShaderKeywordIndex = 0;

        private enum State
        {
            None,
            Success,
            Failure
        }

        private State mState;
        private string mMessage;
        
        
        private static void InitGetKeywordMethod()
        {
            if (sGetShaderGlobalKeywordsMethod == null)
                sGetShaderGlobalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderGlobalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);

            if (sGetShaderLocalKeywordsMethod == null)
                sGetShaderLocalKeywordsMethod = typeof(ShaderUtil).GetMethod("GetShaderLocalKeywords",
                    BindingFlags.NonPublic | BindingFlags.Static);
        }
        
        public void Setup(Shader shader, PassType passType, ShaderVariantCollectionMapper mapper)
        {
            mShader = shader;
            mPassType = passType;
            mMapper = mapper;
            
            InitGetKeywordMethod();
            
            string[] globalKeywords = sGetShaderGlobalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
            string[] localKeywords = sGetShaderLocalKeywordsMethod.Invoke(null, new object[] { shader }) as string[];
            mShaderKeywords = globalKeywords.Concat(localKeywords).ToArray();
            
            mSelectedShaderKeywords.Clear();
            // mSelectedShaderKeywordIndex = 0;
            mState = State.None;
        }
        
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.LabelField($"当前Shader: {mShader}");
            mPassType = (PassType)EditorGUILayout.EnumPopup("PassType", mPassType);

            #region 待添加keyword
            EditorGUILayout.LabelField($"待添加Keyword：");
            
            EditorGUILayout.BeginHorizontal();
            int i = 0;
            foreach (string keyword in mShaderKeywords)
            {
                if (i != 0 && i % 4 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (!mSelectedShaderKeywords.Contains(keyword))
                {
                    if (GUILayout.Button(keyword))
                    {
                        mSelectedShaderKeywords.Add(keyword);
                    }
                    i++;
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion
            
            #region 当前keyword
            EditorGUILayout.LabelField($"当前Keyword：");
            
            EditorGUILayout.BeginHorizontal();
            for (int keywordIndex = 0; keywordIndex < mSelectedShaderKeywords.Count; ++keywordIndex)
            {
                if (keywordIndex != 0 && keywordIndex % 4 == 0)
                {
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                }

                if (GUILayout.Button(mSelectedShaderKeywords[keywordIndex]))
                {
                    mSelectedShaderKeywords.RemoveAt(keywordIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
            #endregion

            if (GUILayout.Button("添加变体"))
            {
                bool newVariantSuccess = false;
                ShaderVariantCollection.ShaderVariant newVariant;
                string errorMessage = "";
                
                try
                { 
                    newVariant = new ShaderVariantCollection.ShaderVariant(mShader, mPassType,
                            mSelectedShaderKeywords.ToArray());
                    newVariantSuccess = true;
                }
                catch (Exception e)
                {
                    newVariantSuccess = false;
                    errorMessage = e.Message;
                }

                string keywordString = string.Join(", ", mSelectedShaderKeywords);
                if (newVariantSuccess)
                {
                    newVariant = new ShaderVariantCollection.ShaderVariant(mShader, mPassType,
                        mSelectedShaderKeywords.ToArray());//不重新new会因为变量可能没初始化导致编译错误
                    if (mMapper.HasVariant(newVariant))
                    {
                        mState = State.Failure;
                        mMessage = $"变体<{mPassType}>[{keywordString}]已存在";
                    }
                    else
                    {
                        ShaderVariantCollectionToolsWindow.Window.UndoShaderVariantCollectionTool();
                        if (!mMapper.AddVariant(newVariant))
                        {
                            mState = State.Failure;
                            mMessage = $"变体<{mPassType}>[{keywordString}]添加失败";
                        }
                        else
                        {
                            mState = State.Success;
                            mMessage = $"变体<{mPassType}>[{keywordString}]添加成功";
                            ShaderVariantCollectionToolsWindow.Window.RefreshPassKeywordMap(mShader);
                            //ShaderVariantCollectionToolsWindow.Window.CollectPassKeywordMap(mMapper.GetShaderVariants(mShader));
                            ShaderVariantCollectionToolsWindow.Window.Repaint();
                        }
                    }
                }
                else
                {
                    mState = State.Failure;
                    mMessage = $"变体<{mPassType}>[{keywordString}]创建失败，相关报错为:{errorMessage}";
                }
            }

            #region 消息显示
            if (mState != State.None)
            {
                Color oriColor = GUI.color;
                GUI.color = mState == State.Success ? Color.green : Color.red;
                GUI.color *= 0.5f;
                
                EditorGUILayout.LabelField(mMessage, EditorStyles.whiteLabel);
                
                GUI.color = oriColor;
            }
            #endregion
            
            EditorGUILayout.EndVertical();
            
            
        }
    }
}
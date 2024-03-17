using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor;

namespace Soco.ShaderVariantsStripper
{
    public class ShaderVariantsStripperConditionAExistAndBNotExist : ShaderVariantsStripperCondition
    {
        private enum AccessLevel
        {
            Global,
            Local
        }
        
        public string keywordA;
        public string keywordB;
        
        private AccessLevel _accessLevel = AccessLevel.Global;
        private int _selectedKeywordIndex;
        private string _inputKeyword;
        
        public bool Completion(Shader shader, ShaderVariantsData data)
        {
            bool valid = keywordA != "" && keywordB != "";
            
            bool AExist = data.IsKeywordEnabled(keywordA, shader);

            bool BNotExist = !data.IsKeywordEnabled(keywordB, shader);

            return valid && AExist && BNotExist;
        }
        
        public bool EqualTo(ShaderVariantsStripperCondition other, ShaderVariantsData variantData)
        {
            return other.GetType() == typeof(ShaderVariantsStripperConditionAExistAndBNotExist) &&
                   (other as ShaderVariantsStripperConditionAExistAndBNotExist).keywordA == this.keywordA &&
                   (other as ShaderVariantsStripperConditionAExistAndBNotExist).keywordB == this.keywordB;
        }

#if UNITY_EDITOR
        public string Overview()
        {
            return $"当Keyword<{keywordA}>存在，且<{keywordB}>不存在时";
        }

        public void OnGUI(ShaderVariantsStripperConditionOnGUIContext context)
        {
            EditorGUILayout.BeginVertical();
            if (context.shader != null)
            {
                #region 选择添加
                EditorGUILayout.BeginHorizontal();
                float width = 
                    EditorGUIUtility.currentViewWidth * 0.25f;
                
                AccessLevel newAccessLevel = (AccessLevel)EditorGUILayout.Popup((int)_accessLevel, new string[] { "Global", "Local" }, GUILayout.Width(width));

                if (newAccessLevel != _accessLevel)
                {
                    _selectedKeywordIndex = 0;
                    _accessLevel = newAccessLevel;
                }

                if (_accessLevel == AccessLevel.Global && context.globalKeywords.Length > 0 ||
                    _accessLevel == AccessLevel.Local && context.localKeywords.Length > 0)
                {
                    _selectedKeywordIndex = EditorGUILayout.Popup(_selectedKeywordIndex,
                        _accessLevel == AccessLevel.Global ? context.globalKeywords : context.localKeywords, GUILayout.Width(width));
                
                    string selectedKeyword = _accessLevel == AccessLevel.Global ? context.globalKeywords[_selectedKeywordIndex] : context.localKeywords[_selectedKeywordIndex];

                    if (GUILayout.Button("设置为A", GUILayout.Width(width)))
                    {
                        keywordA = selectedKeyword;
                    }
                    if (GUILayout.Button("设置为B", GUILayout.Width(width)))
                    {
                        keywordB = selectedKeyword;
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion
                
                EditorGUILayout.Space(20);
            }
            
            #region 输入添加
            EditorGUILayout.BeginHorizontal();

            _inputKeyword = EditorGUILayout.TextField("输入Keyword", _inputKeyword, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.5f));
            if (GUILayout.Button("设置为A", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)) && _inputKeyword != null)
            {
                string[] inputKeywords = _inputKeyword.Split(' ');

                foreach (var keyword in inputKeywords)
                {
                    if (keyword != "" || _inputKeyword == "")
                    {
                        keywordA = keyword;
                    }
                }
            }
            if (GUILayout.Button("设置为B", GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.25f)) && _inputKeyword != null)
            {
                string[] inputKeywords = _inputKeyword.Split(' ');

                foreach (var keyword in inputKeywords)
                {
                    if (keyword != "" || _inputKeyword == "")
                    {
                        keywordB = keyword;
                    }
                }
            }
            
            EditorGUILayout.EndHorizontal();
            #endregion
            
            EditorGUILayout.Space(20);
            
            #region 显示Keyword
            EditorGUILayout.LabelField($"当包含Keyword<{keywordA}>,却不包含Keyword<{keywordB}>时");
            #endregion
            EditorGUILayout.EndVertical();
        }

        public string GetName()
        {
            return "KeywordA 存在 且 KeywordB不存在";
        }
#endif
    }
}
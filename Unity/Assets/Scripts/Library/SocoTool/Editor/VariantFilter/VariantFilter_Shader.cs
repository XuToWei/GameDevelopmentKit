using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;

namespace Soco.ShaderVariantsCollection
{
    
    
    public class VariantFilter_Shader : IVariantFilter
    {
        public enum Mode
        {
            Strip,
            OnlyReserveContains
        }

        public Mode mode = Mode.Strip;
        public Shader[] mShaders = new Shader[0];
        
        public override bool Filter(ShaderVariantCollection.ShaderVariant variant)
        {
            bool containShader = mShaders.Contains(variant.shader);
            return mode == Mode.Strip ? !containShader : containShader;
        }
    }

    [ShaderVariantCollectionToolEditor(typeof(VariantFilter_Shader))]
    class VariantFilter_ShaderEditor : ShaderVariantCollectionToolEditor
    {
        private bool foldoutValue = false;
        private int mOperatorIndex1 = 0;
        private int mOperatorIndex2 = 0;
        private Shader mOperatorShader = null;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            foldoutValue = EditorGUILayout.Foldout(foldoutValue, "添加删除交换操作");

            if (foldoutValue)
            {
                EditorGUILayout.BeginVertical();

                EditorGUILayout.BeginHorizontal();
                mOperatorIndex1 = EditorGUILayout.IntField("操作索引1", mOperatorIndex1);
                mOperatorIndex2 = EditorGUILayout.IntField("操作索引2", mOperatorIndex2);
                EditorGUILayout.EndHorizontal();
                mOperatorShader =  EditorGUILayout.ObjectField("添加Shader", mOperatorShader, typeof(Shader), true) as Shader;

                var targetObj = target as VariantFilter_Shader;
                
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("添加至操作索引1"))
                {
                    if (mOperatorIndex1 >= 0 && mOperatorIndex1 <= targetObj.mShaders.Length)
                    {
                        Undo.RecordObject(targetObj, "VariantFilter_Shader insert element from list");
                        List<Shader> temp = new List<Shader>(targetObj.mShaders);
                        temp.Insert(mOperatorIndex1, mOperatorShader);
                        targetObj.mShaders = temp.ToArray();
                    }
                }

                if (GUILayout.Button("删除操作索引1"))
                {
                    if (mOperatorIndex1 >= 0 && mOperatorIndex1 < targetObj.mShaders.Length)
                    {
                        Undo.RecordObject(targetObj, "VariantFilter_Shader delete shader from list");
                        List<Shader> temp = new List<Shader>(targetObj.mShaders);
                        temp.RemoveAt(mOperatorIndex1);
                        targetObj.mShaders = temp.ToArray();
                    }
                }

                if (GUILayout.Button("交换操作索引"))
                {
                    if (mOperatorIndex1 >= 0 && mOperatorIndex1 < targetObj.mShaders.Length
                                             && mOperatorIndex2 >= 0 && mOperatorIndex2 < targetObj.mShaders.Length)
                    {
                        Undo.RecordObject(targetObj, "VariantFilter_Shader swap element from list");
                        (targetObj.mShaders[mOperatorIndex1], targetObj.mShaders[mOperatorIndex2]) = (targetObj.mShaders[mOperatorIndex2], targetObj.mShaders[mOperatorIndex1]);
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
            }
        }
    }
}
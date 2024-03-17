using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Soco.ShaderVariantsStripper
{
    [System.Serializable]
    public struct ConditionPair
    {
        [SerializeReference]
        public ShaderVariantsStripperCondition condition;
        public bool strip;
        public uint priority;
    }
    
    [System.Serializable]
    public class ShaderVariantsItem
    {
        public bool applyGlobalConfig;
        public List<ConditionPair> conditionPairs;
    }

    //可序列化字典
    [System.Serializable]
    public class ShaderConditionsDictionary : Dictionary<Shader, ShaderVariantsItem>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<Shader> _keys = new List<Shader>();
        [SerializeField] private List<ShaderVariantsItem> _values = new List<ShaderVariantsItem>();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();
    
            for (var i = 0; i != Mathf.Min(_keys.Count, _values.Count); i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }
    
    [CreateAssetMenu(menuName = "Soco/ShaderVariantsStripper/Create Config")]
    public class ShaderVariantsStripperConfig : ScriptableObject
    {
        public bool mEnable = true;
        public bool mIsWhiteList = false;
        
        public List<ConditionPair> mGlobalConditions = new List<ConditionPair>();
        public ShaderConditionsDictionary mShaderConditions = new ShaderConditionsDictionary();
    }
}
using System.Collections.Generic;
using GameFramework.Localization;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game
{
    [CreateAssetMenu(menuName = "Game/Localization Dictionary", fileName = "UGFLocalizationDictionary", order = 0)]
    public class BuiltinLocalizationDictionary : SerializedScriptableObject
    {
        [SerializeField] private readonly Language m_Language;
        [SerializeField] private readonly Dictionary<string, string> m_Dictionary = new Dictionary<string, string>();

        public Language Language => m_Language;
        public Dictionary<string, string> Dictionary => m_Dictionary;
    }
}
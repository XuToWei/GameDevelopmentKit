using ET;
using GameFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using Log = UnityGameFramework.Runtime.Log;

namespace UGF
{
    public class BuiltinComponent : GameFrameworkComponent
    {
        [SerializeField]
        private GlobalConfig m_GlobalConfig;
        
        [SerializeField]
        private BuildInfo m_BuildInfo;

        [SerializeField]
        private TextAsset m_DefaultDictionaryTextAsset;

        [SerializeField]
        private BuiltinUpdateResourceForm m_UpdateResourceFormTemplate;
        
        [SerializeField]
        private BuiltinDialogForm m_DialogFormTemplate;

        public GlobalConfig GlobalConfig => this.m_GlobalConfig;
        public BuildInfo BuildInfo => m_BuildInfo;
        public BuiltinUpdateResourceForm UpdateResourceFormTemplate => m_UpdateResourceFormTemplate;
        public BuiltinDialogForm DialogFormTemplate => m_DialogFormTemplate;

        public void InitDefaultDictionary()
        {
            if (m_DefaultDictionaryTextAsset == null || string.IsNullOrEmpty(m_DefaultDictionaryTextAsset.text))
            {
                Log.Info("Default dictionary can not be found or empty.");
                return;
            }

            if (!GameEntry.Localization.ParseData(m_DefaultDictionaryTextAsset.text))
            {
                Log.Warning("Parse default dictionary failure.");
                return;
            }
        }

        public BuiltinDialogForm OpenDialogForm(BuiltinDialogParams dialogParams)
        {
            BuiltinDialogForm dialogForm = Instantiate(m_DialogFormTemplate);
            dialogForm.Open(dialogParams);
            return dialogForm;
        }

        public BuiltinUpdateResourceForm OpenUpdateResourceForm()
        {
            BuiltinUpdateResourceForm updateResourceForm = Instantiate(m_UpdateResourceFormTemplate);
            return updateResourceForm;
        }
    }
}

using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET
{
    [ChildOf]
    public sealed class UGFUIForm : Entity, IAwake<int, ETMonoUIForm>, IDestroy, ILoad
    {
        public UIForm uiForm { get; private set; }
        public int uiFormId { get; private set; }
        public Transform transform { get; private set; }
        /// <summary>
        /// 界面是否开启
        /// </summary>
        public bool isOpen => this.m_ETMonoUIForm.isOpen;
        
        private ETMonoUIForm m_ETMonoUIForm;

        [EntitySystem]
        private class UGFUIFormAwakeSystem : AwakeSystem<UGFUIForm, int, ETMonoUIForm>
        {
            protected override void Awake(UGFUIForm self, int uiFormId, ETMonoUIForm ugfETUIForm)
            {
                self.uiFormId = uiFormId;
                self.m_ETMonoUIForm = ugfETUIForm;
                self.uiForm = ugfETUIForm.UIForm;
                self.transform = ugfETUIForm.CachedTransform;
            }
        }

        [EntitySystem]
        private class UGFUIFormDestroySystem : DestroySystem<UGFUIForm>
        {
            protected override void Destroy(UGFUIForm self)
            {
                ETMonoUIForm etMonoUIForm = self.m_ETMonoUIForm;
                self.uiFormId = default;
                self.m_ETMonoUIForm = default;
                self.uiForm = default;
                self.transform = default;
                if (etMonoUIForm != default && etMonoUIForm.isOpen)
                {
                    GameEntry.UI.CloseUIForm(etMonoUIForm.UIForm);
                }
            }
        }

        [EntitySystem]
        private class UGFUIFormLoadSystem : LoadSystem<UGFUIForm>
        {
            protected override void Load(UGFUIForm self)
            {
                self.m_ETMonoUIForm.OnLoad();
            }
        }
    }
}
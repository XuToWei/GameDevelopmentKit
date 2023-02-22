using UnityEngine;

namespace ET
{
    public sealed class UGFUIForm : Entity, IAwake<int, ETMonoUIForm>, ILoad
    {
        public ETMonoUIForm etMonoUIForm
        {
            get;
            private set;
        }

        public int uiFormId
        {
            get;
            private set;
        }

        public Transform transform
        {
            get;
            private set;
        }

        public bool isOpen;

        [ObjectSystem]
        public sealed class UGFUIFormAwakeSystem: AwakeSystem<UGFUIForm, int, ETMonoUIForm>
        {
            protected override void Awake(UGFUIForm self, int uiFormId, ETMonoUIForm ugfETUIForm)
            {
                self.uiFormId = uiFormId;
                self.etMonoUIForm = ugfETUIForm;
                self.transform = self.etMonoUIForm.CachedTransform;
            }
        }
        
        [ObjectSystem]
        public sealed class UGFUIFormLoadSystem: LoadSystem<UGFUIForm>
        {
            protected override void Load(UGFUIForm self)
            {
                self.etMonoUIForm.Load();
            }
        }
    }
}

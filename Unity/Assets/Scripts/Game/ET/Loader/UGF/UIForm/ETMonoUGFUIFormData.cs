using GameFramework;

namespace ET
{
    public sealed class ETMonoUGFUIFormData : IReference
    {
        public EntityRef<UGFUIForm> UGFUIForm
        {
            get;
            private set;
        }

        public void Clear()
        {
            UGFUIForm = null;
        }

        public static ETMonoUGFUIFormData Create(UGFUIForm ugfUIForm)
        {
            ETMonoUGFUIFormData formData = ReferencePool.Acquire<ETMonoUGFUIFormData>();
            formData.UGFUIForm = ugfUIForm;
            return formData;
        }
    }
}

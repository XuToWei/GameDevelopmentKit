using GameFramework;

namespace ET
{
    public sealed class ETMonoUIFormData : IReference
    {
        public int UIFormId
        {
            get;
            private set;
        }

        public Entity ParentEntity
        {
            get;
            private set;
        }
        
        public object UserData
        {
            get;
            private set;
        }

        public void Clear()
        {
            this.UIFormId = default;
            this.ParentEntity = default;
            this.UserData = default;
        }

        public static ETMonoUIFormData Acquire(int uiFormId, Entity parentEntity, object userData)
        {
            ETMonoUIFormData formData = ReferencePool.Acquire<ETMonoUIFormData>();
            formData.UIFormId = uiFormId;
            formData.ParentEntity = parentEntity;
            formData.UserData = userData;
            return formData;
        }

        public void Release()
        {
            ReferencePool.Release(this);
        }
    }
}

namespace ET
{
    public sealed class UGFUIForm : Entity, IAwake<int, ETMonoUIForm>, ILoad
    {
        public ETMonoUIForm EtMonoUIForm
        {
            get;
            private set;
        }

        public int UIFormId
        {
            get;
            private set;
        }

        public bool IsOpen;

        [ObjectSystem]
        public sealed class UGFUIFormAwakeSystem: AwakeSystem<UGFUIForm, int, ETMonoUIForm>
        {
            protected override void Awake(UGFUIForm self, int uiFormId, ETMonoUIForm ugfETUIForm)
            {
                self.UIFormId = uiFormId;
                self.EtMonoUIForm = ugfETUIForm;
            }
        }
        
        [ObjectSystem]
        public class UGFUIFormLoadSystem: LoadSystem<UGFUIForm>
        {
            protected override void Load(UGFUIForm self)
            {
                self.EtMonoUIForm.Load();
            }
        }
    }
}

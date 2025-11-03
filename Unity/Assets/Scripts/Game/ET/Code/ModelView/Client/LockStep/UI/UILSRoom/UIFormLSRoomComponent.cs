namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public class UIFormLSRoomComponent : UGFUIForm<MonoUIFormLSRoom>, IAwake, IUGFUIFormOnOpen, IUGFUIFormOnUpdate
    {
        public int frame;
        public int predictFrame;
    }
}

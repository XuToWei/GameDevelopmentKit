namespace ET.Client
{
    [ComponentOf(typeof(UIComponent))]
    public class UIFormLSRoomComponent : UGFUIForm<MonoUIFormLSRoom>, IUGFUIFormOnOpen, IUGFUIFormOnUpdate
    {
        public int frame;
        public int predictFrame;
    }
}

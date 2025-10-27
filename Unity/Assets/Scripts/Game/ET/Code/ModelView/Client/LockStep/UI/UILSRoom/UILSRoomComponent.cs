namespace ET.Client
{
    [EnableMethod]
    [ComponentOf(typeof(UIComponent))]
    public partial class UILSRoomComponent : UGFUIForm<ETMonoUIFormLSRoom>, IUGFUIFormOnOpen, IUGFUIFormOnUpdate
    {
        public int frame;
        public int predictFrame;
    }
}

using UGF;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_EnterMain: AEvent<EventType.LoginFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.LoginFinish args)
        {
            await UIHelper.Open(scene, UIFormId.Main);
            UIHelper.Close(scene, UIFormId.Login);
        }
    }
}
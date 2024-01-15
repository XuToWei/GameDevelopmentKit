using Game;

namespace ET.Client
{
    public static partial class UnityEventHandler
    {
        [Invoke]
        public class OnApplicationPauseHandler: AInvokeHandler<OnApplicationPause>
        {
            public override void Handle(OnApplicationPause arg)
            {
                
            }
        }
        
        [Invoke]
        public class OnApplicationFocusHandler: AInvokeHandler<OnApplicationFocus>
        {
            public override void Handle(OnApplicationFocus arg)
            {
                
            }
        }
        
        [Invoke]
        public class OnShutdownHandler: AInvokeHandler<OnShutdown>
        {
            public override void Handle(OnShutdown arg)
            {
                
            }
        }
        
        [Invoke]
        public class OnCodeReloadEventHandler: AInvokeHandler<OnCodeReload>
        {
            public override void Handle(OnCodeReload args)
            {
                UnityGameFramework.Runtime.UIForm[] uiForms = GameEntry.UI.GetAllLoadedUIForms();
                foreach (var uiForm in uiForms)
                {
                    if (uiForm.Logic is ETMonoUIForm etMonoUIForm)
                    {
                        etMonoUIForm.OnReload();
                    }
                }
                UnityGameFramework.Runtime.Entity[] entities = GameEntry.Entity.GetAllLoadedEntities();
                foreach (var entity in entities)
                {
                    if (entity.Logic is ETMonoEntity etMonoEntity)
                    {
                        etMonoEntity.OnReload();
                    }
                }
            }
        }
    }
}
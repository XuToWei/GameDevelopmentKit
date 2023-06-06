using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Extension;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [Event(SceneType.LockStep)]
    public class LSSceneInitFinish_Finish: AEvent<Scene, EventType.LSSceneInitFinish>
    {
        protected override async UniTask Run(Scene clientScene, EventType.LSSceneInitFinish args)
        {
            Room room = clientScene.GetComponent<Room>();
            
            GameObject unitGo = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetPrefabAsset("Skeleton/Skeleton"));
            GameEntry.DataNode.SetData<VarGameObject>("UnitGameObject", unitGo);
            
            room.AddComponent<LSUnitViewComponent>();
            
            room.AddComponent<LSCameraComponent>();

            if (!room.IsReplay)
            {
                room.AddComponent<LSOperaComponent>();
            }

            clientScene.GetComponent<UIComponent>().CloseUIForm(UGFUIFormId.UILSLobby);
            await UniTask.CompletedTask;
        }
    }
}
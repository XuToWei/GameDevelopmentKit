using Cysharp.Threading.Tasks;
using Game;
using UnityEngine;
using UnityGameFramework.Extension;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterUnitCreate_CreateUnitView : AEvent<EventType.AfterUnitCreate>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterUnitCreate args)
        {
            Unit unit = args.Unit;
            // Unit View层
            // 这里资源需要卸载，Demo就不搞了
            GameObject go = await GameEntry.Resource.LoadAssetAsync<GameObject>(AssetUtility.GetPrefabAsset("Skeleton/Skeleton"));
            
            go = UnityEngine.Object.Instantiate(go);
            go.transform.position = unit.Position;
            unit.AddComponent<GameObjectComponent>().GameObject = go;
            unit.AddComponent<AnimatorComponent>();
            await UniTask.CompletedTask;
        }
    }
}
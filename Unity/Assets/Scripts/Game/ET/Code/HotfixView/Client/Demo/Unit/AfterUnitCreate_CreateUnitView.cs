using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityGameFramework.Runtime;
using GameEntry = Game.GameEntry;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class AfterUnitCreate_CreateUnitView: AEvent<Scene, EventType.AfterUnitCreate>
    {
        protected override async UniTask Run(Scene scene, EventType.AfterUnitCreate args)
        {
            Unit unit = args.Unit;
            // Unit View层
            // 这里资源需要卸载，Demo就不搞了
            GameObject go = GameEntry.DataNode.GetData<VarGameObject>("UnitGameObject");
            
            go = UnityEngine.Object.Instantiate(go);
            go.transform.position = unit.Position;
            unit.AddComponent<GameObjectComponent>().GameObject = go;
            unit.AddComponent<AnimatorComponent>();
            await UniTask.CompletedTask;
        }
    }
}
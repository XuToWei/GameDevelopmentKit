using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Mathematics;

namespace ET.Client
{
    public class AI_XunLuo : AAIHandler
    {
        public override int Check(AIComponent aiComponent, DRAIConfig aiConfig)
        {
            long sec = TimeHelper.ClientNow() / 1000 % 15;
            if (sec < 10)
            {
                return 0;
            }

            return 1;
        }

        public override async UniTaskVoid Execute(AIComponent aiComponent, DRAIConfig aiConfig, CancellationTokenSource cts)
        {
            Scene clientScene = aiComponent.DomainScene();

            Unit myUnit = UnitHelper.GetMyUnitFromClientScene(clientScene);
            if (myUnit == null)
            {
                return;
            }

            Log.Debug("开始巡逻");

            while (true)
            {
                XunLuoPathComponent xunLuoPathComponent = myUnit.GetComponent<XunLuoPathComponent>();
                float3 nextTarget = xunLuoPathComponent.GetCurrent();
                await myUnit.MoveToAsync(nextTarget, cts);
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                xunLuoPathComponent.MoveNext();
            }
        }
    }
}
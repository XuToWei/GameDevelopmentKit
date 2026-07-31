using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.SurvivorClient)]
    public sealed class SurvivorRoom2C_StateFrameHandler:
            MessageHandler<Scene, SurvivorRoom2C_StateFrame>
    {
        protected override async UniTask Run(Scene root, SurvivorRoom2C_StateFrame message)
        {
            root.GetComponent<SurvivorClientComponent>()
                    ?.ApplyStateFrame(
                        message.Sequence,
                        message.IsFull,
                        message.Payload);
            await UniTask.CompletedTask;
        }
    }
}

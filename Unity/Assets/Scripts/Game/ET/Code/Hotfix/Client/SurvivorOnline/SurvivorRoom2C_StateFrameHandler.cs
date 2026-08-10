using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.Survivor)]
    public sealed class SurvivorRoom2C_StateFrameHandler:
            MessageHandler<Scene, SurvivorRoom2C_StateFrame>
    {
        protected override UniTask Run(Scene root, SurvivorRoom2C_StateFrame message)
        {
            root.GetComponent<SurvivorClientComponent>().ApplyStateFrame(message.Sequence, message.IsFull, message.Payload);
            return UniTask.CompletedTask;
        }
    }
}

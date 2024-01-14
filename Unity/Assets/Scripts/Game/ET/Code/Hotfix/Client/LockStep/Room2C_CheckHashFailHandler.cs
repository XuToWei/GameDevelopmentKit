using Cysharp.Threading.Tasks;

namespace ET.Client
{
    [MessageHandler(SceneType.LockStep)]
    public class Room2C_CheckHashFailHandler: MessageHandler<Scene, Room2C_CheckHashFail>
    {
        protected override async UniTask Run(Scene root, Room2C_CheckHashFail message)
        {
            LSWorld serverWorld = MemoryPackHelper.Deserialize(typeof(LSWorld), message.LSWorldBytes, 0, message.LSWorldBytes.Length) as LSWorld;
            using (root.AddChild(serverWorld))
            {
                Log.Debug($"check hash fail, server: {message.Frame} {serverWorld.ToJson()}");
            }

            Room room = root.GetComponent<Room>();
            LSWorld clientWorld = room.GetLSWorld(SceneType.LockStepClient, message.Frame);
            using (root.AddChild(clientWorld))
            {
                Log.Debug($"check hash fail, client: {message.Frame} {clientWorld.ToJson()}");
            }
            
            await UniTask.CompletedTask;
        }
    }
}
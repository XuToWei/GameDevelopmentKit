using System.IO;
using Cysharp.Threading.Tasks;

namespace ET
{
    [Invoke]
    public class RecastFileReader: AInvokeHandler<NavmeshComponent.RecastFileLoader, UniTask<byte[]>>
    {
        public override async UniTask<byte[]> Handle(NavmeshComponent.RecastFileLoader args)
        {
            await UniTask.CompletedTask;
            // ReSharper disable once MethodHasAsyncOverload
            return File.ReadAllBytes(Path.Combine("../Config/Recast", args.Name));
        }
    }
}
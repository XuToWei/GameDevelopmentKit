using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [FriendOf(typeof(RobotCaseComponent))]
    public static partial class RobotCaseComponentSystem
    {
        public static int GetN(this RobotCaseComponent self)
        {
            return ++self.N;
        }
        
        public static async UniTask<RobotCase> New(this RobotCaseComponent self)
        {
            await UniTask.CompletedTask;
            RobotCase robotCase = self.AddChild<RobotCase>();
            return robotCase;
        }
    }
}
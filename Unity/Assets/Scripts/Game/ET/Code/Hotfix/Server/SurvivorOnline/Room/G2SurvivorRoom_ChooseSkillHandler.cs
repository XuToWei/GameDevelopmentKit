using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.SurvivorRoomRoot)]
    public sealed class G2SurvivorRoom_ChooseSkillHandler: MessageHandler<Scene, G2SurvivorRoom_ChooseSkill, SurvivorRoom2G_ChooseSkill>
    {
        protected override async UniTask Run(Scene root, G2SurvivorRoom_ChooseSkill request, SurvivorRoom2G_ChooseSkill response)
        {
            SurvivorRoom room = root.GetComponent<SurvivorRoom>();
            SurvivorWorldComponent world = room.GetComponent<SurvivorWorldComponent>();
            if (world.Data.Phase != SurvivorRoomPhase.Running)
            {
                response.Error = ErrorCode.ERR_SurvivorInvalidSkillChoice;
                response.Message = "对局已结束，无法继续选择技能";
                return;
            }

            if (!world.TryChooseSkill(request.PlayerId, request.ChoiceRevision, (SurvivorSkillType)request.SkillType))
            {
                response.Error = ErrorCode.ERR_SurvivorInvalidSkillChoice;
                response.Message = "技能选项已更新，请使用最新选项";
                return;
            }

            room.GetComponent<SurvivorRoomServerComponent>().BroadcastStateFrame(false);
            response.Message = "技能升级成功";
            await UniTask.CompletedTask;
        }
    }
}

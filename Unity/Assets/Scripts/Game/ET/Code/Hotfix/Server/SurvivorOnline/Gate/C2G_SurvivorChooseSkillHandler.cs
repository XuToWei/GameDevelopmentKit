using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public sealed class C2G_SurvivorChooseSkillHandler: MessageSessionHandler<C2G_SurvivorChooseSkill, G2C_SurvivorChooseSkill>
    {
        protected override async UniTask Run(Session session, C2G_SurvivorChooseSkill request, G2C_SurvivorChooseSkill response)
        {
            Player player = session.GetComponent<SessionPlayerComponent>().Player;
            SurvivorPlayerRoomComponent playerRoom = player.GetComponent<SurvivorPlayerRoomComponent>();
            if (playerRoom == null)
            {
                response.Error = ErrorCode.ERR_SurvivorNotInRoom;
                response.Message = "尚未加入 Survivor 房间";
                return;
            }

            using G2SurvivorRoom_ChooseSkill chooseSkillRequest = G2SurvivorRoom_ChooseSkill.Create(true);
            chooseSkillRequest.PlayerId = player.Id;
            chooseSkillRequest.SkillType = request.SkillType;
            chooseSkillRequest.ChoiceRevision = request.ChoiceRevision;
            using SurvivorRoom2G_ChooseSkill chooseSkillResponse = (SurvivorRoom2G_ChooseSkill)await session.Root().GetComponent<MessageSender>().Call(playerRoom.RoomActorId, chooseSkillRequest);
            response.Error = chooseSkillResponse.Error;
            response.Message = chooseSkillResponse.Message;
        }
    }
}

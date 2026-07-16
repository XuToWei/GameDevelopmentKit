using System;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Main)]
    public class Admin2S_KickPlayerHandler : MessageHandler<Scene, Admin2S_KickPlayerRequest, Admin2S_KickPlayerResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2S_KickPlayerRequest request, Admin2S_KickPlayerResponse response)
        {
            var innerSender = scene.GetComponent<ProcessInnerSender>();
            if (innerSender == null)
            {
                response.Error = 1;
                response.Message = "ProcessInnerSender not initialized";
                return;
            }

            var gateScenes = Tables.Instance.DTStartSceneConfig.GetByProcess(Options.Instance.Process);
            string lastError = null;

            foreach (var gateScene in gateScenes)
            {
                if (gateScene.Type != SceneType.Gate)
                {
                    continue;
                }

                try
                {
                    var gateRequest = Admin2S_KickPlayerRequest.Create();
                    gateRequest.PlayerId = request.PlayerId;
                    gateRequest.Reason = request.Reason;

                    using Admin2S_KickPlayerResponse gateResponse =
                            await innerSender.Call(gateScene.ActorId, gateRequest) as Admin2S_KickPlayerResponse;
                    if (gateResponse == null)
                    {
                        lastError = $"Gate {gateScene.Name} returned no response";
                        continue;
                    }

                    if (gateResponse.Error == 0)
                    {
                        response.Message = gateResponse.Message;
                        Log.Info($"Admin kicked player {request.PlayerId} from {gateScene.Name}: {request.Reason}");
                        return;
                    }

                    lastError = gateResponse.Message;
                }
                catch (Exception e)
                {
                    lastError = e.Message;
                    Log.Warning($"Admin kick failed on gate {gateScene.Name}: {e.Message}");
                }
            }

            response.Error = 1;
            response.Message = string.IsNullOrWhiteSpace(lastError)
                    ? $"Player {request.PlayerId} was not found on any Gate scene in process {Options.Instance.Process}"
                    : lastError;
        }
    }
}

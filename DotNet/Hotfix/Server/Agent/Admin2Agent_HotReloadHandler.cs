using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [MessageHandler(SceneType.Agent)]
    public class Admin2Agent_HotReloadHandler : MessageHandler<Scene, Admin2Agent_HotReloadRequest, Admin2Agent_HotReloadResponse>
    {
        protected override async UniTask Run(Scene scene, Admin2Agent_HotReloadRequest request, Admin2Agent_HotReloadResponse response)
        {
            var innerSender = scene.Fiber().Root.GetComponent<ProcessInnerSender>();
            if (innerSender == null)
            {
                response.Error = 1;
                response.Message = "ProcessInnerSender not initialized";
                return;
            }

            var failed = new List<int>();
            foreach (var processId in request.ProcessIds)
            {
                try
                {
                    var reloadRequest = Admin2S_ReloadRequest.Create();
                    reloadRequest.Type = request.Type;

                    var actorId = new ActorId(processId, ConstFiberId.Main);
                    var resp = await innerSender.Call(actorId, reloadRequest) as Admin2S_ReloadResponse;

                    if (resp == null || resp.Error != 0)
                    {
                        Log.Warning($"Reload ({request.Type}) failed for process {processId}: {resp?.Message}");
                        failed.Add(processId);
                    }
                    else
                    {
                        Log.Info($"Reload ({request.Type}) succeeded for process {processId}");
                    }
                }
                catch (Exception e)
                {
                    Log.Warning($"Reload ({request.Type}) error for process {processId}: {e.Message}");
                    failed.Add(processId);
                }
            }

            if (failed.Count > 0)
            {
                response.Error = 1;
                response.Message = $"Reload failed for processes: {string.Join(",", failed)}";
            }
        }
    }
}

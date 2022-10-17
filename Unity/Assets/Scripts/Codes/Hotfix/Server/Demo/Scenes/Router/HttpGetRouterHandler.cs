using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using cfg;
using cfg.ET;

namespace ET.Server
{
    [HttpHandler(SceneType.RouterManager, "/get_router")]
    public class HttpGetRouterHandler : IHttpHandler
    {
        public async ETTask Handle(Entity domain, HttpListenerContext context)
        {
            HttpGetRouterResponse response = new HttpGetRouterResponse();
            response.Realms = new List<string>();
            response.Routers = new List<string>();
            foreach (DRStartSceneConfig startSceneConfig in DataTables.Instance.DTStartSceneConfig.Realms)
            {
                response.Realms.Add(startSceneConfig.InnerIPOutPort.ToString());
            }
            foreach (DRStartSceneConfig startSceneConfig in DataTables.Instance.DTStartSceneConfig.Routers)
            {
                response.Routers.Add($"{startSceneConfig.StartProcessConfig.OuterIP}:{startSceneConfig.OuterPort}");
            }
            HttpHelper.Response(context, response);
            await ETTask.CompletedTask;
        }
    }
}

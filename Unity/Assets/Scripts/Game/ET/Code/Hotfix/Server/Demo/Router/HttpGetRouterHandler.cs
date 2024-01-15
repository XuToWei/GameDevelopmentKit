using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    [HttpHandler(SceneType.RouterManager, "/get_router")]
    public class HttpGetRouterHandler : IHttpHandler
    {
        public async UniTask Handle(Scene scene, HttpListenerContext context)
        {
            HttpGetRouterResponse response = HttpGetRouterResponse.Create();
            foreach (var startSceneConfig in Tables.Instance.DTStartSceneConfig.Realms)
            {
                response.Realms.Add(startSceneConfig.InnerIPPort.ToString());
            }
            foreach (var startSceneConfig in Tables.Instance.DTStartSceneConfig.Routers)
            {
                response.Routers.Add($"{startSceneConfig.StartProcessConfig.OuterIP}:{startSceneConfig.Port}");
            }
            HttpHelper.Response(context, response);
            await UniTask.CompletedTask;
        }
    }
}

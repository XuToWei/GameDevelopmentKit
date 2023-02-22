using System.Net;
using Cysharp.Threading.Tasks;

namespace ET.Server
{
    public interface IHttpHandler
    {
        UniTask Handle(Entity domain, HttpListenerContext context);
    }
}
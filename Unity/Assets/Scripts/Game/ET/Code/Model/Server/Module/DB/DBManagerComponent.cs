#if UNITY_EDITOR || UNITY_STANDALONE || DOTNET
namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class DBManagerComponent: Entity, IAwake
    {
    }
}
#endif
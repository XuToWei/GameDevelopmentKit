using System.Threading;
using Cysharp.Threading.Tasks;
using UnityGameFramework.Runtime;

namespace ET
{
    [ChildOf(typeof(UGFUIForm))]
    public abstract class UGFUIWidget : Entity, IAwake, IDestroy
    {
        
    }
}
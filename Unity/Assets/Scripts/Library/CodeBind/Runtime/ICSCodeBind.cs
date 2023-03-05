using UnityEngine;

namespace CodeBind
{
    public interface ICSCodeBind
    {
        CSCodeBindMono mono { get; }
        Transform transform { get; }
        void InitBind(CSCodeBindMono csCodeBindMono);
        void ClearBind();
    }
}

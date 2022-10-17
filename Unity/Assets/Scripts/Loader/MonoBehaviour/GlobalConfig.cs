using UnityEngine;

namespace ET
{
    public enum CodeMode
    {
        Client = 1,
        Server = 2,
        ClientServer = 3,
        ClientServerWhenEditor = 4,//clinet模式，编辑器下clinetserver模式
        ServerClientWhenEditor = 5,//server模式，编辑器下clinetserver模式
    }
    
    [CreateAssetMenu(menuName = "ET/GlobalConfig", fileName = "ETGlobalConfig", order = 0)]
    public class GlobalConfig: ScriptableObject
    {
        [SerializeField]
        private CodeMode m_CodeMode;
        public CodeMode CodeMode => m_CodeMode;
    }
}
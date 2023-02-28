using UnityEngine;

namespace CodeBind
{
    public static class CodeBindExtension
    {
        public static T GetCSCodeBindObject<T>(this Transform self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeBindObject<T>();
        }
        
        public static T GetCSCodeBindObject<T>(this GameObject self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeBindObject<T>();
        }
    }
}

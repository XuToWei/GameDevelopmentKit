using UnityEngine;

namespace CodeBind
{
    public static class CodeBindExtension
    {
        public static T GetCSCodeObject<T>(this Transform self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeObject<T>();
        }
        
        public static T GetCSCodeObject<T>(this GameObject self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeObject<T>();
        }
    }
}

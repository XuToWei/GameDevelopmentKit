using UnityEngine;

namespace CodeBind
{
    public static class CodeBindExtension
    {
        /// <summary>
        /// 获取绑定代码对象（自带缓存）
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetCSCodeBindObject<T>(this Transform self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeBindObject<T>();
        }

        /// <summary>
        /// 获取绑定代码对象（自带缓存）
        /// </summary>
        /// <param name="self"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetCSCodeBindObject<T>(this GameObject self) where T : ICSCodeBind, new()
        {
            return self.GetComponent<CSCodeBindMono>().GetCSCodeBindObject<T>();
        }

        /// <summary>
        /// 初始化绑定代码（非缓存池）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="gameObject"></param>
        public static void InitBind(this ICSCodeBind self, GameObject gameObject)
        {
            self.InitBind(gameObject.GetComponent<CSCodeBindMono>());
        }

        /// <summary>
        /// 初始化绑定代码（非缓存池）
        /// </summary>
        /// <param name="self"></param>
        /// <param name="transform"></param>
        public static void InitBind(this ICSCodeBind self, Transform transform)
        {
            self.InitBind(transform.GetComponent<CSCodeBindMono>());
        }
    }
}
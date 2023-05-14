using System;

namespace UnityGameFramework.Extension.Editor
{
    /// <summary>
    /// UGF Build Preprocess时候执行函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class UGFPreprocessBuildEventAttribute : Attribute
    {
        public int CallbackOrder { get; private set; }

        public UGFPreprocessBuildEventAttribute(int callbackOrder)
        {
            CallbackOrder = callbackOrder;
        }
    }
}

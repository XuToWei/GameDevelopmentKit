using System;

namespace UnityGameFramework.Extension.Editor
{
    [AttributeUsage(AttributeTargets.Method)]
    public abstract class UGFBuildEventAttribute : Attribute
    {
        public int CallbackOrder { get; private set; }

        public UGFBuildEventAttribute(int callbackOrder)
        {
            CallbackOrder = callbackOrder;
        }
    }
    
    public class UGFBuildOnPreprocessAllPlatformsAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnPreprocessAllPlatformsAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }
    
    public class UGFBuildOnPreprocessPlatformAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnPreprocessPlatformAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }

    public class UGFBuildOnBuildAssetBundlesCompleteAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnBuildAssetBundlesCompleteAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }

    public class UGFBuildOnPostprocessPlatformAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnPostprocessPlatformAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }
    
    public class UGFBuildOnOutputUpdatableVersionListDataAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnOutputUpdatableVersionListDataAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }

    public class UGFBuildOnPostprocessAllPlatformsAttribute : UGFBuildEventAttribute
    {
        public UGFBuildOnPostprocessAllPlatformsAttribute(int callbackOrder) : base(callbackOrder)
        {
        }
    }
}

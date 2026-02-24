using GameFramework;

namespace UnityGameFramework.Extension
{
    public class ResourceData : IReference
    {
        public static ResourceData Create(ISetTexture2dObject setTexture2dObject)
        {
            ResourceData resourceData = ReferencePool.Acquire<ResourceData>();
            resourceData.m_SetTexture2dObject = setTexture2dObject;
            return resourceData;
        }

        private ISetTexture2dObject m_SetTexture2dObject;

        public ISetTexture2dObject SetTexture2dObject => m_SetTexture2dObject;

        public void Clear()
        {
            m_SetTexture2dObject = null;
        }
    }
}
using GameFramework;

namespace UnityGameFramework.Extension
{
    public class ResourceData : IReference
    {
        public static ResourceData Create(ISetTexture2dObject setTexture2dObject, int serialId)
        {
            ResourceData resourceData = ReferencePool.Acquire<ResourceData>();
            resourceData.m_SetTexture2dObject = setTexture2dObject;
            resourceData.m_SerialId = serialId;
            return resourceData;
        }

        private ISetTexture2dObject m_SetTexture2dObject;
        private int m_SerialId;

        public ISetTexture2dObject SetTexture2dObject => m_SetTexture2dObject;

        public int SerialId => m_SerialId;


        public void Clear()
        {
            m_SetTexture2dObject = null;
            m_SerialId = -1;
        }
    }
}
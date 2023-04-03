using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnhancedScrollerDemos.RemoteResourcesDemo
{
    [Serializable]
    public class RemoteImageList
    {
        public RemoteImage[] images;
    }

    [Serializable]
    public class RemoteImage
    {
        public string url;
        public RemoteImageSize size;
    }

    [Serializable]
    public class RemoteImageSize
    {
        public float x;
        public float y;
    }
}

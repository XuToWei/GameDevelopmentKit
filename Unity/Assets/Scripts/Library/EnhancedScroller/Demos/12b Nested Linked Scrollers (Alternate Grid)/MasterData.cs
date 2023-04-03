using System.Collections.Generic;

namespace EnhancedScrollerDemos.NestedLinkedScrollers
{
    /// <summary>
    /// Main cell view data
    /// </summary>
    public class MasterData
    {
        // This value will store the position of the detail scroller to be used 
        // when the scroller's cell view is recycled
        public float normalizedScrollPosition;

        public float linkedScrollPosition;

        public List<DetailData> childData;
    }
}

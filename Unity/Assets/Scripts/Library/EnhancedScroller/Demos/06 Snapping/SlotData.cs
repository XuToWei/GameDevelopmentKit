using UnityEngine;
using System.Collections;

namespace EnhancedScrollerDemos.SnappingDemo
{
    /// <summary>
    /// This class represents a slot cell
    /// </summary>
    public class SlotData
    {
        /// <summary>
        /// The preloaded sprite for the slot cell. 
        /// We could have loaded the sprite while scrolling,
        /// but since there are so few slot cell types, we'll
        /// just preload them to speed up the in-game processing.
        /// </summary>
        public Sprite sprite;
    }
}
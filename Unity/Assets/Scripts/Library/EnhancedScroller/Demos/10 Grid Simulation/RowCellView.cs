using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using EnhancedUI;
using System;

namespace EnhancedScrollerDemos.GridSimulation
{
    /// <summary>
    /// This is the sub cell of the row cell
    /// </summary>
    public class RowCellView : MonoBehaviour
    {
        public GameObject container;
        public Text text;

        /// <summary>
        /// This function just takes the Demo data and displays it
        /// </summary>
        /// <param name="data"></param>
        public void SetData(Data data)
        {
            // this cell was outside the range of the data, so we disable the container.
            // Note: We could have disable the cell gameobject instead of a child container,
            // but that can cause problems if you are trying to get components (disabled objects are ignored).
            container.SetActive(data != null);

            if (data != null)
            {
                // set the text if the cell is inside the data range
                text.text = data.someText;
            }
        }
    }
}
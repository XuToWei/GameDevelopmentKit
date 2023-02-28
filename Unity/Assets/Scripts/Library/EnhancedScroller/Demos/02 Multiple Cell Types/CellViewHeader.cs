using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using EnhancedUI.EnhancedScroller;

namespace EnhancedScrollerDemos.MultipleCellTypesDemo
{
    /// <summary>
    /// This is the view for the header cells
    /// </summary>
    public class CellViewHeader : CellView
    {
        /// <summary>
        /// An internal reference to the header data. We could have just
        /// used the base CellView's class member _data, but that would
        /// require us to cast it each time a header data field is needed.
        /// By referencing the header data, we can save some time accessing
        /// the fields.
        /// </summary>
        private HeaderData _headerData;

        /// <summary>
        /// A link to the Unity UI Text object to show the category
        /// </summary>
        public Text categoryText;

        /// <summary>
        /// Override of the base class's SetData function. This links the data
        /// and updates the UI
        /// </summary>
        /// <param name="data"></param>
        public override void SetData(Data data)
        {
            // call the base SetData to link to the underlying _data
            base.SetData(data);

            // cast the data as headerData and store the reference
            _headerData = data as HeaderData;

            // update the Category UI Text field with the data
            categoryText.text = _headerData.category;
        }
    }
}
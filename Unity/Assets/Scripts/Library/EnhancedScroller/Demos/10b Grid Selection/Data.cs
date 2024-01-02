namespace EnhancedScrollerDemos.GridSelection
{
    public delegate void SelectedChangedDelegate(bool val);

    /// <summary>
    /// Data class to store information
    /// </summary>
    public class Data
    {
        public string someText;

        public SelectedChangedDelegate selectedChanged;

        /// <summary>
        /// The selection state
        /// </summary>
        private bool _selected;
        public bool Selected
        {
            get { return _selected; }
            set
            {
                // if the value has changed
                if (_selected != value)
                {
                    // update the state and call the selection handler if it exists
                    _selected = value;
                    if (selectedChanged != null) selectedChanged(_selected);
                }
            }
        }
    }
}
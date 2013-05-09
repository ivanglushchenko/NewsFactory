using NewsFactory.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.Foundation.Model
{
    [Bindable]
    public partial class Group : BindableBase
    {
        #region Properties

        /// <summary>
        /// Gets/sets Key.
        /// </summary>
        public string Title
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Key; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Key != value)
                {
                    p_Key = value;
                    OnKeyChanged();
                }
            }
        }
        private string p_Key;
        partial void OnKeyChanged();

        /// <summary>
        /// Gets/sets NewsFeeds.
        /// </summary>
        public ObservableCollection<NewsFeed> NewsFeeds
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_NewsFeeds; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_NewsFeeds != value)
                {
                    p_NewsFeeds = value;
                    OnNewsFeedsChanged();
                }
            }
        }
        private ObservableCollection<NewsFeed> p_NewsFeeds;
        partial void OnNewsFeedsChanged();

        /// <summary>
        /// Gets/sets IsSelected.
        /// </summary>
        public bool IsSelected
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsSelected; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsSelected != value)
                {
                    p_IsSelected = value;
                    OnPropertyChanged("IsSelected");
                    OnIsSelectedChanged();
                }
            }
        }
        private bool p_IsSelected;
        partial void OnIsSelectedChanged();

        #endregion Properties

        #region Methods

        partial void OnIsSelectedChanged()
        {
            if (NewsFeeds != null)
            {
                foreach (var item in NewsFeeds)
                {
                    item.IsSelected = IsSelected;
                }
            }
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Utils;
using NewsFactory.UI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.UI.Pages.ImportFeeds
{
    public partial class FeedModel : PageModel
    {
        #region .ctors

        public FeedModel(ImportFeedsViewModel model)
        {
            _model = model;
        }

        #endregion .ctors

        #region Fields

        private ImportFeedsViewModel _model;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets Title.
        /// </summary>
        public string Title
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Title; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Title != value)
                {
                    p_Title = value;
                    OnPropertyChanged("Title");
                    OnTitleChanged();
                }
            }
        }
        private string p_Title;
        partial void OnTitleChanged();

        /// <summary>
        /// Gets/sets Url.
        /// </summary>
        public string Url
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Url; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Url != value)
                {
                    p_Url = value;
                    OnPropertyChanged("Url");
                    OnUrlChanged();
                }
            }
        }
        private string p_Url;
        partial void OnUrlChanged();

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
            if (IsSelected)
                _model.FeedsCount++;
            else
                _model.FeedsCount--;
            _model.AddCommand.RaiseCanExecuteChanged();
        }

        public FeedInfo ToFeedInfo()
        {
            return new FeedInfo() { Url = Url.ToUri(), Title = Title };
        }

        #endregion Methods
    }
}

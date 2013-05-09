using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.AppSettings
{
    public partial class AppSettingsPageModel : PageModel
    {
        #region .ctors

        public AppSettingsPageModel()
        {
            ResetAllFeedsCommand = new DelegateCommand(ResetAllFeeds);
        }

        #endregion .ctors

        #region Properties

        /// <summary>
        /// Gets/sets ResetAllFeedsCommand.
        /// </summary>
        public DelegateCommand ResetAllFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ResetAllFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ResetAllFeedsCommand != value)
                {
                    p_ResetAllFeedsCommand = value;
                    OnPropertyChanged("ResetAllFeedsCommand");
                    OnResetAllFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_ResetAllFeedsCommand;
        partial void OnResetAllFeedsCommandChanged();

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            base.Dispose();
            DataService.SaveSettings();
        }

        private void ResetAllFeeds()
        {
            foreach (var item in DataService.FeedsStore.NewsFeeds)
            {
                item.FeedInfo.LastPub = DateTime.MinValue;
            }
        }

        #endregion Methods
    }
}

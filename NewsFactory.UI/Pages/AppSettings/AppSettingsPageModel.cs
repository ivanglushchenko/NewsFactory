using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Controls;
using NewsFactory.Tasks;
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
            RefreshAllIconsCommand = new DelegateCommand(RefreshAllIcons);
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

        /// <summary>
        /// Gets/sets RefreshAllIconsCommand.
        /// </summary>
        public DelegateCommand RefreshAllIconsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_RefreshAllIconsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_RefreshAllIconsCommand != value)
                {
                    p_RefreshAllIconsCommand = value;
                    OnPropertyChanged("RefreshAllIconsCommand");
                    OnRefreshAllIconsCommandChanged();
                }
            }
        }
        private DelegateCommand p_RefreshAllIconsCommand;
        partial void OnRefreshAllIconsCommandChanged();

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            base.Dispose();
            DataService.SaveSettings();

            DownloadFeedTask.RegisterBackgroundTask(Settings.SecondaryTileUpdateInterval);
        }

        private void ResetAllFeeds()
        {
            foreach (var item in DataService.FeedsStore.NewsFeeds)
            {
                item.FeedInfo.LastPub = DateTime.MinValue;
            }
        }

        void RefreshAllIcons()
        {
            DataService.NewsStore.RefreshAllIcons();
        }

        #endregion Methods
    }
}

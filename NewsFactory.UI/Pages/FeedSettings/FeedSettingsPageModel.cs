using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.FeedSettings
{
    [Bindable]
    public partial class FeedSettingsPageModel : PageModel
    {
        #region .ctors

        public FeedSettingsPageModel()
        {
            RefreshFeedIconCommand = new DelegateCommand(RefreshFeedIcon);
            ResetLastUpdateDateCommand = new DelegateCommand(ResetLastUpdateDate);
            DeleteFeedCommand = new DelegateCommand(DeleteFeed);
            Categories = Enum.GetValues(typeof(Category)).OfType<Category>().ToList();
        }

        #endregion .ctors

        #region Properties

        /// <summary>
        /// Gets/sets Feed.
        /// </summary>
        public NewsFeed Feed
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Feed; }
            [System.Diagnostics.DebuggerStepThrough]
            private set
            {
                if (p_Feed != value)
                {
                    p_Feed = value;
                    OnPropertyChanged("Feed");
                    OnFeedChanged();
                }
            }
        }
        private NewsFeed p_Feed;
        partial void OnFeedChanged();


        /// <summary>
        /// Gets/sets RefreshFeedIconCommand.
        /// </summary>
        public DelegateCommand RefreshFeedIconCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_RefreshFeedIconCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_RefreshFeedIconCommand != value)
                {
                    p_RefreshFeedIconCommand = value;
                    OnPropertyChanged("RefreshFeedIconCommand");
                    OnRefreshFeedIconCommandChanged();
                }
            }
        }
        private DelegateCommand p_RefreshFeedIconCommand;
        partial void OnRefreshFeedIconCommandChanged();

        /// <summary>
        /// Gets/sets ResetLastUpdateDateCommand.
        /// </summary>
        public DelegateCommand ResetLastUpdateDateCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ResetLastUpdateDateCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ResetLastUpdateDateCommand != value)
                {
                    p_ResetLastUpdateDateCommand = value;
                    OnPropertyChanged("ResetLastUpdateDateCommand");
                    OnResetLastUpdateDateCommandChanged();
                }
            }
        }
        private DelegateCommand p_ResetLastUpdateDateCommand;
        partial void OnResetLastUpdateDateCommandChanged();

        /// <summary>
        /// Gets/sets DeleteFeedCommand.
        /// </summary>
        public DelegateCommand DeleteFeedCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteFeedCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteFeedCommand != value)
                {
                    p_DeleteFeedCommand = value;
                    OnPropertyChanged("DeleteFeedCommand");
                    OnDeleteFeedCommandChanged();
                }
            }
        }
        private DelegateCommand p_DeleteFeedCommand;
        partial void OnDeleteFeedCommandChanged();

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
        /// Gets/sets Categories.
        /// </summary>
        public List<Category> Categories
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Categories; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Categories != value)
                {
                    p_Categories = value;
                    OnPropertyChanged("Categories");
                    OnCategoriesChanged();
                }
            }
        }
        private List<Category> p_Categories;
        partial void OnCategoriesChanged();

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            base.Dispose();

            var url = Url.ToUri();
            if (url != null)
                Feed.FeedInfo.Url = url;
        }

        public void SetFeed(NewsFeed feed)
        {
            Feed = feed;
            Url = Feed.FeedInfo.Url.ToString();
        }

        private async void RefreshFeedIcon()
        {
            Feed.FeedInfo.FavIconUrl = await Feed.GetFavIcon();
            await DataService.FeedsStore.Save();
        }

        private void ResetLastUpdateDate()
        {
            Feed.FeedInfo.LastPub = DateTime.MinValue;
        }

        private async void DeleteFeed()
        {
            if (await MsgService.Ask(string.Format("The '{0}' feed is going to be deleted. Continue?", Feed.FeedInfo.Title), Buttons.YesNoCancel) == Buttons.Yes)
            {
                IsBusy = true;

                await DataService.NewsStore.DeleteFeed(Feed);

                NavigationService.GoBack();
            }
        }

        #endregion Methods
    }
}
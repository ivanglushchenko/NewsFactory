using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Common;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using NewsFactory.Tasks;
using NewsFactory.UI.Pages.AppSettings;
using NewsFactory.UI.Pages.FeedSettings;
using NewsFactory.UI.Pages.PrivacyPolicy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.Feed
{
    [Bindable]
    public partial class FeedPageModel : PageModel
    {
        #region .ctors

        public FeedPageModel()
        {
            DeleteFeedCommand = new DelegateCommand(DeleteFeed);
            RefreshFeedCommand = new DelegateCommand(RefreshFeed);
            MarkAllAsReadCommand = new DelegateCommand(MarkAllAsRead);
            DeleteAllCommand = new DelegateCommand(DeleteAllNews);
            OpenArticleCommand = new DelegateCommand(OpenArticle);
            LikeCommand = new DelegateCommand(Like);
            DislikeCommand = new DelegateCommand(Dislike);
            ReadLaterCommand = new DelegateCommand(ReadLater);
            MarkAsFavoriteCommand = new DelegateCommand(MarkAsFavorite);
            UnmarkAsFavoriteCommand = new DelegateCommand(UnmarkAsFavorite);
            GoToPrevItemCommand = new DelegateCommand(GoToPrevItem);
            GoToNextItemCommand = new DelegateCommand(GoToNextItem);
            CopyNewsItemUrlToClipboardCommand = new DelegateCommand(CopyNewsItemUrlToClipboard);
            CopyNewsFeedUrlToClipboardCommand = new DelegateCommand(CopyNewsFeedUrlToClipboard);
            PinCommand = new DelegateCommand<Rect>(Pin);
            UnpinCommand = new DelegateCommand<Rect>(Unpin);

            AddSettingsPane<AppSettingsPage>("Settings");
            AddSettingsPane("Edit current feed", () =>
            {
                var p = new FeedSettingsPage();
                p.Model.SetFeed(Feed);
                return p;
            });
            AddSettingsPane<PrivacyPolicyView>("Privacy policy");

            Now = DateTime.Now;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMinutes(1);
            _timer.Tick += t_Tick;
            _timer.Start();
        }

        #endregion .ctors

        #region Fields

        private DispatcherTimer _timer;

        #endregion Fields

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
        /// Gets/sets Items.
        /// </summary>
        public ObservableCollection<NewsItem> Items
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Items; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Items != value)
                {
                    p_Items = value;
                    OnPropertyChanged("Items");
                    OnItemsChanged();
                }
            }
        }
        private ObservableCollection<NewsItem> p_Items;
        partial void OnItemsChanged();

        /// <summary>
        /// Gets/sets SelectedItem.
        /// </summary>
        public NewsItem SelectedItem
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SelectedItem; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SelectedItem != value)
                {
                    p_SelectedItem = value;
                    OnPropertyChanged("SelectedItem");
                    OnSelectedItemChanged();
                }
            }
        }
        private NewsItem p_SelectedItem;
        partial void OnSelectedItemChanged();

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
        /// Gets/sets RefreshFeedCommand.
        /// </summary>
        public DelegateCommand RefreshFeedCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_RefreshFeedCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_RefreshFeedCommand != value)
                {
                    p_RefreshFeedCommand = value;
                    OnPropertyChanged("RefreshFeedCommand");
                    OnRefreshFeedCommandChanged();
                }
            }
        }
        private DelegateCommand p_RefreshFeedCommand;
        partial void OnRefreshFeedCommandChanged();

        /// <summary>
        /// Gets/sets MarkAllAsReadCommand.
        /// </summary>
        public DelegateCommand MarkAllAsReadCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_MarkAllAsReadCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_MarkAllAsReadCommand != value)
                {
                    p_MarkAllAsReadCommand = value;
                    OnPropertyChanged("MarkAllAsReadCommand");
                    OnMarkAllAsReadCommandChanged();
                }
            }
        }
        private DelegateCommand p_MarkAllAsReadCommand;
        partial void OnMarkAllAsReadCommandChanged();

        /// <summary>
        /// Gets/sets DeleteAllCommand.
        /// </summary>
        public DelegateCommand DeleteAllCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteAllCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteAllCommand != value)
                {
                    p_DeleteAllCommand = value;
                    OnPropertyChanged("DeleteAllCommand");
                    OnDeleteAllCommandChanged();
                }
            }
        }
        private DelegateCommand p_DeleteAllCommand;
        partial void OnDeleteAllCommandChanged();

        /// <summary>
        /// Gets/sets OpenArticleCommand.
        /// </summary>
        public DelegateCommand OpenArticleCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_OpenArticleCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_OpenArticleCommand != value)
                {
                    p_OpenArticleCommand = value;
                    OnPropertyChanged("OpenArticleCommand");
                    OnOpenArticleCommandChanged();
                }
            }
        }
        private DelegateCommand p_OpenArticleCommand;
        partial void OnOpenArticleCommandChanged();

        /// <summary>
        /// Gets/sets LikeCommand.
        /// </summary>
        public DelegateCommand LikeCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_LikeCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_LikeCommand != value)
                {
                    p_LikeCommand = value;
                    OnPropertyChanged("LikeCommand");
                    OnLikeCommandChanged();
                }
            }
        }
        private DelegateCommand p_LikeCommand;
        partial void OnLikeCommandChanged();

        /// <summary>
        /// Gets/sets DislikeCommand.
        /// </summary>
        public DelegateCommand DislikeCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DislikeCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DislikeCommand != value)
                {
                    p_DislikeCommand = value;
                    OnPropertyChanged("DislikeCommand");
                    OnDislikeCommandChanged();
                }
            }
        }
        private DelegateCommand p_DislikeCommand;
        partial void OnDislikeCommandChanged();

        /// <summary>
        /// Gets/sets ReadLaterCommand.
        /// </summary>
        public DelegateCommand ReadLaterCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ReadLaterCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ReadLaterCommand != value)
                {
                    p_ReadLaterCommand = value;
                    OnPropertyChanged("ReadLaterCommand");
                    OnReadLaterCommandChanged();
                }
            }
        }
        private DelegateCommand p_ReadLaterCommand;
        partial void OnReadLaterCommandChanged();

        /// <summary>
        /// Gets/sets MarkAsFavoriteCommand.
        /// </summary>
        public DelegateCommand MarkAsFavoriteCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_MarkAsFavoriteCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_MarkAsFavoriteCommand != value)
                {
                    p_MarkAsFavoriteCommand = value;
                    OnPropertyChanged("MarkAsFavoriteCommand");
                    OnMarkAsFavoriteCommandChanged();
                }
            }
        }
        private DelegateCommand p_MarkAsFavoriteCommand;
        partial void OnMarkAsFavoriteCommandChanged();

        public bool UseWebView
        {
            get
            {
                return DataService.Settings.UseWebView;
            }
        }

        /// <summary>
        /// Gets/sets Now.
        /// </summary>
        public DateTime Now
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Now; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Now != value)
                {
                    p_Now = value;
                    OnPropertyChanged("Now");
                    OnNowChanged();
                }
            }
        }
        private DateTime p_Now;
        partial void OnNowChanged();

        /// <summary>
        /// Gets/sets GoToNextItemCommand.
        /// </summary>
        public DelegateCommand GoToNextItemCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_GoToNextItemCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_GoToNextItemCommand != value)
                {
                    p_GoToNextItemCommand = value;
                    OnPropertyChanged("GoToNextItemCommand");
                    OnGoToNextItemCommandChanged();
                }
            }
        }
        private DelegateCommand p_GoToNextItemCommand;
        partial void OnGoToNextItemCommandChanged();

        /// <summary>
        /// Gets/sets GoToPrevItemCommand.
        /// </summary>
        public DelegateCommand GoToPrevItemCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_GoToPrevItemCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_GoToPrevItemCommand != value)
                {
                    p_GoToPrevItemCommand = value;
                    OnPropertyChanged("GoToPrevItemCommand");
                    OnGoToPrevItemCommandChanged();
                }
            }
        }
        private DelegateCommand p_GoToPrevItemCommand;
        partial void OnGoToPrevItemCommandChanged();

        /// <summary>
        /// Gets/sets CopyNewsItemUrlToClipboardCommand.
        /// </summary>
        public DelegateCommand CopyNewsItemUrlToClipboardCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_CopyNewsItemUrlToClipboardCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_CopyNewsItemUrlToClipboardCommand != value)
                {
                    p_CopyNewsItemUrlToClipboardCommand = value;
                    OnPropertyChanged("CopyNewsItemUrlToClipboardCommand");
                    OnCopyNewsItemUrlToClipboardCommandChanged();
                }
            }
        }
        private DelegateCommand p_CopyNewsItemUrlToClipboardCommand;
        partial void OnCopyNewsItemUrlToClipboardCommandChanged();

        /// <summary>
        /// Gets/sets CopyNewsFeedUrlToClipboardCommand.
        /// </summary>
        public DelegateCommand CopyNewsFeedUrlToClipboardCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_CopyNewsFeedUrlToClipboardCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_CopyNewsFeedUrlToClipboardCommand != value)
                {
                    p_CopyNewsFeedUrlToClipboardCommand = value;
                    OnPropertyChanged("CopyNewsFeedUrlToClipboardCommand");
                    OnCopyNewsFeedUrlToClipboardCommandChanged();
                }
            }
        }
        private DelegateCommand p_CopyNewsFeedUrlToClipboardCommand;
        partial void OnCopyNewsFeedUrlToClipboardCommandChanged();

        /// <summary>
        /// Gets/sets IsReadLaterFeed.
        /// </summary>
        public bool IsReadLaterFeed
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsReadLaterFeed; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsReadLaterFeed != value)
                {
                    p_IsReadLaterFeed = value;
                    OnPropertyChanged("IsReadLaterFeed");
                    OnIsReadLaterFeedChanged();
                }
            }
        }
        private bool p_IsReadLaterFeed;
        partial void OnIsReadLaterFeedChanged();

        /// <summary>
        /// Gets/sets IsFavoritesFeed.
        /// </summary>
        public bool IsFavoritesFeed
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsFavoritesFeed; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsFavoritesFeed != value)
                {
                    p_IsFavoritesFeed = value;
                    OnPropertyChanged("IsFavoritesFeed");
                    OnIsFavoritesFeedChanged();
                }
            }
        }
        private bool p_IsFavoritesFeed;
        partial void OnIsFavoritesFeedChanged();

        /// <summary>
        /// Gets/sets UnmarkAsFavoriteCommand.
        /// </summary>
        public DelegateCommand UnmarkAsFavoriteCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_UnmarkAsFavoriteCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_UnmarkAsFavoriteCommand != value)
                {
                    p_UnmarkAsFavoriteCommand = value;
                    OnPropertyChanged("UnmarkAsFavoriteCommand");
                    OnUnmarkAsFavoriteCommandChanged();
                }
            }
        }
        private DelegateCommand p_UnmarkAsFavoriteCommand;
        partial void OnUnmarkAsFavoriteCommandChanged();

        /// <summary>
        /// Gets/sets IsPinned.
        /// </summary>
        public bool IsPinned
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsPinned; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsPinned != value)
                {
                    p_IsPinned = value;
                    OnPropertyChanged("IsPinned");
                    OnIsPinnedChanged();
                }
            }
        }
        private bool p_IsPinned;
        partial void OnIsPinnedChanged();

        /// <summary>
        /// Gets/sets PinCommand.
        /// </summary>
        public DelegateCommand<Rect> PinCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_PinCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_PinCommand != value)
                {
                    p_PinCommand = value;
                    OnPropertyChanged("PinCommand");
                    OnPinCommandChanged();
                }
            }
        }
        private DelegateCommand<Rect> p_PinCommand;
        partial void OnPinCommandChanged();

        /// <summary>
        /// Gets/sets UnpinCommand.
        /// </summary>
        public DelegateCommand<Rect> UnpinCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_UnpinCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_UnpinCommand != value)
                {
                    p_UnpinCommand = value;
                    OnPropertyChanged("UnpinCommand");
                    OnUnpinCommandChanged();
                }
            }
        }
        private DelegateCommand<Rect> p_UnpinCommand;
        partial void OnUnpinCommandChanged();

        #endregion Properties

        #region Methods

        public override void Dispose()
        {
            base.Dispose();

            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= t_Tick;
                _timer = null;
            }
            DataService.NewsStore.ItemsReceived -= NewsStore_ItemsReceived;
        }

        public void SetFeed(NewsFeed feed)
        {
            Feed = feed;
            
            IsReadLaterFeed = feed == DataService.FeedsStore.ReadLater;
            IsFavoritesFeed = feed == DataService.FeedsStore.Favorites;

            Items = DataService.NewsStore.GetItems(feed);
            SelectedItem = Items.FirstOrDefault();

            //var wordIndex = new WordIndex();
            //wordIndex.AddRange(Items);

            try
            {
                IsPinned = SecondaryTile.Exists(feed.Id);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        public void SetFeedUrl(string urlStr)
        {
            Initialize(() =>
                {
                    try
                    {
                        var url = UriHelper.ToUri(urlStr);
                        var feed = DataService.FeedsStore.NewsFeedsMap[url];
                        SetFeed(feed);
                        RefreshFeed();

                        try
                        {
                            var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(feed.Id);
                            updater.EnableNotificationQueue(false);
                            updater.Clear();

                            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(feed.Id).Clear();
                        }
                        catch
                        {
                        }
                    }
                    catch (Exception exc)
                    {
                        LogService.Error(exc);

                        NavigationService.NavigateTo<MainPage>();
                    }
                });
        }

        public async override void SaveState()
        {
            await DataService.NewsStore.SaveFeed(Feed);
        }

        private void Initialize(Action completed)
        {
            if (DataService.FeedsStore == null || DataService.NewsStore == null)
            {
                Status = "Loading feeds & news, please wait";

                GeneralHelper.Run(async () =>
                {
                    await DataService.LoadStores();
                    await DataService.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        Status = null;
                        DataService.NewsStore.ItemsReceived += NewsStore_ItemsReceived;
                        completed();
                    });
                });
            }
            else
            {
                DataService.NewsStore.ItemsReceived += NewsStore_ItemsReceived;
                completed();
            }
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

        partial void OnSelectedItemChanged()
        {
            if (SelectedItem != null && SelectedItem.IsNew)
            {
                DataService.NewsStore.MarkAsRead(SelectedItem);
            }
        }

        private async void RefreshFeed()
        {
            IsBusy = true;
            await DataService.NewsStore.UpdateOne(Feed.FeedInfo);
            IsBusy = false;
        }

        private void MarkAllAsRead()
        {
            DataService.NewsStore.MarkAsRead(Feed);
        }

        private async void DeleteAllNews()
        {
            if (IsFavoritesFeed)
            {
                DataService.NewsStore.RemoveAllFavorites();
                Items.Clear();
            }
            else if (IsReadLaterFeed)
            {
                DataService.NewsStore.RemoveAllReadLater();
                Items.Clear();
            }
            else
            {
                IsBusy = true;
                await DataService.NewsStore.DeleteFeedNewsItems(Feed);
                Items.Clear();
                SelectedItem = null;
                IsBusy = false;
            }
        }

        private async void OpenArticle()
        {
            if (SelectedItem != null)
                await Launcher.LaunchUriAsync(SelectedItem.Url);
        }

        private void Like()
        {
            if (SelectedItem == null) return;

            SelectedItem.IsLike = !SelectedItem.IsLike;
            if (SelectedItem.IsLike)
                SelectedItem.IsDislike = false;

            DataService.NewsStore.MarkAsLike(SelectedItem);
        }

        private void Dislike()
        {
            if (SelectedItem == null) return;

            SelectedItem.IsDislike = !SelectedItem.IsDislike;
            if (SelectedItem.IsDislike)
                SelectedItem.IsLike = false;

            DataService.NewsStore.MarkAsDislike(SelectedItem);
        }

        private void ReadLater()
        {
            if (SelectedItem == null) return;

            SelectedItem.IsReadLater = true;
        }

        private void MarkAsFavorite()
        {
            if (SelectedItem == null) return;

            SelectedItem.IsFavorite = true;
        }

        async void t_Tick(object sender, object e)
        {
            await DataService.Invoke(Windows.UI.Core.CoreDispatcherPriority.Low, () => Now = DateTime.Now);
        }

        private void GoToPrevItem()
        {
            if (SelectedItem == null) return; 
            
            var i = Items.IndexOf(SelectedItem) - 1;
            while (i > 0 && Items[i].IsChildNewsItem)
                i--;

            if (i > 0)
            {
                SelectedItem = Items[i];
            }
        }

        private void GoToNextItem()
        {
            if (SelectedItem == null) return;

            var i = Items.IndexOf(SelectedItem) + 1;
            while (i < Items.Count + 1 && Items[i].IsChildNewsItem)
                i++;

            if (i < Items.Count - 1)
            {
                SelectedItem = Items[i];
            }
        }


        private void CopyNewsItemUrlToClipboard()
        {
            if (SelectedItem == null) return;

            GeneralHelper.CopyToClipboard(SelectedItem.Url.ToString());
        }

        private void CopyNewsFeedUrlToClipboard()
        {
            GeneralHelper.CopyToClipboard(Feed.FeedInfo.Url.ToString());
        }

        private void UnmarkAsFavorite()
        {
            if (SelectedItem == null) return;

            if (SelectedItem.IsFavorite == true)
            {
                SelectedItem.IsFavorite = false;
                if (Items.Contains(SelectedItem))
                    Items.Remove(SelectedItem);
            }
        }

        private void NewsStore_ItemsReceived(List<NewsItem> newItems)
        {
            if (newItems == null) return;

            if (newItems != null)
            {
                foreach (var item in newItems.Where(i => Feed.Filter(DataService.Instance.Settings, i)).OrderBy(i => i))
                {
                    Items.Insert(0, item);
                }
            }
        }

        private async void Pin(Rect rect)
        {
            var tileActivationArguments = Feed.FeedInfo.Url.ToString();
            var logo = new Uri("ms-appx:///Assets/Logo.png", UriKind.Absolute);
            var secondaryTile = new SecondaryTile(Feed.Id, Feed.FeedInfo.Title, Feed.FeedInfo.Title, tileActivationArguments, TileOptions.ShowNameOnLogo, logo, logo);
            var isPinned = await secondaryTile.RequestCreateForSelectionAsync(rect);

            if (isPinned)
            {
                DownloadFeedTask.RegisterBackgroundTask(DataService.Instance.Settings.SecondaryTileUpdateInterval);

                try
                {
                    IsPinned = SecondaryTile.Exists(Feed.Id);
                }
                catch (Exception exc)
                {
                    LogService.Error(exc);
                }
            }
        }

        private async void Unpin(Rect rect)
        {
            var secondaryTile = new SecondaryTile(Feed.Id);
            await secondaryTile.RequestDeleteForSelectionAsync(rect);

            try
            {
                IsPinned = SecondaryTile.Exists(Feed.Id);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using NewsFactory.UI.Pages.About;
using NewsFactory.UI.Pages.AppSettings;
using NewsFactory.UI.Pages.ArrangeFeeds;
using NewsFactory.UI.Pages.Feed;
using NewsFactory.UI.Pages.ImportFeeds;
using NewsFactory.UI.Pages.ImportGoogleReaderFeeds;
using NewsFactory.UI.Pages.MessageContainer;
using NewsFactory.UI.Pages.PickFeeds;
using NewsFactory.UI.Pages.PrivacyPolicy;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Animation;

namespace NewsFactory.UI.Pages
{
    [Bindable]
    public partial class MainPageModel : PageModel
    {
        #region .ctors

        public MainPageModel()
        {
            InitializeCommand = new DelegateCommand(Initialize);
            AddFeedCommand = new DelegateCommand(AddFeed);
            ImportFeedsCommand = new DelegateCommand(ImportFeeds);
            ImportFeedsFromGoogleReaderCommand = new DelegateCommand(ImportFeedsFromGoogleReader);
            PickFeedsCommand = new DelegateCommand(PickFeeds);
            GoToFeedCommand = new DelegateCommand<NewsFeed>(GoToFeed);
            RefreshCommand = new DelegateCommand(Refresh);
            MarkAllAsReadCommand = new DelegateCommand(MarkAllAsRead);
            ExportFeedsCommand = new DelegateCommand(ExportFeeds, () => DataService.FeedsStore.NewsFeeds.Count > 0);
            SwitchToDeleteFeedsModeCommand = new DelegateCommand(SwitchToDeleteFeedsMode, () => DataService.FeedsStore.NewsFeeds.Count > 0);
            DeleteFeedsCommand = new AsyncDelegateCommand(DeleteFeeds);
            CancelDeleteFeedsModeCommand = new DelegateCommand(CancelDeleteFeedsMode);
            DeleteAllFeedsCommand = new DelegateCommand(DeleteAllFeeds, () => DataService.FeedsStore.NewsFeeds.Count > 0);
            DeleteAllNewsItemsCommand = new DelegateCommand(DeleteAllNewsItems);

            AddSettingsPane<AppSettingsPage>("Settings");
            AddSettingsPane<ArrangeFeedsView>("Arrange feeds");
            AddSettingsPane<PrivacyPolicyView>("Privacy policy");
            AddSettingsPane<AboutPage>("About");

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

        #region Events

        public event Action FeedAdded;

        #endregion Events

        #region Properties

        /// <summary>
        /// Gets/sets InitializeCommand.
        /// </summary>
        public DelegateCommand InitializeCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_InitializeCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_InitializeCommand != value)
                {
                    p_InitializeCommand = value;
                    OnPropertyChanged("InitializeCommand");
                    OnInitializeCommandChanged();
                }
            }
        }
        private DelegateCommand p_InitializeCommand;
        partial void OnInitializeCommandChanged();

        /// <summary>
        /// Gets/sets Categories.
        /// </summary>
        public ObservableCollection<NewsFeed> Categories
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
        private ObservableCollection<NewsFeed> p_Categories;
        partial void OnCategoriesChanged();

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
                    OnPropertyChanged("NewsFeeds");
                    OnNewsFeedsChanged();
                }
            }
        }
        private ObservableCollection<NewsFeed> p_NewsFeeds;
        partial void OnNewsFeedsChanged();

        /// <summary>
        /// Gets/sets AddFeedCommand.
        /// </summary>
        public DelegateCommand AddFeedCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AddFeedCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AddFeedCommand != value)
                {
                    p_AddFeedCommand = value;
                    OnPropertyChanged("AddFeedCommand");
                    OnAddFeedCommandChanged();
                }
            }
        }
        private DelegateCommand p_AddFeedCommand;
        partial void OnAddFeedCommandChanged();

        /// <summary>
        /// Gets/sets ImportFeedsCommand.
        /// </summary>
        public DelegateCommand ImportFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ImportFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ImportFeedsCommand != value)
                {
                    p_ImportFeedsCommand = value;
                    OnPropertyChanged("ImportFeedsCommand");
                    OnImportFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_ImportFeedsCommand;
        partial void OnImportFeedsCommandChanged();

        /// <summary>
        /// Gets/sets ImportFeedsFromGoogleReaderCommand.
        /// </summary>
        public DelegateCommand ImportFeedsFromGoogleReaderCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ImportFeedsFromGoogleReaderCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ImportFeedsFromGoogleReaderCommand != value)
                {
                    p_ImportFeedsFromGoogleReaderCommand = value;
                    OnPropertyChanged("ImportFeedsFromGoogleReaderCommand");
                    OnImportFeedsFromGoogleReaderCommandChanged();
                }
            }
        }
        private DelegateCommand p_ImportFeedsFromGoogleReaderCommand;
        partial void OnImportFeedsFromGoogleReaderCommandChanged();

        /// <summary>
        /// Gets/sets PickFeedsCommand.
        /// </summary>
        public DelegateCommand PickFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_PickFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_PickFeedsCommand != value)
                {
                    p_PickFeedsCommand = value;
                    OnPropertyChanged("PickFeedsCommand");
                    OnPickFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_PickFeedsCommand;
        partial void OnPickFeedsCommandChanged();

        /// <summary>
        /// Gets/sets GoToFeedCommand.
        /// </summary>
        public DelegateCommand<NewsFeed> GoToFeedCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_GoToFeedCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_GoToFeedCommand != value)
                {
                    p_GoToFeedCommand = value;
                    OnPropertyChanged("GoToFeedCommand");
                    OnGoToFeedCommandChanged();
                }
            }
        }
        private DelegateCommand<NewsFeed> p_GoToFeedCommand;
        partial void OnGoToFeedCommandChanged();

        /// <summary>
        /// Gets/sets RefreshCommand.
        /// </summary>
        public DelegateCommand RefreshCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_RefreshCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_RefreshCommand != value)
                {
                    p_RefreshCommand = value;
                    OnPropertyChanged("RefreshCommand");
                    OnRefreshCommandChanged();
                }
            }
        }
        private DelegateCommand p_RefreshCommand;
        partial void OnRefreshCommandChanged();

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
        /// Gets/sets ExportFeedsCommand.
        /// </summary>
        public DelegateCommand ExportFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ExportFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ExportFeedsCommand != value)
                {
                    p_ExportFeedsCommand = value;
                    OnPropertyChanged("ExportFeedsCommand");
                    OnExportFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_ExportFeedsCommand;
        partial void OnExportFeedsCommandChanged();

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
        /// Gets/sets IsDeleteFeedsMode.
        /// </summary>
        public bool IsDeleteFeedsMode
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsDeleteFeedsMode; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsDeleteFeedsMode != value)
                {
                    p_IsDeleteFeedsMode = value;
                    OnPropertyChanged("IsDeleteFeedsMode");
                    OnIsDeleteFeedsModeChanged();
                }
            }
        }
        private bool p_IsDeleteFeedsMode;
        partial void OnIsDeleteFeedsModeChanged();

        /// <summary>
        /// Gets/sets SwitchToDeleteFeedsModeCommand.
        /// </summary>
        public DelegateCommand SwitchToDeleteFeedsModeCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SwitchToDeleteFeedsModeCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SwitchToDeleteFeedsModeCommand != value)
                {
                    p_SwitchToDeleteFeedsModeCommand = value;
                    OnPropertyChanged("SwitchToDeleteFeedsModeCommand");
                    OnSwitchToDeleteFeedsModeCommandChanged();
                }
            }
        }
        private DelegateCommand p_SwitchToDeleteFeedsModeCommand;
        partial void OnSwitchToDeleteFeedsModeCommandChanged();

        /// <summary>
        /// Gets/sets SelectedFeeds.
        /// </summary>
        public ObservableCollection<NewsFeed> SelectedFeeds
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SelectedFeeds; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SelectedFeeds != value)
                {
                    p_SelectedFeeds = value;
                    OnPropertyChanged("SelectedFeeds");
                    OnSelectedFeedsChanged();
                }
            }
        }
        private ObservableCollection<NewsFeed> p_SelectedFeeds = new ObservableCollection<NewsFeed>();
        partial void OnSelectedFeedsChanged();

        /// <summary>
        /// Gets/sets DeleteFeedsCommand.
        /// </summary>
        public AsyncDelegateCommand DeleteFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteFeedsCommand != value)
                {
                    p_DeleteFeedsCommand = value;
                    OnPropertyChanged("DeleteFeedsCommand");
                    OnDeleteFeedsCommandChanged();
                }
            }
        }
        private AsyncDelegateCommand p_DeleteFeedsCommand;
        partial void OnDeleteFeedsCommandChanged();

        /// <summary>
        /// Gets/sets CancelDeleteFeedsModeCommand.
        /// </summary>
        public DelegateCommand CancelDeleteFeedsModeCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_CancelDeleteFeedsModeCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_CancelDeleteFeedsModeCommand != value)
                {
                    p_CancelDeleteFeedsModeCommand = value;
                    OnPropertyChanged("CancelDeleteFeedsModeCommand");
                    OnCancelDeleteFeedsModeCommandChanged();
                }
            }
        }
        private DelegateCommand p_CancelDeleteFeedsModeCommand;
        partial void OnCancelDeleteFeedsModeCommandChanged();

        /// <summary>
        /// Gets/sets DeleteAllFeedsCommand.
        /// </summary>
        public DelegateCommand DeleteAllFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteAllFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteAllFeedsCommand != value)
                {
                    p_DeleteAllFeedsCommand = value;
                    OnPropertyChanged("DeleteAllFeedsCommand");
                    OnDeleteAllFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_DeleteAllFeedsCommand;
        partial void OnDeleteAllFeedsCommandChanged();

        /// <summary>
        /// Gets/sets DeleteAllNewsItemsCommand.
        /// </summary>
        public DelegateCommand DeleteAllNewsItemsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_DeleteAllNewsItemsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_DeleteAllNewsItemsCommand != value)
                {
                    p_DeleteAllNewsItemsCommand = value;
                    OnPropertyChanged("DeleteAllNewsItemsCommand");
                    OnDeleteAllNewsItemsCommandChanged();
                }
            }
        }
        private DelegateCommand p_DeleteAllNewsItemsCommand;
        partial void OnDeleteAllNewsItemsCommandChanged();

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
        }

        private void Initialize()
        {
            //await MsgService.Show("Hey, it's your first time using the Reader, congratilations! Please use the bottom app bar to add feeds");

            if (NewsFeeds == null)
            {
                if (DataService.FeedsStore == null || DataService.NewsStore == null)
                {
                    Status = "Loading feeds & news, please wait";

                    GeneralHelper.Run(async () =>
                        {
                            await DataService.LoadStores();
                            await DataService.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                {
                                    SetGroups();
                                    Status = null;
                                });
                        });
                }
                else
                {
                    SetGroups();
                }
            }
        }

        private void SetGroups()
        {
            if (Settings.SupportsClassification)
                Categories = new ObservableCollection<NewsFeed>() { DataService.FeedsStore.Unread, DataService.FeedsStore.All, DataService.FeedsStore.ReadLater, DataService.FeedsStore.Favorites, DataService.FeedsStore.Likes, DataService.FeedsStore.Dislikes };
            else
                Categories = new ObservableCollection<NewsFeed>() { DataService.FeedsStore.Unread, DataService.FeedsStore.All, DataService.FeedsStore.ReadLater, DataService.FeedsStore.Favorites };
            NewsFeeds = DataService.FeedsStore.NewsFeeds;

            DataService.FeedsStore.NewsFeeds.CollectionChanged += NewsFeeds_CollectionChanged;
        }

        void NewsFeeds_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                if (FeedAdded != null) FeedAdded();
            }
        }

        private void AddFeed()
        {
            MsgService.ShowDialog(new MessageContainerView() { InternalContent = new AddFeedPage() });
        }

        private async void ImportFeeds()
        {
            var picker = new FileOpenPicker();
            //picker.FileTypeFilter.Add(".xml");
            picker.FileTypeFilter.Add(".opml");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                var feeds = await DataService.GetFeedsToImport(file);
                var view = new ImportFeedsView();
                view.Model.SetFeeds(feeds);
                MsgService.ShowDialog(new MessageContainerView() { InternalContent = view });
            }
        }

        private void GoToFeed(NewsFeed feed)
        {
            NavigationService.NavigateTo<FeedPage>(feed);
        }

        private async void Refresh()
        {
            await DataService.NewsStore.UpdateAll();
        }

        private void MarkAllAsRead()
        {
            DataService.NewsStore.MarkAllAsRead();
        }

        async void t_Tick(object sender, object e)
        {
            await DataService.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => Now = DateTime.Now);
        }

        private void PickFeeds()
        {
            MsgService.ShowDialog(new MessageContainerView() { InternalContent = new PickFeedsView() });
        }

        private void ImportFeedsFromGoogleReader()
        {
            MsgService.ShowDialog(new MessageContainerView() { InternalContent = new ImportGoogleReaderFeedsView() });
        }

        private void ExportFeeds()
        {
            if (DataService.FeedsStore.NewsFeeds.Count == 0) return;

            DataService.ExportFeeds();
        }

        private async void SwitchToDeleteFeedsMode()
        {
            if (DataService.FeedsStore.NewsFeeds.Count == 0) return;

            await MsgService.Ask("Select feeds you would like to delete and press 'Delete' button", Foundation.Services.Buttons.Ok);

            IsDeleteFeedsMode = true;
        }

        private async Task DeleteFeeds()
        {
            IsDeleteFeedsMode = false;

            if (await MsgService.Ask(string.Format("{0} feeds are going to be deleted. Continue?", SelectedFeeds.Count), Buttons.YesNoCancel) == Buttons.Yes)
            {
                foreach (var item in SelectedFeeds.ToList())
                {
                    await DataService.NewsStore.DeleteFeed(item);
                }
            }
        }

        private void CancelDeleteFeedsMode()
        {
            IsDeleteFeedsMode = false;
        }

        private async void DeleteAllFeeds()
        {
            if (DataService.FeedsStore.NewsFeeds.Count == 0) return;

            if (await MsgService.Ask("All feeds are going to be deleted. Continue?", Buttons.YesNoCancel) == Buttons.Yes)
            {
                await DataService.NewsStore.DeleteAllFeeds();
            }
        }

        private async void DeleteAllNewsItems()
        {
            Status = "Deleting news...";

            await DataService.NewsStore.DeleteAllNewsItems();

            Status = null;
        }

        #endregion Methods
    }
}

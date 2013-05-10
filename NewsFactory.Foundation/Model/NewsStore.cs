using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Common;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;

namespace NewsFactory.Foundation.Model
{
    public partial class NewsStore : BindableBase
    {
        #region .ctors

        private NewsStore(FeedsStore feedsStore, StorageFolder feedFolder, StorageFile likes, StorageFile dislikes)
        {
            Settings = feedsStore.Settings;
            _feedsStore = feedsStore;
            _feedsFolder = feedFolder;
            _likesFile = likes;
            _likesQueue = new JobAggregator(OnSaveLikes);
            _dislikesFile = dislikes;
            _dislikesQueue = new JobAggregator(OnSaveDislikes);
            _readItemsQueue = new JobAggregator<string>(OnSaveReadItems, TimeSpan.FromSeconds(1));
            _favIconsQueue = new JobAggregator<NewsFeed>(OnGetFavIcon) { AllowBatching = false, AllowDuplicates = false };
            _favIconsQueue.Empty += OnFavIconsQueueEmpty;

            Items = new ObservableCollection<NewsItem>();
            ItemsMap = new Dictionary<string, NewsItem>();
        }

        #endregion .ctors

        #region Events

        public event Action<List<NewsItem>> ItemsReceived;

        #endregion Events

        #region Fields

        public const string FEEDS = "allfeeds";
        private const string READ = "read";
        private const string LIKES = "likes";
        private const string DISLIKES = "dislikes";

        private object _syncObject = new object();
        private FeedsStore _feedsStore;
        private StorageFolder _feedsFolder;
        private StorageFile _likesFile;
        private JobAggregator _likesQueue;
        private StorageFile _dislikesFile;
        private JobAggregator _dislikesQueue;
        private StorageFile _readItemsFile;
        private JobAggregator<string> _readItemsQueue;
        private JobAggregator<NewsFeed> _favIconsQueue;

        #endregion Fields

        #region Properties

        public Settings Settings { get; private set; }

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
                    OnItemsChanged();
                }
            }
        }
        private ObservableCollection<NewsItem> p_Items;
        partial void OnItemsChanged();

        /// <summary>
        /// Gets/sets ItemsMap.
        /// </summary>
        public Dictionary<string, NewsItem> ItemsMap
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ItemsMap; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ItemsMap != value)
                {
                    p_ItemsMap = value;
                    OnItemsMapChanged();
                }
            }
        }
        private Dictionary<string, NewsItem> p_ItemsMap;
        partial void OnItemsMapChanged();

        /// <summary>
        /// Gets/sets ActiveFeedDownloads.
        /// </summary>
        public int ActiveFeedDownloads
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ActiveFeedDownloads; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ActiveFeedDownloads != value)
                {
                    p_ActiveFeedDownloads = value;
                    OnPropertyChanged("ActiveFeedDownloads");
                    OnActiveFeedDownloadsChanged();
                }
            }
        }
        private int p_ActiveFeedDownloads;
        partial void OnActiveFeedDownloadsChanged();

        #endregion Properties

        #region Methods

        public async static Task<NewsStore> Get(FeedsStore feedsStore)
        {
            var folders = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            var folder = folders.FirstOrDefault(f => f.DisplayName == NewsStore.FEEDS);
            if (folder == null)
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(NewsStore.FEEDS);

            var subfolders = await folder.GetFoldersAsync();

            var feedsFolder = subfolders.FirstOrDefault(f => f.DisplayName == "feeds");
            if (feedsFolder == null)
                feedsFolder = await folder.CreateFolderAsync("feeds");

            var likes = await folder.CreateFileAsync(LIKES, CreationCollisionOption.OpenIfExists);
            var dislikes = await folder.CreateFileAsync(DISLIKES, CreationCollisionOption.OpenIfExists);

            var store = new NewsStore(feedsStore, feedsFolder, likes, dislikes);
            await store.LoadAll(feedsStore);

            return store;
        }

        public Task LoadAll(FeedsStore feedsStore)
        {
            return GeneralHelper.Run(async () =>
            {
                var files = await _feedsFolder.GetFilesAsync();
                var newsFilter = feedsStore.Settings.GetNewsFilter();

                var items = await new JobDispatcher<NewsFeed, List<NewsItem>>(50, async item =>
                    {
                        var fileName = item.FeedInfo.GetFileName();
                        var file = files.FirstOrDefault(f => f.DisplayName == fileName);
                        if (file != null)
                        {
                            var content = await FileIO.ReadTextAsync(file);
                            return string.IsNullOrEmpty(content) ? null : SerializerHelper.Deserialize<List<NewsItem>>(content);
                        }
                        return null;
                    }).Start(feedsStore.NewsFeeds);
                foreach (var item in items.Where(t => t != null))
                {
                    AddItems(null, item.Where(newsFilter));
                }

                _readItemsFile = files.FirstOrDefault(f => f.DisplayName == READ);
                if (_readItemsFile == null)
                    _readItemsFile = await _feedsFolder.CreateFileAsync(READ);
                else
                {
                    var affectedFeeds = new HashSet<NewsFeed>();
                    var lines = await FileIO.ReadLinesAsync(_readItemsFile);
                    var urls = lines.ToList();
                    foreach (var url in lines.Where(u => u != null && ItemsMap.ContainsKey(u)))
                    {
                        if (ItemsMap[url].MarkAsRead() && !affectedFeeds.Contains(ItemsMap[url].Feed))
                            affectedFeeds.Add(ItemsMap[url].Feed);
                    }
                    foreach (var item in affectedFeeds)
                    {
                        await SaveFeed(item);
                    }
                    await FileIO.WriteTextAsync(_readItemsFile, "");
                }

                var likesContent = await FileIO.ReadTextAsync(_likesFile);
                var likes = SerializerHelper.Deserialize<ObservableCollection<NewsItem>>(likesContent);
                _feedsStore.Likes.Items = likes ?? new ObservableCollection<NewsItem>();

                var dislikesContent = await FileIO.ReadTextAsync(_dislikesFile);
                var dislikes = SerializerHelper.Deserialize<ObservableCollection<NewsItem>>(dislikesContent);
                _feedsStore.Dislikes.Items = dislikes ?? new ObservableCollection<NewsItem>();
            });
        }

        public async Task UpdateAll()
        {
            await UpdateMany(_feedsStore.NewsFeeds.Select(t => t.FeedInfo).ToList());
        }

        public async Task UpdateMany(List<FeedInfo> feedsToUpdate)
        {
            if (ActiveFeedDownloads != 0) return;

            var feeds = await _feedsStore.RegisterMany(feedsToUpdate);

            var receiveDate = DateTime.Now;
            Settings.LastUpdated = receiveDate;
            await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => ActiveFeedDownloads = feeds.Count);

            LogService.Info("Start updating many feeds ({0})", feeds.Count);

            await GeneralHelper.Run(async () =>
            {
                await new JobDispatcher<NewsFeed>(30, async (feed) =>
                    {
                        var newItems = await feed.DownloadFeed(receiveDate);
                        if (newItems != null)
                        {
                            newItems = newItems.Where(i => !ItemsMap.ContainsKey(i.Url.ToString())).OrderBy(t => t.Published).ToList();
                            if (newItems.Count > 0)
                            {
                                await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => AddItems(feed, newItems));
                                await SaveFeed(feed);
                            }
                        }

                        await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                        {
                            lock (_syncObject)
                                ActiveFeedDownloads--;
                        });
                    }).Start(feeds);

                await _feedsStore.Save();

                _favIconsQueue.AddRange(_feedsStore.NewsFeeds.Where(f => f.FeedInfo.HasDefaultFavIcon).ToList());
            });
        }

        private async Task OnGetFavIcon(List<NewsFeed> feeds)
        {
            var feed = feeds.Single();
            var favIconUri = await feed.GetFavIcon(null);
            await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                feed.FeedInfo.HasDefaultFavIcon = true;
                if (favIconUri != null)
                    feed.FeedInfo.FavIconUrl = favIconUri;
            });
        }

        private async void OnFavIconsQueueEmpty(JobAggregator<NewsFeed> sender)
        {
            await _feedsStore.Save();
        }

        public async Task UpdateOne(FeedInfo feedInfo, bool saveChangesIfNeeded = true, DateTime? receiveDate = null)
        {
            if (feedInfo.Url == null)
                await UpdateAll();
            else
                await UpdateMany(new List<FeedInfo>() { feedInfo });
        }

        public void RemoveAllFavorites()
        {
            lock (_syncObject)
            {
                foreach (var item in Items)
                {
                    item.IsFavorite = false;
                }
            }
        }

        public void RemoveAllReadLater()
        {
            lock (_syncObject)
            {
                foreach (var item in Items)
                {
                    item.IsReadLater = false;
                }
            }
        }

        public async Task SaveFeed(NewsFeed feed)
        {
            if (feed.FeedInfo.Url == null) return;

            var file = await _feedsFolder.CreateFileAsync(feed.FeedInfo.GetFileName(), CreationCollisionOption.ReplaceExisting);
            var newsFilter = Settings.GetNewsFilter();
            await FileIO.WriteTextAsync(file, SerializerHelper.Serialize(GetItems(feed).Where(newsFilter).ToList()));
        }

        public void MarkAsRead(NewsItem item)
        {
            MarkAsRead(new NewsItem[] { item });
        }

        public void MarkAsRead(IEnumerable<NewsItem> items)
        {
            if (items == null) return;

            var readItems = new List<string>();
            foreach (var item in items)
            {
                if (item.MarkAsRead())
                    readItems.Add(item.Url.ToString());
            }
            if (readItems.Count > 0)
                _readItemsQueue.AddRange(readItems);
        }

        public void MarkAsRead(NewsFeed feed)
        {
            MarkAsRead(GetItems(feed));
        }

        public void MarkAllAsRead()
        {
            var readItems = new List<string>();
            lock (_syncObject)
            {
                foreach (var item in Items)
                {
                    if (item.IsNew)
                    {
                        item.IsNew = false;
                        readItems.Add(item.Url.ToString());
                    }
                }
                foreach (var item in _feedsStore.NewsFeeds)
                {
                    item.NewItemsCount = 0;
                }
                _feedsStore.All.NewItemsCount = 0;
                _feedsStore.Unread.NewItemsCount = 0;
            }
            if (readItems.Count > 0)
                _readItemsQueue.AddRange(readItems);
        }

        public ObservableCollection<NewsItem> GetItems(NewsFeed feed)
        {
            var s = DataService.Instance.Settings;
            lock (_syncObject)
                return new ObservableCollection<NewsItem>(Items.Where(i => feed.Filter(s, i)).OrderByDescending(i => i));
        }

        public async Task DeleteFeed(NewsFeed feed)
        {
            if (feed.FeedInfo.Url == null) return;

            await _feedsStore.DeleteFeed(feed);

            await DeleteFeedNewsItems(feed);
        }

        public async Task DeleteAllFeeds()
        {
            await _feedsStore.DeleteAllFeeds();
            lock (_syncObject)
            {
                Items.Clear();
                ItemsMap.Clear();
                _feedsStore.All.NewItemsCount = 0;
                _feedsStore.Unread.NewItemsCount = 0;
            }
        }

        public async Task DeleteFeedNewsItems(NewsFeed feed, bool deleteFile = false)
        {
            lock (_syncObject)
            {
                var items = GetItems(feed);
                var deletedItemsCount = 0;
                foreach (var item in items)
                {
                    Items.Remove(item);
                    ItemsMap.Remove(item.Url.ToString());
                    if (item.IsNew)
                        deletedItemsCount++;
                }
                _feedsStore.All.NewItemsCount -= deletedItemsCount;
                _feedsStore.Unread.NewItemsCount -= deletedItemsCount;
                feed.NewItemsCount -= deletedItemsCount;
            }

            await ClearFeedFile(feed, deleteFile);
        }

        private async Task ClearFeedFile(NewsFeed feed, bool deleteFile = false)
        {
            var file = await _feedsFolder.GetFileOrDefault(feed.FeedInfo.GetFileName());
            if (file != null)
            {
                if (deleteFile)
                    await file.DeleteAsync();
                else
                    await FileIO.WriteTextAsync(file, "");
            }
        }

        public async Task DeleteAllNewsItems()
        {
            lock (_syncObject)
            {
                foreach (var feed in _feedsStore.NewsFeeds)
                {
                    feed.NewItemsCount = 0;
                }
                Items.Clear();
                ItemsMap.Clear();
                _feedsStore.All.NewItemsCount = 0;
                _feedsStore.Unread.NewItemsCount = 0;
            }

            var files = await _feedsFolder.GetFilesAsync();
            foreach (var item in files.Where(f => f.Name != READ))
            {
                await item.DeleteAsync();
            }
        }

        private void AddItems(NewsFeed feed, IEnumerable<NewsItem> items)
        {
            if (items == null) return;

            var newItems = new List<NewsItem>();

            lock (_syncObject)
            {
                foreach (var item in items)
                {
                    if (!ItemsMap.ContainsKey(item.Url.ToString()))
                    {
                        Items.Insert(0, item);
                        ItemsMap[item.Url.ToString()] = item;
                        item.Feed = feed ?? _feedsStore.NewsFeedsMap[item.FeedUrl];
                        if (item.IsNew)
                        {
                            if (feed == null)
                            {
                                item.Feed.NewItemsCount++;
                                _feedsStore.All.NewItemsCount++;
                                _feedsStore.Unread.NewItemsCount++;
                            }
                            newItems.Add(item);
                        }
                    }
                }
                if (feed != null)
                {
                    feed.NewItemsCount += newItems.Count;
                    _feedsStore.All.NewItemsCount += newItems.Count;
                    _feedsStore.Unread.NewItemsCount += newItems.Count;
                }
            }

            if (ItemsReceived != null)
                ItemsReceived(newItems);
        }

        private async Task OnSaveReadItems(List<string> data)
        {
            try
            {
                await FileIO.AppendLinesAsync(_readItemsFile, data);
            }
            catch (FileNotFoundException)
            {
                _readItemsFile = null;
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
            if (_readItemsFile == null)
            {
                try
                {
                    _readItemsFile = await _feedsFolder.CreateFileAsync(READ, CreationCollisionOption.OpenIfExists);
                    await FileIO.AppendLinesAsync(_readItemsFile, data);
                }
                catch
                {
                }
            }
        }

        public void MarkAsLike(NewsItem item)
        {
            _feedsStore.Likes.Items.Add(item);
            _likesQueue.Trigger();
        }

        public void MarkAsDislike(NewsItem item)
        {
            _feedsStore.Dislikes.Items.Add(item);
            _dislikesQueue.Trigger();
        }

        private async Task OnSaveLikes()
        {
            try
            {
                await FileIO.WriteTextAsync(_likesFile, SerializerHelper.Serialize(_feedsStore.Likes.Items));
                await new ClassifierService().SendTrainingSet(Settings.Instance.UniqueID);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        private async Task OnSaveDislikes()
        {
            try
            {
                await FileIO.WriteTextAsync(_dislikesFile, SerializerHelper.Serialize(_feedsStore.Dislikes.Items));
                await new ClassifierService().SendTrainingSet(Settings.Instance.UniqueID);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Search;

namespace NewsFactory.Foundation.Model
{
    public partial class FeedsStore : TrackableObject
    {
        #region .ctors

        private FeedsStore(Settings settings, ObservableCollection<FeedInfo> feeds, StorageFolder folder)
        {
            Debug.Assert(feeds != null);

            Settings = settings;
            _folder = folder;
            _feeds = feeds;
            NewsFeeds = new ObservableCollection<NewsFeed>();
            NewsFeedsMap = new Dictionary<Uri, NewsFeed>();
            foreach (var item in _feeds)
            {
                var newsItem = new NewsFeed(item, this, folder, (s, i) => i.FeedUrl == item.Url);
                NewsFeeds.Add(newsItem);
                NewsFeedsMap[item.Url] = newsItem;
            }

            All = new NewsFeed(
                new FeedInfo()
                {
                    Title = "All news",
                    Description = "All feeds on one page",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => (s != null && s.ShowOnlyNewItemsInAllItemsFeed) ? i.IsNew : true);
            Unread = new NewsFeed(
                new FeedInfo()
                {
                    Title = "All unread news",
                    Description = "All unread news on one page",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => i.IsNew);
            Favorites = new NewsFeed(
                new FeedInfo()
                {
                    Title = "Bookmarks",
                    Description = "Something to remember...",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => i.IsFavorite);
            ReadLater = new NewsFeed(
                new FeedInfo()
                {
                    Title = "Read it later",
                    Description = "when you have time",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => i.IsReadLater);
            Likes = new NewsFeed(
                new FeedInfo()
                {
                    Title = "Likes",
                    Description = "The best of the best",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => i.IsLike);
            Dislikes = new NewsFeed(
                new FeedInfo()
                {
                    Title = "Dislikes",
                    Description = "The worst of the worst",
                    FavIconUrl = new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute)
                },
                this,
                folder,
                (s, i) => i.IsDislike);
        }

        #endregion .ctors

        #region Fields

        private const string FILE_NAME = "feedsStore";
        private const string FILE_NAME_PREV = "feedsStore.prev";

        private object _syncObject = new object();
        private StorageFolder _folder;
        private ObservableCollection<FeedInfo> _feeds;

        #endregion Fields

        #region Properties

        public Settings Settings { get; private set; }

        public NewsFeed All { get; private set; }
        public NewsFeed Unread { get; private set; }
        public NewsFeed Favorites { get; private set; }
        public NewsFeed ReadLater { get; private set; }
        public NewsFeed Likes { get; private set; }
        public NewsFeed Dislikes { get; private set; }

        public ObservableCollection<NewsFeed> NewsFeeds { get; private set; }
        public Dictionary<Uri, NewsFeed> NewsFeedsMap { get; private set; }

        #endregion Properties

        #region Methods

        public async static Task<FeedsStore> Get(Settings settings)
        {
            var folders = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            var folder = folders.FirstOrDefault(f => f.DisplayName == NewsStore.FEEDS);
            if (folder == null)
                folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(NewsStore.FEEDS);
            var subfolders = await folder.GetFoldersAsync();
            var subfolder = subfolders.FirstOrDefault(f => f.DisplayName == "icons");
            if (subfolder == null)
                subfolder = await folder.CreateFolderAsync("icons");

            try
            {
                var file = await ApplicationData.Current.LocalFolder.GetFirstFile(FILE_NAME, FILE_NAME_PREV);
                if (file == null)
                {
                    await ApplicationData.Current.LocalFolder.CreateFileAsync(FILE_NAME);
                    var instance = new FeedsStore(
                        settings,
                        new ObservableCollection<FeedInfo>() 
                        {
                            new FeedInfo() { Title = "Ars Technica", Description = "The Art of Technology", Url = "http://feeds.arstechnica.com/arstechnica/index/".ToUri(), FavIconUrl = "http://arstechnica.com/favicon.ico".ToUri() },
                            new FeedInfo() { Title = "xkcd.com", Description = "xkcd.com: A webcomic of romance and math humor.", Url = "http://xkcd.com/rss.xml".ToUri(), FavIconUrl = "http://xkcd.com/favicon.ico".ToUri() }
                        },
                        subfolder);
                    await instance.Save();
                    return instance;
                }
                else
                    LogService.Info("Loaded feeds from {0}", file.Name);

                var data = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                var feedsInfo = (IEnumerable<FeedInfo>)SerializerHelper.Deserialize<ObservableCollection<FeedInfo>>(data);
                var feeds = new ObservableCollection<FeedInfo>(settings.FeedOrderMode == FeedOrderMode.SortedAlphabetically ? feedsInfo.OrderBy(t => t.Title) : feedsInfo);

                return new FeedsStore(settings, feeds, subfolder);
            }
            catch (Exception)
            {
                return new FeedsStore(settings, new ObservableCollection<FeedInfo>(), subfolder);
            }
        }

        public async Task DeleteFeed(NewsFeed feed)
        {
            if (feed.FeedInfo.Url == null) return;

            _feeds.Remove(feed.FeedInfo);
            NewsFeeds.Remove(feed);
            NewsFeedsMap.Remove(feed.FeedInfo.Url);
            await Save();
        }

        public async Task DeleteAllFeeds()
        {
            _feeds.Clear();
            NewsFeeds.Clear();
            NewsFeedsMap.Clear();
            await Save();
        }

        public void SwapFeeds(int from, int to)
        {
            _feeds.Move(from, to);
        }

        public async Task Save()
        {
            LogService.Info("Saving FeedsStore");
            try
            {
                await ApplicationData.Current.LocalFolder.Copy(FILE_NAME, FILE_NAME_PREV);
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync(FILE_NAME, CreationCollisionOption.OpenIfExists);
                await FileIO.WriteTextAsync(file, SerializerHelper.Serialize(_feeds));
                LogService.Info("Saved FeedsStore");
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        public async Task<List<NewsFeed>> RegisterMany(List<FeedInfo> feedsToAdd)
        {
            if (feedsToAdd == null) throw new ArgumentException("feedsToAdd");

            var newFeeds = new List<NewsFeed>();
            foreach (var feedInfo in feedsToAdd)
            {
                if (feedInfo == null || feedInfo.Url == null) throw new ArgumentException("feedInfo");
                if (!NewsFeedsMap.ContainsKey(feedInfo.Url))
                {
                    feedInfo.IsNew = true;
                    feedInfo.IsActive = true;
                    feedInfo.HasDefaultFavIcon = true;
                    feedInfo.FavIconUrl =
                        feedInfo.FavIconUrl != null
                        ? feedInfo.FavIconUrl
                        : string.Format("{0}://{1}/favicon.ico", feedInfo.Url.Scheme, feedInfo.Url.Host).ToUri();

                    var feed = new NewsFeed(feedInfo, this, _folder, (s, i) => i.FeedUrl == feedInfo.Url);
                    NewsFeedsMap[feedInfo.Url] = feed;
                    newFeeds.Add(feed);
                }
            }
            if (newFeeds.Count > 0)
            {
                newFeeds = newFeeds.OrderBy(f => f.FeedInfo.Title).ToList();
                await DataService.Instance.Invoke(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    if (Settings.FeedOrderMode == FeedOrderMode.SortedAlphabetically)
                        Merge(newFeeds, NewsFeeds);
                    else
                    {
                        foreach (var item in newFeeds)
                        {
                            NewsFeeds.Add(item);
                            _feeds.Add(item.FeedInfo);
                        }
                    }
                });

                await Save();
            }
            return feedsToAdd.Select(f => NewsFeedsMap[f.Url]).ToList();
        }

        private void Merge(List<NewsFeed> newFeeds, ObservableCollection<NewsFeed> existingFeeds)
        {
            var newFeedIndex = 0;
            var existingFeedIndex = 0;

            while (newFeeds.Count > newFeedIndex)
            {
                if (existingFeedIndex >= existingFeeds.Count)
                {
                    existingFeeds.Add(newFeeds[newFeedIndex]);
                    _feeds.Add(newFeeds[newFeedIndex].FeedInfo);
                    newFeedIndex++;
                }
                else
                {
                    if (string.Compare(existingFeeds[existingFeedIndex].FeedInfo.Title, newFeeds[newFeedIndex].FeedInfo.Title) > 0)
                    {
                        existingFeeds.Insert(existingFeedIndex, newFeeds[newFeedIndex]);
                        _feeds.Insert(existingFeedIndex, newFeeds[newFeedIndex].FeedInfo);
                        newFeedIndex++;
                        existingFeedIndex++;
                    }
                    else
                        existingFeedIndex++;
                }
            }
        }

        #endregion Methods
    }
}

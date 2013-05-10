using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.ApplicationModel.Background;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.Web.Syndication;

namespace NewsFactory.Foundation.Services
{
    public partial class DataService : TrackableObject
    {
        #region .ctors

        private DataService()
        {
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();
        private Settings _settings;

        #endregion Fields

        #region Properties

        public static DataService Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataService();
                return _instance;
            }
        }
        private static DataService _instance;

        public CoreDispatcher Dispatcher { get; set; }

        public Settings Settings
        {
            get
            {
                if (_settings == null) throw new NotSupportedException();
                return _settings;
            }
        }

        /// <summary>
        /// Gets/sets FeedsStore.
        /// </summary>
        public FeedsStore FeedsStore
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_FeedsStore; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_FeedsStore != value)
                {
                    p_FeedsStore = value;
                    OnPropertyChanged("FeedsStore");
                    OnFeedsStoreChanged();
                }
            }
        }
        private FeedsStore p_FeedsStore;
        partial void OnFeedsStoreChanged();

        /// <summary>
        /// Gets/sets NewsStore.
        /// </summary>
        public NewsStore NewsStore
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_NewsStore; }
            [System.Diagnostics.DebuggerStepThrough]
            private set
            {
                if (p_NewsStore != value)
                {
                    p_NewsStore = value;
                    OnPropertyChanged("NewsStore");
                    OnNewsStoreChanged();
                }
            }
        }
        private NewsStore p_NewsStore;
        partial void OnNewsStoreChanged();

        #endregion Properties

        #region Methods

        public async Task Init()
        {
            await LogService.Init();

            LogService.Info("Loading settings...");

            await LoadSettings();

            LogService.Info("Loaded settings");
        }

        public async Task LoadStores()
        {
            LogService.Info("Loading data stores...");

            var feedsStore = await FeedsStore.Get(Settings);
            var newsStore = await NewsStore.Get(feedsStore);

            LogService.Info("Loaded {0} feeds, {1} newsitems", feedsStore.NewsFeeds.Count, newsStore.Items.Count);

            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    FeedsStore = feedsStore;
                    NewsStore = newsStore;
                });

            if ((DateTime.Now - (Settings.LastUpdated ?? DateTime.MinValue)).TotalMinutes >= Settings.UpdateInterval)
            {
                var t = GeneralHelper.Run(async () =>
                {
                    Settings.LastUpdated = DateTime.Now;
                    await SaveSettings();
                    await NewsStore.UpdateAll();
                });
            }
        }

        public async Task SaveSettings()
        {
            await SaveTo("settings", SerializerHelper.Serialize(_settings));
        }

        private async Task SaveTo(string fileName, string obj)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            var file = files.FirstOrDefault(f => f.Name == fileName);
            if (file == null)
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName);
            await FileIO.WriteTextAsync(file, obj);
        }

        private async Task<string> LoadFrom(string fileName)
        {
            var files = await ApplicationData.Current.LocalFolder.GetFilesAsync(CommonFileQuery.DefaultQuery);
            var file = files.FirstOrDefault(f => f.Name == fileName);
            if (file == null)
                return null;
            else
                return await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
        }

        private async Task LoadSettings()
        {
            var str = await LoadFrom("settings");
            if (str == null)
                _settings = new Settings();
            else
            {
                try
                {
                    _settings = SerializerHelper.Deserialize<Settings>(str);
                }
                catch (Exception)
                {
                    _settings = new Settings();
                }
            }
            if (_settings.UniqueID == null)
            {
                _settings.UniqueID = new string(Guid.NewGuid().ToString().OfType<char>().Where(c => char.IsLetterOrDigit(c)).ToArray());
                await SaveSettings();
            }
            Settings.Instance = _settings;
        }

        public async Task<List<Tuple<string, string>>> GetFeedsToImport(StorageFile file)
        {
            var content = await FileIO.ReadTextAsync(file);
            var doc = XDocument.Parse(content, LoadOptions.None);
            var list = doc.DescendantNodes().OfType<XElement>().Where(t => t.Name == "outline").Select(t => new { Title = t.Attribute("title").Value, Url = t.Attribute("xmlUrl").Value }).ToList();

            return list.Select(t => new Tuple<string, string>(t.Url, t.Title)).ToList();
        }

        public async void ExportFeeds()
        {
            var p = new FileSavePicker();
            p.SuggestedFileName = "feeds.opml";
            p.FileTypeChoices.Add("opml", new List<string>() { ".opml" });
            var file = await p.PickSaveFileAsync();
            if (file != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                sb.AppendLine("<opml version=\"1.0\">");
                sb.AppendLine("\t<head>");
                sb.AppendLine("\t\t<title>News Reader</title>");
                sb.AppendLine("\t</head>");
                sb.AppendLine("\t<body>");
                foreach (var item in FeedsStore.NewsFeeds.OrderBy(t => t.FeedInfo.Title))
                {
                    sb.AppendLine(string.Format("\t\t<outline text=\"{0}\" title=\"{0}\" type=\"rss\" xmlUrl=\"{1}\" />", item.FeedInfo.Title.Replace("\"", "&quot;").Replace("&", "&amp;"), item.FeedInfo.Url.ToString().Replace("\"", "&quot;").Replace("&", "&amp;")));
                }
                sb.AppendLine("\t</body>");
                sb.AppendLine("</opml>");

                await FileIO.WriteTextAsync(file, sb.ToString(), Windows.Storage.Streams.UnicodeEncoding.Utf8);
            }
        }

        #endregion Methods
    }
}

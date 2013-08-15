using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml.Data;
using Windows.Web.Syndication;

namespace NewsFactory.Foundation.Model
{
    [Bindable]
    public partial class NewsFeed : TrackableObject
    {
        #region .ctors

        public NewsFeed(FeedInfo info, FeedsStore store, StorageFolder folder, Func<Settings, NewsItem, bool> filter)
        {
            Store = store;
            _folder = folder;
            FeedInfo = info;
            Filter = filter ?? ((s, i) => i.FeedUrl == info.Url);
        }

        #endregion .ctors

        #region Events

        public event Action<bool> IsSelectedChanged;
        #endregion Events

        #region Fields

        private object _syncObject = new object();
        private StorageFolder _folder;

        #endregion Fields

        #region Properties

        public FeedInfo FeedInfo { get; private set; }
        public Func<Settings, NewsItem, bool> Filter { get; private set; }
        public FeedsStore Store { get; private set; }

        /// <summary>
        /// Gets/sets NewItemsCount.
        /// </summary>
        public int NewItemsCount
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_NewItemsCount; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_NewItemsCount != value)
                {
                    p_NewItemsCount = value;
                    OnPropertyChanged("NewItemsCount");
                    OnNewItemsCountChanged();
                }
            }
        }
        private int p_NewItemsCount;
        partial void OnNewItemsCountChanged();

        /// <summary>
        /// Gets/sets IsLoading.
        /// </summary>
        public bool IsLoading
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsLoading; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsLoading != value)
                {
                    p_IsLoading = value;
                    OnPropertyChanged("IsLoading");
                    OnIsLoadingChanged();
                }
            }
        }
        private bool p_IsLoading;
        partial void OnIsLoadingChanged();

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

        /// <summary>
        /// Gets/sets Items.
        /// </summary>
        [IgnoreDataMember]
        public ObservableCollection<NewsItem> Items
        {
            get 
            {
                return p_Items; 
            }
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
        [IgnoreDataMember]
        private ObservableCollection<NewsItem> p_Items;
        partial void OnItemsChanged();

        #endregion Properties

        #region Methods

        public override string ToString()
        {
            return string.Format("Feed: {0}", FeedInfo.Title);
        }

        public async Task<List<NewsItem>> DownloadFeed(DateTime? receiveDate = null)
        {
            LogService.Info("-> Updating {0}", FeedInfo.Title);

            await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => IsLoading = true);

            var client = new SyndicationClient();
            client.BypassCacheOnRetrieve = true;
            // Although most HTTP servers do not require User-Agent header, others will reject the request or return
            // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
            client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

            var sf = default(SyndicationFeed);
            var tryManualFixing = false;

            if (FeedInfo.Url.ToString() == "http://www.dontdatehimgirl.com/feeds/todayonddhg.php" || FeedInfo.Url.ToString() == "http://bnreview.barnesandnoble.com/bnreview/plugins/custom/barnesandnoble/bnreview/rss_full")
            {
                tryManualFixing = true;
            }
            else
            {
                try
                {
                    sf = await client.RetrieveFeedAsync(FeedInfo.Url);
                }
                catch (Exception exc)
                {
                    if (exc.Message == "Invalid XML. (Exception from HRESULT: 0x83750002)")
                        tryManualFixing = true;
                    else
                        LogService.Error(exc, "Cannot download feed {0}", FeedInfo.Url);
                }
            }

            if (tryManualFixing)
            {
                try
                {
                    var httpClient = new HttpClient();
                    var content = await httpClient.GetStringAsync(FeedInfo.Url);
                    content = ApplyCorrections(content);

                    sf = new SyndicationFeed();
                    sf.Load(content);
                }
                catch (Exception exc)
                {
                    sf = null;
                    LogService.Error(exc, "Cannot download feed {0}, manual parsing failed", FeedInfo.Url);
                }
            }

            if (sf == null)
            {
                await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => IsLoading = false);
                return null;
            }

            if (receiveDate == null)
                receiveDate = DateTime.Now;

            var favIconUri = default(Uri);

            var items = sf.Items
                .Select(t => new NewsItem()
                {
                    FeedUrl = FeedInfo.Url,
                    ReceivedAt = receiveDate.Value,
                    IsNew = true,
                    Url = t.ItemUri != null ? t.ItemUri : (t.Links.Count > 0 ? ExtractLink(t.Links) : null),
                    Title = t.Title != null ? t.Title.Text.Beautify() : null,
                    Description = t.Summary != null ? t.Summary.Text : (t.Content != null ? t.Content.Text : null),
                    Published = t.PublishedDate.LocalDateTime
                })
                .Where(i => i.Url != null && i.Published > FeedInfo.LastPub)
                .ToList();

            var newLastPub = items.Count > 0 ? items.Max(i => i.Published) : (DateTime?)null;

            await DataService.Instance.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                FeedInfo.IsNew = false;

                if (sf.Title != null && !string.IsNullOrWhiteSpace(sf.Title.Text.Beautify()))
                    FeedInfo.Title = sf.Title.Text.Beautify();
                if (sf.Subtitle != null)
                    FeedInfo.Description = sf.Subtitle.Text.Beautify();
                if (sf.ImageUri != null)
                    FeedInfo.ImageUrl = sf.ImageUri;
                if (newLastPub.HasValue)
                    FeedInfo.LastPub = newLastPub.Value;
                if (favIconUri != null)
                    FeedInfo.FavIconUrl = favIconUri;

                IsLoading = false;
            });

            LogService.Info("<- Updated {0}", FeedInfo.Title);

            return items;
        }

        private Uri ExtractLink(IList<SyndicationLink> links)
        {
            var link = (links.Count == 1) ? links[0] : (links.Any(l => l.Relationship == "alternate") ? links.First(l => l.Relationship == "alternate") : links[0]);
            try 
	        {
                return link.Uri;
	        }
	        catch
	        {
	        }
            return link.NodeValue.ToUri();
        }

        private string ApplyCorrections(string content)
        {
            // Fix Barns & Noble feed
            foreach (var item in Regex.Matches(content, "<bn:ean>([0-9]+&)</bn:ean>").OfType<Match>())
            {
                content = content.Replace(item.Value, "");
            }

            content = content.Replace("&nbsp;", " ").Replace("</br>", "<br/>").Replace("<br>", "<br/>").Replace("<BR>", "<br/>");
            return content;
        }

        public async Task<Uri> GetFavIcon(Uri currentValue = null)
        {
            var candidates = new Dictionary<string, int>();
            var httpClient = new HttpClient() { Timeout = TimeSpan.FromSeconds(15) };
            
            try
            {
                var siteUrl = default(Uri);
                var siteUrlValue = default(string);
                var s = await httpClient.GetStringAsync(FeedInfo.Url);
                if (!string.IsNullOrWhiteSpace(s))
                {
                    var doc = default(XDocument);
                    try
                    {
                        doc = XDocument.Load(new StringReader(s));
                    }
                    catch
                    {
                        s = ApplyCorrections(s);
                        doc = XDocument.Load(new StringReader(s));
                    }

                    var link = doc.Descendants("channel").Elements("link").FirstOrDefault();
                    if (link != null)
                        siteUrlValue = link.Value;
                    else
                    {
                        link = doc.Descendants().Where(e => e.Name.LocalName == "feed").FirstOrDefault();
                        if (link != null)
                        {
                            link = link.Elements().Where(t => t.Name.LocalName == "link").OrderBy(t => t.Attribute("rel") != null ? t.Attribute("rel").Value : "z").FirstOrDefault();
                            if (link != null && link.Attribute("href") != null)
                                siteUrlValue = link.Attribute("href").Value;
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(siteUrlValue))
                        siteUrl = new Uri(siteUrlValue, UriKind.Absolute);
                }
                if (siteUrl != null)
                {
                    if (siteUrlValue == FeedInfo.Url.ToString()) // If true, it means we still don't have site url, so we might as well try something different here
                        siteUrl = new Uri(string.Format("{0}://{1}", siteUrl.Scheme, siteUrl.Host), UriKind.Absolute);

                    try
                    {
                        var response = await httpClient.GetAsync(siteUrl);
                        var content = await ExtractContent(response);

                        var favIconString = XamlConverter.GetTagAttributeBySpecificAttribute(content, "link", "rel", "shortcut icon", "href").FirstOrDefault();
                        if (favIconString == null)
                            favIconString = XamlConverter.GetTagAttributeBySpecificAttribute(content, "link", "rel", "icon", "href").FirstOrDefault();

                        var priorityOffset = 0;
                        if (favIconString == null && string.Format("{0}://{1}", siteUrl.Scheme, siteUrl.Host) != siteUrlValue)
                        {
                            siteUrl = new Uri(string.Format("{0}://{1}", siteUrl.Scheme, siteUrl.Host), UriKind.Absolute);

                            response = await httpClient.GetAsync(siteUrl);
                            content = await response.Content.ReadAsStringAsync();

                            favIconString = XamlConverter.GetTagAttributeBySpecificAttribute(content, "link", "rel", "shortcut icon", "href").FirstOrDefault();
                            if (favIconString == null)
                                favIconString = XamlConverter.GetTagAttributeBySpecificAttribute(content, "link", "rel", "icon", "href").FirstOrDefault();
                            priorityOffset = 100;
                        }

                        if (favIconString != null)
                        {
                            if (favIconString.StartsWith("http"))
                                AddCandidate(candidates, favIconString, 1 + priorityOffset);
                            else
                            {
                                if (favIconString.StartsWith("//"))
                                    favIconString = favIconString.Substring(2);
                                if (favIconString.StartsWith("/"))
                                    favIconString = favIconString.Substring(1);
                                AddCandidate(candidates, string.Format("{0}://{1}/{2}", siteUrl.Scheme, siteUrl.Host, favIconString), 1 + priorityOffset);
                                AddCandidate(candidates, siteUrlValue.TrimEnd('/') + "/" + favIconString, 2 + priorityOffset);
                            }
                        }
                    }
                    catch
                    {
                    }

                    AddCandidate(candidates, siteUrlValue + "/favicon.ico", 3);
                    AddCandidate(candidates, string.Format("{0}://{1}/favicon.ico", siteUrl.Scheme, siteUrl.Host), 4);
                }
            }
            catch
            {
            }

            AddCandidate(candidates, string.Format("{0}://{1}/favicon.ico", FeedInfo.Url.Scheme, FeedInfo.Url.Host), 500);

            if (currentValue != null)
                AddCandidate(candidates, currentValue.ToString(), 501);

            // Some sites send favicons as "text/html" (for instance, businessinsider), so we need to return "text" icon if there are no better choices
            // By better choices I mean everything which has type "image" and doesn't come from feed aggregator sites like feedburner.com
            var lesserEvilUrl = default(Uri);
            var genericAggrUrl = default(Uri);
            
            foreach (var item in candidates.OrderBy(t => t.Value).Select(t => t.Key))
            {
                try
                {
                    var response = await httpClient.GetAsync(item);
                    if (response.IsSuccessStatusCode && response.Content.Headers.ContentLength > 0)
                    {
                        if (item == "http://feeds2.feedburner.com/favicon.ico" || item == "http://feeds.feedburner.com/favicon.ico")
                        {
                            genericAggrUrl = UriHelper.ToUri(item);
                        }
                        else if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.StartsWith("text"))
                        {
                            if (lesserEvilUrl == null)
                                lesserEvilUrl = UriHelper.ToUri(item);
                        }
                        else if (response.Content.Headers.ContentType != null && response.Content.Headers.ContentType.MediaType.StartsWith("image"))
                        {
                            try 
	                        {
                                var ext = response.Content.Headers.ContentType.MediaType == "image/png" ? "png" : (response.Content.Headers.ContentType.MediaType == "image/gif" ? "gif" : "ico");
                                var image = await response.Content.ReadAsByteArrayAsync();
                                var fileName = string.Format("{0}.{1}", StringHelper.Encode(item), ext);
                                LogService.Info("Feed {0}: fav icon = {1}", FeedInfo.Title, item);
                                if (await IOHelper.GetFileOrDefault(_folder, fileName) == null)
                                {
                                    var imageFile = await _folder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                                    await FileIO.WriteBytesAsync(imageFile, image);
                                }
                                return new Uri(string.Format("ms-appdata:///local/allfeeds/icons/{0}", fileName));
	                        }
	                        catch
	                        {
	                        }
                            
                            return UriHelper.ToUri(item);
                        }
                    }
                }
                catch (Exception)
                {
                }
            }

            if (lesserEvilUrl != null || genericAggrUrl != null)
                return lesserEvilUrl ?? genericAggrUrl;

            return new Uri("ms-appx:///Assets/rss.png", UriKind.Absolute);
        }

        private async Task<string> ExtractContent(HttpResponseMessage response)
        {
            if (response.Content.Headers.ContentEncoding.Contains("gzip"))
            {
                var rsp = await response.Content.ReadAsStreamAsync();
                var gzip = new GZipStream(rsp, CompressionMode.Decompress, false);
                return new StreamReader(gzip).ReadToEnd();
            }
            return await response.Content.ReadAsStringAsync();
        }

        private void AddCandidate(Dictionary<string, int> candidates, string url, int priority)
        {
            if (!candidates.ContainsKey(url) || candidates[url] > priority)
                candidates[url] = priority;
        }

        partial void OnIsSelectedChanged()
        {
            if (IsSelectedChanged != null) IsSelectedChanged(IsSelected);
        }

        #endregion Methods
    }
}

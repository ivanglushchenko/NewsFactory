using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace NewsFactory.UI.Pages
{
    public partial class AddFeedPageModel : PageModel
    {
        #region .ctors

        public AddFeedPageModel()
        {
            Keyword = "";
            SearchCommand = new DelegateCommand(Search, "AddFeedPageModel.Search");
            AddCommand = new DelegateCommand(Add, () => SelectedResult != null, "AddFeedPageModel.AddFeed");
        }

        #endregion .ctors

        #region Properties

        /// <summary>
        /// Gets/sets Keyword.
        /// </summary>
        public string Keyword
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Keyword; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Keyword != value)
                {
                    p_Keyword = value;
                    OnPropertyChanged("Keyword");
                    OnKeywordChanged();
                }
            }
        }
        private string p_Keyword;
        partial void OnKeywordChanged();

        /// <summary>
        /// Gets/sets SearchCommand.
        /// </summary>
        public DelegateCommand SearchCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SearchCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SearchCommand != value)
                {
                    p_SearchCommand = value;
                    OnPropertyChanged("SearchCommand");
                    OnSearchCommandChanged();
                }
            }
        }
        private DelegateCommand p_SearchCommand;
        partial void OnSearchCommandChanged();

        /// <summary>
        /// Gets/sets Results.
        /// </summary>
        public List<Tuple<string, string, string>> Results
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Results; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Results != value)
                {
                    p_Results = value;
                    OnPropertyChanged("Results");
                    OnResultsChanged();
                }
            }
        }
        private List<Tuple<string, string, string>> p_Results;
        partial void OnResultsChanged();

        /// <summary>
        /// Gets/sets SelectedResult.
        /// </summary>
        public Tuple<string, string, string> SelectedResult
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SelectedResult; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SelectedResult != value)
                {
                    p_SelectedResult = value;
                    OnPropertyChanged("SelectedResult");
                    OnSelectedResultChanged();
                }
            }
        }
        private Tuple<string, string, string> p_SelectedResult;
        partial void OnSelectedResultChanged();

        /// <summary>
        /// Gets/sets AddCommand.
        /// </summary>
        public DelegateCommand AddCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AddCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AddCommand != value)
                {
                    p_AddCommand = value;
                    OnPropertyChanged("AddCommand");
                    OnAddCommandChanged();
                }
            }
        }
        private DelegateCommand p_AddCommand;
        partial void OnAddCommandChanged();

        #endregion Properties

        #region Methods

        [DebuggerStepThrough]
        private async void Search()
        {
            IsBusy = true;
            Results = null;
            SelectedResult = null;

            try
            {
                var httpClient = new HttpClient();
                var uri = UriHelper.ToUri(Keyword);
                if (uri != null)
                {
                    var response = await httpClient.GetAsync(uri);
                    if (response.IsSuccessStatusCode)
                    {
                        Results = new List<Tuple<string, string, string>>() { new Tuple<string, string, string>(Keyword, Keyword, null) };
                        IsBusy = false;
                        return;
                    }
                }

                var content = await httpClient.GetStringAsync(string.Format("https://www.google.com/search?q={0}", (Keyword ?? string.Empty).Replace(" ", "%20")));
                var tasks = GetCitations(content).Select(t => Task.Run(async () =>
                {
                    return await GetPage(t);
                })).ToArray();
                Task.WaitAll(tasks);

                var rssUrls = tasks.Select(t => t.Result).Where(t => t != null).SelectMany(t => t).ToList();
                var rssTasks = rssUrls.Select(t => Task.Run(async () =>
                {
                    return await GetRss(t);
                })).ToArray();
                Task.WaitAll(rssTasks);

                var ht = new Dictionary<string, Tuple<string, string, string>>();
                foreach (var item in rssTasks.Where(t => t.Result != null).Select(t => t.Result))
                {
                    ht[item.Item1] = item;
                }
                Results = ht.Select(t => t.Value).Take(5).ToList();
            }
            catch (Exception exc)
            {
                LogService.Error(exc, "Searching for RSS feed {0}", Keyword);
            }

            IsBusy = false;
        }

        [DebuggerStepThrough]
        private static async Task<Tuple<string, string, string>> GetRss(string t)
        {
            try
            {
                var client = new SyndicationClient();
                client.BypassCacheOnRetrieve = true;
                // Although most HTTP servers do not require User-Agent header, others will reject the request or return
                // a different response if this header is missing. Use SetRequestHeader() to add custom headers.
                client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");

                var sf = default(SyndicationFeed);
                try
                {
                    sf = await client.RetrieveFeedAsync(UriHelper.ToUri(t));
                    return new Tuple<string, string, string>(t, sf.Title.Text, sf.Subtitle != null ? sf.Subtitle.Text : null);
                }
                catch (Exception)
                {
                }
            }
            catch
            {
            }
            return null;
        }

        [DebuggerStepThrough]
        private static async Task<List<string>> GetPage(string t)
        {
            try
            {
                var client = new HttpClient();
                var url = t;
                if (!url.StartsWith("http://") && !url.StartsWith("https://"))
                {
                    if (!url.StartsWith("www."))
                        url = "www." + url;
                    url = "http://" + url;
                }
                var page = await client.GetStringAsync(url);
                var links = XamlConverter.GetTagAttributeBySpecificAttribute(page, "link", "rel", "alternate", "href");
                if (links != null)
                {
                    var parsedUrl = new Uri(url);
                    var linkPrefix = parsedUrl.LocalPath;
                    if (!linkPrefix.EndsWith("/"))
                        linkPrefix = linkPrefix + "/";
                    return links.Select(l =>
                        {
                            if (l != null && l.StartsWith("/"))
                            {
                                if (l.StartsWith(linkPrefix))
                                    return (url.EndsWith("/") ? url : url + "/") + l.Substring(linkPrefix.Length);
                                return url.EndsWith("/") ? (url + l.Substring(1)) : (url + l);
                            }
                            return l;
                        }).Where(t2 => t2 != null).OrderBy(t2 => t2.Contains("rss") || t2.Contains("atom") || t2.Contains("feed") ? 0 : t2.Length).Take(3).ToList();
                }
                return null;
            }
            catch
            {
            }
            return null;
        }

        private static IEnumerable<string> GetCitations(string content)
        {
            var index = 0;
            while ((index = content.IndexOf("<cite>", index)) >= 0)
            {
                var end = content.IndexOf("</cite>", index);
                if (end > index)
                {
                    yield return content.Substring(index + 6, end - index - 6).Replace("<b>", "").Replace("</b>", "");
                    index = end + 7;
                }
                else
                    break;
            }
        }

        private async void Add()
        {
            if (SelectedResult == null) return;
            IsBusy = true;

            await DataService.NewsStore.UpdateOne(new FeedInfo() { Url = SelectedResult.Item1.ToUri() });
            MsgService.HideDialog();
        }

        partial void OnSelectedResultChanged()
        {
            AddCommand.RaiseCanExecuteChanged();
        }

        protected override void OnIsBusyChanged()
        {
            base.OnIsBusyChanged();
            AddCommand.RaiseCanExecuteChanged();
        }

        string GetHost(string url)
        {
            try
            {
                var u = new Uri(url);
                return u.Host;
            }
            catch
            {
            }
            return null;
        }

        #endregion Methods
    }
}

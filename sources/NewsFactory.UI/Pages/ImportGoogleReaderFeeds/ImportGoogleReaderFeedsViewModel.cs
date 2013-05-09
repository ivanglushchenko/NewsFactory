using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using NewsFactory.UI.Pages.ImportFeeds;
using NewsFactory.UI.Pages.MessageContainer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace NewsFactory.UI.Pages.ImportGoogleReaderFeeds
{
    public partial class ImportGoogleReaderFeedsViewModel : PageModel
    {
        #region .ctors

        public ImportGoogleReaderFeedsViewModel()
        {
            //Email = "franklyfrankly999@gmail.com";
            //Password = "bnggG4%2566fdsdnfssafas";

            ImportCommand = new DelegateCommand(Import);
        }

        #endregion .ctors

        #region Properties

        /// <summary>
        /// Gets/sets ImportCommand.
        /// </summary>
        public DelegateCommand ImportCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ImportCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ImportCommand != value)
                {
                    p_ImportCommand = value;
                    OnPropertyChanged("ImportCommand");
                    OnImportCommandChanged();
                }
            }
        }
        private DelegateCommand p_ImportCommand;
        partial void OnImportCommandChanged();

        /// <summary>
        /// Gets/sets Email.
        /// </summary>
        public string Email
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Email; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Email != value)
                {
                    p_Email = value;
                    OnPropertyChanged("Email");
                    OnEmailChanged();
                }
            }
        }
        private string p_Email;
        partial void OnEmailChanged();

        /// <summary>
        /// Gets/sets Password.
        /// </summary>
        public string Password
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Password; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Password != value)
                {
                    p_Password = value;
                    OnPropertyChanged("Password");
                    OnPasswordChanged();
                }
            }
        }
        private string p_Password;
        partial void OnPasswordChanged();

        /// <summary>
        /// Gets/sets ErrorMessage.
        /// </summary>
        public string ErrorMessage
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ErrorMessage; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ErrorMessage != value)
                {
                    p_ErrorMessage = value;
                    OnPropertyChanged("ErrorMessage");
                    OnErrorMessageChanged();
                }
            }
        }
        private string p_ErrorMessage;
        partial void OnErrorMessageChanged();

        #endregion Properties

        #region Methods

        private async void Import()
        {
            IsBusy = true;

            try
            {
                var handler = new HttpClientHandler();
                handler.UseDefaultCredentials = true;
                handler.AllowAutoRedirect = false;

                HttpContent httpContent = new StringContent(string.Format("Email={0}&Passwd={1}&service=reader&source=yourapp&continue=http://www.google.com", Email, Password));
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                var client = new HttpClient(handler);
                var response = await client.PostAsync("https://www.google.com/accounts/ClientLogin", httpContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var lines = responseContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    var auth = lines.First(t => t.StartsWith("Auth")).Substring(5);

                    client.DefaultRequestHeaders.Add("Authorization", "GoogleLogin auth=" + auth);

                    var content = await client.GetStringAsync("http://www.google.com/reader/api/0/subscription/list?output=xml");
                    var doc = XDocument.Parse(content);
                    var feeds =
                        doc.Document.Root.Descendants("list").Elements("object")
                        .Select(f => new Tuple<string, string>(f.Descendants("string").Where(e => e.Attribute("name") != null && e.Attribute("name").Value == "id").Select(e => FormatFeedUrl(e.Value)).FirstOrDefault(), f.Descendants("string").Where(e => e.Attribute("name") != null && e.Attribute("name").Value == "title").Select(e => e.Value).FirstOrDefault()))
                        .Where(f => !f.Item1.StartsWith("user/"))
                        .ToList();

                    var view = new ImportFeedsView();
                    view.Model.SetFeeds(feeds);
                    MsgService.ShowDialog(new MessageContainerView() { InternalContent = view });
                }
                else
                    ErrorMessage = "Nope. Please check your credentials and try again.";
            }
            catch (Exception exc)
            {
                LogService.Error(exc, "Cannot connect to Google Reader");
                ErrorMessage = "Cannot connect to Google Reader";
            }

            IsBusy = false;
        }

        private string FormatFeedUrl(string t)
        {
            if (t == null) return t;

            if (t.StartsWith("feed/http://"))
                return t.Substring(5);

            return t;
        }

        #endregion Methods
    }
}

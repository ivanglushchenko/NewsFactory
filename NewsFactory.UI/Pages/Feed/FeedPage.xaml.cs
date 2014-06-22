using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace NewsFactory.UI.Pages.Feed
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class FeedPage : NewsFactory.UI.Common.LayoutAwarePage
    {
        #region .ctors

        public FeedPage()
        {
            this.InitializeComponent();

            DataContext = new FeedPageModel();

            Unloaded += FeedPage_Unloaded;
            XamlConverter.XamlUpdated += OnImagesDownloaded;

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(400);
            _timer.Tick += _timer_Tick;

            webView.NavigationCompleted += webView_NavigationCompleted;

            Loaded += FeedPage_Loaded;
        }

        #endregion .ctors

        #region Fields

        private int _isReadingPaneInUse;
        private ScrollViewer _scrollViewer;
        private ScrollBar _scrollBar;
        private bool _selectedItemChanged;
        private DispatcherTimer _timer;

        #endregion Fields

        #region Properties

        private FeedPageModel Model { get { return (FeedPageModel)DataContext; } }

        #endregion Properties

        #region Methods

        void FeedPage_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= FeedPage_Loaded;
            _scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(itemListView, 0), 0);
            _scrollBar = (ScrollBar)VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(VisualTreeHelper.GetChild(_scrollViewer, 0), 0), 1);
            _scrollBar.ValueChanged += _scrollBar_ValueChanged;
        }

        void _scrollBar_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (_selectedItemChanged)
            {
                _selectedItemChanged = false;
                var newValue = 
                    e.NewValue > e.OldValue
                    ? _scrollViewer.VerticalOffset + _scrollViewer.ViewportHeight * 0.6
                    : _scrollViewer.VerticalOffset - _scrollViewer.ViewportHeight * 0.6;
                newValue = Math.Floor(newValue);
                _scrollViewer.ScrollToVerticalOffset(newValue);
            }
        }

        void webView_NavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            Model.IsBusy = false;
        }

        void _timer_Tick(object sender, object e)
        {
            _timer.Stop();
            _selectedItemChanged = false;
            SetContent();
        }

        void FeedPage_Unloaded(object sender, RoutedEventArgs e)
        {
            rtbView.Blocks.Clear();
            XamlConverter.XamlUpdated -= OnImagesDownloaded;
            webView.NavigationCompleted -= webView_NavigationCompleted;
            _timer.Tick -= _timer_Tick;
            _timer.Stop();
            _scrollBar.ValueChanged -= _scrollBar_ValueChanged;
            _scrollBar = null;
            _scrollViewer = null;
        }

        private void OnImagesDownloaded(NewsItem item, List<Paragraph> paragraph)
        {
            _timer.Stop();
            _timer.Start();
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            if (navigationParameter is NewsFeed)
                Model.SetFeed((NewsFeed)navigationParameter);
            else
                Model.SetFeedUrl((navigationParameter ?? string.Empty).ToString());
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            Model.SaveState();
        }

        private void ItemListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (e.AddedItems != null && e.AddedItems.Count > 0)
                {
                    new XamlConverter().AssignXamlToItem(Model.SelectedItem);

                    if (Model.UseWebView)
                    {
                        rtbView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        webView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    }
                    else
                    {
                        rtbView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                        webView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    }

                    itemListView.ScrollIntoView(Model.SelectedItem);

                    _selectedItemChanged = true;
                }
                else
                {
                    rtbView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    webView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }

            _timer.Stop();
            _timer.Start();
        }

        private void SetContent()
        {
            var isInUse = Interlocked.Exchange(ref _isReadingPaneInUse, 1);
            if (isInUse == 0)
            {
                try
                {
                    if (Model.SelectedItem != null)
                    {
                        if (rtbView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                        {
                            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(rtbView); i++)
                            {
                                var t = VisualTreeHelper.GetChild(rtbView, i);
                            }

                            rtbView.Blocks.Clear();

                            if (Model.SelectedItem.DescriptionXamlUpdated != null)
                            {
                                foreach (var item in Model.SelectedItem.DescriptionXamlUpdated)
                                {
                                    rtbView.Blocks.Add(item);
                                }
                            }
                            else
                            {
                                foreach (var item in Model.SelectedItem.DescriptionXaml)
                                {
                                    rtbView.Blocks.Add(item);
                                }
                            }
                        }
                        if (webView.Visibility == Windows.UI.Xaml.Visibility.Visible)
                        {
                            try
                            {
                                Model.IsBusy = true;
                                webView.Navigate(Model.SelectedItem.Url);
                            }
                            catch
                            {
                                Model.IsBusy = false;
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    LogService.Error(exc);
                }
                Interlocked.Exchange(ref _isReadingPaneInUse, 0);
            }
        }

        private void OnToggleButtonClick(object sender, RoutedEventArgs e)
        {
            ToggleButton button = (ToggleButton)sender;
            VisualStateManager.GoToState(button, button.IsChecked.Value ? "Checked" : "Unchecked", false);
        }

        private async void OnOpen(object sender, RoutedEventArgs e)
        {
            abBottom.IsOpen = false;

            if (Model.SelectedItem != null)
            {
                rtbView.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                webView.Visibility = Windows.UI.Xaml.Visibility.Visible;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => webView.Navigate(Model.SelectedItem.Url));
            }
        }

        private void OnUrlClick(object sender, RoutedEventArgs e)
        {
            Model.OpenArticleCommand.Execute(null);
        }

        private void OnAppBarTapped(object sender, TappedRoutedEventArgs e)
        {
            var appBar = sender as AppBar;
            if (appBar != null)
            {
                appBar.IsOpen = false;
            }
        }

        private void Image_ImageOpened_1(object sender, RoutedEventArgs e)
        {
            var image = sender as Image;
            if (image != null)
            {
                var bs = image.Source as BitmapSource;
                if (bs != null)
                {
                    image.Width = bs.PixelWidth;
                    image.Height = bs.PixelHeight;
                }
            }
        }

        private void CloseAppBar(object sender, RoutedEventArgs e)
        {
            abBottom.IsOpen = false;
        }

        private async void OnMore(object sender, RoutedEventArgs e)
        {
            try
            {
                var popupMenu = new PopupMenu();

                popupMenu.Commands.Add(new UICommand("Copy news item's URL to clipboard", h =>
                {
                    abBottom.IsOpen = false;
                    Model.RaiseCommand(Model.CopyNewsItemUrlToClipboardCommand);
                }));
                popupMenu.Commands.Add(new UICommand("Copy news feed's URL to clipboard", h =>
                {
                    abBottom.IsOpen = false;
                    Model.RaiseCommand(Model.CopyNewsFeedUrlToClipboardCommand);
                }));
                popupMenu.Commands.Add(new UICommandSeparator());
                popupMenu.Commands.Add(new UICommand("Delete feed", h =>
                {
                    abBottom.IsOpen = false;
                    Model.RaiseCommand(Model.DeleteFeedCommand);
                }));

                var button = (Button)sender;
                var transform = button.TransformToVisual(this);
                var point = transform.TransformPoint(new Point(45, -10));

                await popupMenu.ShowAsync(point);
            }
            catch (Exception exc)
            {
                LogService.Error(exc);
            }
        }

        private void OnPin(object sender, RoutedEventArgs e)
        {
            var btn = (FrameworkElement)sender;
            var buttonTransform = btn.TransformToVisual(null);
            var point = buttonTransform.TransformPoint(new Point());
            var rect = new Rect(point, new Size(100, 100));

            Model.PinCommand.Execute(rect);
        }

        private void OnUnpin(object sender, RoutedEventArgs e)
        {
            var btn = (FrameworkElement)sender;
            var buttonTransform = btn.TransformToVisual(null);
            var point = buttonTransform.TransformPoint(new Point());
            var rect = new Rect(point, new Size(100, 100));

            Model.UnpinCommand.Execute(rect);
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            if (NavigationService.IsMainPageCreated)
                base.GoBack(sender, e);
            else
                Model.NavigationService.NavigateTo<MainPage>();
        }

        #endregion Methods
    }
}

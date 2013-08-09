using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace NewsFactory.UI.Pages.FeedSettings
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class FeedSettingsPage : NewsFactory.UI.Common.LayoutAwarePage
    {
        #region .ctors

        public FeedSettingsPage()
        {
            this.InitializeComponent();

            DataContext = new FeedSettingsPageModel();
        }

        #endregion .ctors

        #region Properties

        public FeedSettingsPageModel Model { get { return (FeedSettingsPageModel)DataContext; } }

        #endregion Properties

        #region Methods

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
            // TODO: Assign a bindable collection of items to this.DefaultViewModel["Items"]
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //await new Windows.UI.Popups.MessageDialog("Hey, it's your first time using the Reader, congratilations! Please use the bottom app bar to add feeds").ShowAsync();
        }

        private void OnFeedClick(object sender, ItemClickEventArgs e)
        {
        }

        protected override void GoBack(object sender, RoutedEventArgs e)
        {
            var parent = this.Parent as Popup;
            if (parent != null)
                parent.IsOpen = false;

            // If the app is not snapped, then the back button shows the Settings pane again.
            if (ApplicationView.Value != ApplicationViewState.Snapped)
            {
                SettingsPane.Show();
            }

            base.GoBack(sender, e);
        }

        #endregion Methods
    }
}

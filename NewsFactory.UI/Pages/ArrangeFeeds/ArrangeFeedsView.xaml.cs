using NewsFactory.Foundation.Base;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace NewsFactory.UI.Pages.ArrangeFeeds
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ArrangeFeedsView : NewsFactory.UI.Common.LayoutAwarePage
    {
        #region .ctors

        public ArrangeFeedsView()
        {
            this.InitializeComponent();

            var m = new ArrangeFeedsViewModel();
            DataContext = m;

            m.SelectedFeedChanged += m_SelectedFeedChanged;
        }

        #endregion .ctors

        #region Properties

        private ArrangeFeedsViewModel Model { get { return (ArrangeFeedsViewModel)DataContext; } }

        #endregion Properties

        #region Methods

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

        private void m_SelectedFeedChanged(object sender, EventArgs e)
        {
            if (Model.SelectedFeed != null)
                lvFeeds.ScrollIntoView(Model.SelectedFeed);
        }

        #endregion Methods
    }
}
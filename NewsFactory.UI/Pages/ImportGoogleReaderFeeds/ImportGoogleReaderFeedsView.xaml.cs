using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace NewsFactory.UI.Pages.ImportGoogleReaderFeeds
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class ImportGoogleReaderFeedsView : NewsFactory.UI.Common.LayoutAwarePage
    {
        #region .ctors

        public ImportGoogleReaderFeedsView()
        {
            this.InitializeComponent();

            DataContext = new ImportGoogleReaderFeedsViewModel();
        }

        #endregion .ctors

        #region Properties

        private ImportGoogleReaderFeedsViewModel Model { get { return (ImportGoogleReaderFeedsViewModel)DataContext; } }

        #endregion Properties

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            //Model.Password = ((PasswordBox)sender).Password
        }

        #region Methods
        #endregion Methods
    }
}

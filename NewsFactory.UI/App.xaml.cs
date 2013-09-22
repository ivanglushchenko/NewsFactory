using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Services;
using NewsFactory.Foundation.Utils;
using NewsFactory.Tasks;
using NewsFactory.UI.Common;
using NewsFactory.UI.Pages;
using NewsFactory.UI.Pages.Feed;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.StartScreen;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Split App template is documented at http://go.microsoft.com/fwlink/?LinkId=234228

namespace NewsFactory.UI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        #region .ctors

        /// <summary>
        /// Initializes the singleton Application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        #endregion .ctors

        #region Methods

        private async void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogService.Error(e.Exception);

            var crashLog = await ApplicationData.Current.LocalFolder.CreateFileAsync("log.crash", CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(crashLog, e.Exception.ToString());
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            DataService.Instance.Dispatcher = Window.Current.Dispatcher;
            await DataService.Instance.Init();

            var tileArgs = args.Arguments;
            var rootFrame = Window.Current.Content as Frame;
            var isFreshStart = rootFrame == null;
            var isSecondaryTileCmd = !string.IsNullOrWhiteSpace(tileArgs);

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active

            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                //Associate the frame with a SuspensionManager key                                
                SuspensionManager.RegisterFrame(rootFrame, "AppFrame");

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if ((isSecondaryTileCmd && !rootFrame.Navigate(typeof(FeedPage), tileArgs)) ||
                    (!isSecondaryTileCmd && !rootFrame.Navigate(typeof(MainPage))))
                {
                    throw new Exception("Failed to create initial page");
                }
            }

            if (!isFreshStart && isSecondaryTileCmd)
                rootFrame.Navigate(typeof(FeedPage), tileArgs);

            // Ensure the current window is active
            Window.Current.Activate();

            if (isFreshStart)
            {
                try
                {
                    var secodaryTiles = await SecondaryTile.FindAllAsync();
                    if (secodaryTiles.Count > 0)
                        DownloadFeedTask.RegisterBackgroundTask(DataService.Instance.Settings.SecondaryTileUpdateInterval);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await DataService.Instance.FeedsStore.Save();
            deferral.Complete();
        }

        #endregion Methods
    }
}

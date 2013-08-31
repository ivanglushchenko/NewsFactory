using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Windows.System.Threading;
using Windows.UI.Notifications;

namespace NewsFactory.Tasks
{
    public sealed class DownloadFeedTask : IBackgroundTask
    {
        #region Methods

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();
            UpdateStatusAndTime();
            deferral.Complete();
        }

        private void UpdateStatusAndTime()
        {
            var tileContent = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText03);
            var tileLines = tileContent.SelectNodes("tile/visual/binding/text");
            var networkStatus = NetworkInformation.GetInternetConnectionProfile();

            tileLines[0].InnerText = (networkStatus == null) ? "No network" : networkStatus.GetNetworkConnectivityLevel().ToString();
            tileLines[1].InnerText = DateTime.Now.ToString("MM/dd/yyyy");
            tileLines[2].InnerText = DateTime.Now.ToString("HH:mm:ss");

            var notification = new TileNotification(tileContent);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        #endregion Methods
    }
}

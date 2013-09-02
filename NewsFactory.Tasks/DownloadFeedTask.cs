using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Notifications;
using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Windows.System.Threading;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;

namespace NewsFactory.Tasks
{
    public sealed class DownloadFeedTask : IBackgroundTask
    {
        #region Fields

        private const string TASK_NAME = "Download Feeds Task";

        #endregion Fields

        #region Methods

        public static async void RegisterBackgroundTask(int refreshInterval)
        {
            try
            {
                var status = await BackgroundExecutionManager.RequestAccessAsync();
                if (status == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity || status == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity)
                {
                    if (BackgroundTaskRegistration.AllTasks.Any(x => x.Value.Name == TASK_NAME))
                        BackgroundTaskRegistration.AllTasks.First(x => x.Value.Name == TASK_NAME).Value.Unregister(true);

                    var builder = new BackgroundTaskBuilder
                    {
                        Name = TASK_NAME,
                        TaskEntryPoint = "NewsFactory.Tasks.DownloadFeedTask"
                    };
                    builder.SetTrigger(new TimeTrigger((uint)refreshInterval, false));
                    builder.AddCondition(new SystemCondition(SystemConditionType.InternetAvailable));
                    builder.Register();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex);
            }
        }

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            var deferral = taskInstance.GetDeferral();

            var tiles = await SecondaryTile.FindAllAsync();
            var newItemsCount = 0;
            if (tiles.Count > 0)
            {
                await DataService.Instance.Init();
                var feedsStore = await FeedsStore.Get(DataService.Instance.Settings);
                foreach (var item in tiles)
                {
                    var feedUrl = new Uri(item.Arguments, UriKind.Absolute);
                    if (feedsStore.NewsFeedsMap.ContainsKey(feedUrl))
                    {
                        var newItems = await feedsStore.NewsFeedsMap[feedUrl].DownloadFeed();
                        if (newItems.Count > 0)
                            UpdateFeedTile(feedsStore.NewsFeedsMap[feedUrl].Id, newItems.Count, newItems.First().Title);
                        newItemsCount += newItems.Count;
                    }
                }
            }
            UpdateAppTile(string.Format("New items: {0}", newItemsCount));

            deferral.Complete();
        }

        private void UpdateAppTile(string msg)
        {
            var tileContent = TileUpdateManager.GetTemplateContent(TileTemplateType.TileSquareText03);
            var tileLines = tileContent.SelectNodes("tile/visual/binding/text");
            var networkStatus = NetworkInformation.GetInternetConnectionProfile();

            tileLines[0].InnerText = msg;
            tileLines[1].InnerText = DateTime.Now.ToString("MM/dd/yyyy");
            tileLines[2].InnerText = DateTime.Now.ToString("HH:mm:ss");

            var notification = new TileNotification(tileContent);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private void UpdateFeedTile(string tileId, int newItemsCount, string msg)
        {
            var badgeContent = new BadgeNumericNotificationContent((uint)newItemsCount);
            BadgeUpdateManager.CreateBadgeUpdaterForSecondaryTile(tileId).Update(badgeContent.CreateNotification());

            var tileContent = TileContentFactory.CreateTileSquareText04();
            tileContent.TextBodyWrap.Text = msg;

            TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileId).Update(tileContent.CreateNotification());
        }

        #endregion Methods
    }
}

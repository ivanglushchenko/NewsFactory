using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace NewsFactory.Foundation.Services
{
    public class LogService
    {
        #region Fields

        private static StorageFile _logFile;
        private static JobAggregator<string> _queue = new JobAggregator<string>(ProcessData);

        #endregion Fields

        #region Methods

        public async static Task Init()
        {
            await ApplicationData.Current.LocalFolder.Move("log", "log.prev");
            _logFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("log", CreationCollisionOption.ReplaceExisting);
        }

        public static void Info(string template, params object[] args)
        {
            _queue.Add(string.Format("{0}  {1}", DateTime.Now.ToString("MM/dd HH:mm:ss.fff"), string.Format(template, args)));
        }

        public static void Error(Exception exception, string template = null, params object[] args)
        {
            try
            {
                if (template != null)
                    _queue.Add(string.Format("{0}  {1}. Reason: {2}", DateTime.Now.ToString("MM/dd HH:mm:ss.fff"), string.Format(template, args), exception.Message));
                else
                    _queue.Add(string.Format("{0}  {1}", DateTime.Now.ToString("MM/dd HH:mm:ss.fff"), exception.ToString()));
            }
            catch
            {
            }
        }

        private async static Task ProcessData(List<string> data)
        {
            await FileIO.AppendLinesAsync(_logFile, data);
        }

        #endregion Methods
    }
}

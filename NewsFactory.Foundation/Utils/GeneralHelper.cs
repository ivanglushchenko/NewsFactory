using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;

namespace NewsFactory.Foundation.Utils
{
    public static class GeneralHelper
    {
        #region Methods

        public static void CopyToClipboard(string content)
        {
            try
            {
                var dataPackage = new DataPackage();
                dataPackage.SetText(content);
                Clipboard.SetContent(dataPackage);
            }
            catch (Exception exc)
            {
                LogService.Error(exc, "Cannot copy to clipboard the value {0}", content);
            }
        }

        public static Task Run(this TaskFactory factory, Func<Task> func, bool showMessage = false)
        {
            return factory.StartNew(async () =>
                {
                    var exc = default(Exception);
                    try
                    {
                        await func();
                    }
                    catch (Exception _exc)
                    {
                        exc = _exc;
                    }
                    if (exc != null)
                    {
                        LogService.Error(exc, exc.ToString());
                        if (showMessage)
                        {
                            await DataService.Instance.Invoke(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                                {
                                    await new Windows.UI.Popups.MessageDialog(exc.Message).ShowAsync();
                                });
                        }
                    }
                });
        }

        public static Task Run(Func<Task> func, bool showMessage = false)
        {
            return Task.Run(async () =>
            {
                var exc = default(Exception);
                try
                {
                    await func();
                }
                catch (Exception _exc)
                {
                    exc = _exc;
                }
                if (exc != null)
                {
                    LogService.Error(exc, exc.ToString());
                    if (showMessage)
                    {
                        await DataService.Instance.Invoke(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                        {
                            await new Windows.UI.Popups.MessageDialog(exc.Message).ShowAsync();
                        });
                    }
                }
            });
        }

        public static Task<T> Run<T>(Func<Task<T>> func, bool showMessage = false)
        {
            return Task.Run(async () =>
            {
                var exc = default(Exception);
                try
                {
                    return await func();
                }
                catch (Exception _exc)
                {
                    exc = _exc;
                }
                if (exc != null)
                {
                    LogService.Error(exc, exc.ToString());
                    if (showMessage)
                    {
                        await DataService.Instance.Invoke(Windows.UI.Core.CoreDispatcherPriority.High, async () =>
                        {
                            await new Windows.UI.Popups.MessageDialog(exc.Message).ShowAsync();
                        });
                    }
                }
                return default(T);
            });
        }

        #endregion Methods
    }
}

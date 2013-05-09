using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace NewsFactory.Foundation.Services
{
    public class MsgService
    {
        private LayoutAwarePage _container;

        public async Task Show(string msg)
        {
            await new Windows.UI.Popups.MessageDialog(msg).ShowAsync();
        }

        public async Task<Buttons> Ask(string msg, Buttons buttons = Buttons.Close, Buttons defaultButton = Buttons.None)
        {
            var md = new MessageDialog(msg);
            var result = Buttons.None;
            if ((buttons & Buttons.Ok) == Buttons.Ok)
            {
                md.Commands.Add(new UICommand("Ok", new UICommandInvokedHandler((cmd) => result = Buttons.Ok)));
                if ((defaultButton & Buttons.Ok) == Buttons.Ok)
                    md.DefaultCommandIndex = (uint)(md.Commands.Count - 1);
            }
            if ((buttons & Buttons.Yes) == Buttons.Yes)
            {
                md.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler((cmd) => result = Buttons.Yes)));
                if ((defaultButton & Buttons.Yes) == Buttons.Yes)
                    md.DefaultCommandIndex = (uint)(md.Commands.Count - 1);
            }
            if ((buttons & Buttons.No) == Buttons.No)
            {
                md.Commands.Add(new UICommand("No", new UICommandInvokedHandler((cmd) => result = Buttons.No)));
                if ((defaultButton & Buttons.No) == Buttons.No)
                    md.DefaultCommandIndex = (uint)(md.Commands.Count - 1);
            }
            if ((buttons & Buttons.Cancel) == Buttons.Cancel)
            {
                md.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler((cmd) => result = Buttons.Cancel)));
                if ((defaultButton & Buttons.Cancel) == Buttons.Cancel)
                    md.DefaultCommandIndex = (uint)(md.Commands.Count - 1);
            }
            if ((buttons & Buttons.Close) == Buttons.Close)
            {
                md.Commands.Add(new UICommand("Close", new UICommandInvokedHandler((cmd) => result = Buttons.Close)));
                if ((defaultButton & Buttons.Close) == Buttons.Close)
                    md.DefaultCommandIndex = (uint)(md.Commands.Count - 1);
            }
            await md.ShowAsync();
            return result;
        }

        public void ShowDialog(LayoutAwarePage container)
        {
            HideDialog();

            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                var page = rootFrame.Content as Page;
                if (page != null)
                {
                    var grid = page.Content as Grid;
                    if (grid != null)
                    {
                        var border = new Border()
                            {
                                Background = new SolidColorBrush(Color.FromArgb(114, 0, 0, 0)),
                                HorizontalAlignment = HorizontalAlignment.Stretch,
                                VerticalAlignment = VerticalAlignment.Stretch,
                                Child = container
                            };
                        Grid.SetColumnSpan(border, grid.ColumnDefinitions.Count + 1);
                        Grid.SetRowSpan(border, grid.RowDefinitions.Count + 1);
                        grid.Children.Add(border);

                        container.SetBackCommand(async () =>
                        {
                            await grid.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => grid.Children.Remove(border));
                        });

                        _container = container;
                    }
                }
            }
        }

        public void HideDialog()
        {
            if (_container != null)
            {
                _container.GoBackCommand();
                _container = null;
            }
        }
    }

    [Flags]
    public enum Buttons
    {
        None = 0,
        Yes = 1,
        No = 2,
        Cancel = 4,
        Ok = 8,
        Close = 16,
        YesNo = Yes | No,
        YesNoCancel = Yes | No | Cancel,
        OkCancel = Ok | Cancel,
    }
}

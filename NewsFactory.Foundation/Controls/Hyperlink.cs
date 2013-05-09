using NewsFactory.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NewsFactory.Foundation.Controls
{
    public sealed class Hyperlink : Button
    {
        public Hyperlink()
        {
            this.DefaultStyleKey = typeof(Hyperlink);

            UpdateSize = true;
            Command = new DelegateCommand(OnClick);
        }


        public bool UpdateSize { get; set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(Hyperlink), new PropertyMetadata(null));



        public Uri NavigationUrl
        {
            get { return (Uri)GetValue(NavigationUrlProperty); }
            set { SetValue(NavigationUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NavigationUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NavigationUrlProperty =
            DependencyProperty.Register("NavigationUrl", typeof(Uri), typeof(Hyperlink), new PropertyMetadata(null));


        private async void OnClick()
        {
            if (NavigationUrl != null)
                await Launcher.LaunchUriAsync(NavigationUrl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var c = VisualTreeHelper.GetChild(this, 0) as Canvas;
            var t = VisualTreeHelper.GetChild(c, 0) as FrameworkElement;
            c.LayoutUpdated += (s, e) =>
                {
                    if (UpdateSize)
                    {
                        c.Width = t.DesiredSize.Width;
                        c.Height = (t.DesiredSize.Height / 1.3d);
                    }
                };
        }
    }
}

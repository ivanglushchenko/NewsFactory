using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.AppSettings
{
    public class LiveTileRefreshIntervalCaptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mins = (int)value;
            var opt = SliderOption.LiveTileIntervals.FirstOrDefault(i => i.Interval == mins);
            if (opt != null) return opt.Caption;
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var opt = SliderOption.LiveTileIntervals.FirstOrDefault(i => i.Caption == (string)value);
            if (opt != null)
                return opt.Interval;
            return int.MinValue;
        }
    }
}

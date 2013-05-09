using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.AppSettings
{
    public class RefreshIntervalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var mins = (int)value;
            var opt = SliderOption.Intervals.FirstOrDefault(i => i.Interval == mins);
            if (opt != null) return opt.Value;
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var opt = SliderOption.Intervals.FirstOrDefault(i => i.Value == (int)(double)value);
            if (opt != null)return opt.Interval;
            return int.MinValue;
        }
    }
}

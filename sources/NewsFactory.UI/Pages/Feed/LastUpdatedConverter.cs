using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.Feed
{
    public class LastUpdatedConverter : IValueConverter
    {
        private const double AVG_DAYS_IN_MONTH = 30.41666666666;

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var dt = DateTime.Now - (DateTime)value;
            if (dt.TotalHours < 0)
                return "";
            if (dt.TotalHours < 1)
            {
                if ((int)dt.TotalMinutes == 1)
                    return string.Format(",  {0} minute ago", (int)dt.TotalMinutes);
                else
                    return string.Format(",  {0} minutes ago", (int)dt.TotalMinutes);
            }

            if (dt.TotalDays < 1)
            {
                if ((int)dt.TotalHours == 1)
                    return string.Format(",  {0} hour ago", (int)dt.TotalHours);
                else
                    return string.Format(",  {0} hours ago", (int)dt.TotalHours);
            }
            if ((int)dt.TotalDays < AVG_DAYS_IN_MONTH)
            {
                if ((int)dt.TotalDays == 1)
                    return string.Format(",  {0} day ago", (int)dt.TotalDays);
                else
                    return string.Format(",  {0} days ago", (int)dt.TotalDays);
            }
            if ((int)(dt.TotalDays / AVG_DAYS_IN_MONTH) == 1)
                return string.Format(",  1 month ago");
            else
                return string.Format(",  {0} months ago", (int)(dt.TotalDays / AVG_DAYS_IN_MONTH));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

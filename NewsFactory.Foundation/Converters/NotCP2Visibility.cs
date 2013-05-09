using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace NewsFactory.Foundation.Converters
{
    public partial class NotCP2Visibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter == null)
                return value == null ? Visibility.Collapsed : Visibility.Visible;
            if (value == null)
                return Visibility.Visible;

            return value.ToString() == parameter.ToString() ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

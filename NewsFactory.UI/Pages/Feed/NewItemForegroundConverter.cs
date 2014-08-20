using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NewsFactory.UI.Pages.Feed
{
    public class NewItemForegroundConverter : IValueConverter
    {
        #region Fields

        static readonly Brush _newItemForeground = new SolidColorBrush(Colors.White);
        static readonly Brush _oldItemForeground = new SolidColorBrush(Colors.Gray);

        #endregion Fields
        
        #region Methods

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (bool)value ? _newItemForeground : _oldItemForeground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Model;
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
        static readonly Brush _newItemSubTitleSelectedForeground = new SolidColorBrush(Colors.Gainsboro);
        static readonly Brush _newItemSubTitleForeground = new SolidColorBrush(Colors.DarkGray);
        static readonly Brush _oldItemForeground = new SolidColorBrush(Colors.Gray);

        #endregion Fields
        
        #region Methods

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var rm = (ItemRenderingMode)value;
            switch (rm)
            {
                case ItemRenderingMode.Selected:
                    return parameter == null ? _newItemForeground : _newItemSubTitleSelectedForeground;
                case ItemRenderingMode.NotSelectedNew:
                    return parameter == null ? _newItemForeground : _newItemSubTitleForeground;
                case ItemRenderingMode.NotSelectedOld:
                    return _oldItemForeground;
                default:
                    throw new NotSupportedException();
            }
            //if (parameter == null)
            //    return (bool)value ? _newItemForeground : _oldItemForeground;
            //return (bool)value ? _newItemSubTitleForeground: _oldItemForeground;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NewsFactory.Foundation.Converters
{
    public class CategoryColorConverter : IValueConverter
    {
        #region .ctors

        static CategoryColorConverter()
        {
            foreach (var item in Enum.GetValues(typeof(Category)).OfType<Category>())
            {
                _brushes[item] = ToBrush(ToColor(item));
            }
            _brushes[Category.None] = new SolidColorBrush(Colors.Transparent);
        }

        #endregion .ctors

        #region Fields

        private static Dictionary<Category, SolidColorBrush> _brushes = new Dictionary<Category, SolidColorBrush>();

        #endregion Fields

        #region Methods

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var cat = value is Category ? (Category)value : (Category)Enum.Parse(typeof(Category), value.ToString());
            if (_brushes.ContainsKey(cat))
                return _brushes[cat];
            return _brushes[Category.None];
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

        private static Color LowerAlpha(Color c)
        {
            return Color.FromArgb(40, c.R, c.G, c.B);
        }

        private static SolidColorBrush ToBrush(Color c)
        {
            return new SolidColorBrush(LowerAlpha(c));
        }

        private static Color ToColor(Category cat)
        {
            switch (cat)
            {
                case Category.Animals:
                    return Colors.SkyBlue;
                case Category.ArtAndPhotography:
                    return Colors.Tan;
                case Category.BestOfYouTube:
                    break;
                case Category.Books:
                    return Colors.Teal;
                case Category.Business:
                    return Colors.Sienna;
                case Category.Cars:
                    return Colors.Wheat;
                case Category.CelebrityGossip:
                    return Colors.Yellow;
                case Category.College:
                    return Colors.YellowGreen;
                case Category.ComicBooks:
                    return Colors.Tomato;
                case Category.Dating:
                    return Colors.Thistle;
                case Category.Deals:
                    return Colors.Tan;
                case Category.Design:
                    return Colors.SteelBlue;
                case Category.Fashion:
                    return Colors.SlateGray;
                case Category.FoodAndWine:
                    return Colors.SandyBrown;
                case Category.Gaming:
                    return Colors.RoyalBlue;
                case Category.Gardening:
                    return Colors.Plum;
                case Category.Health:
                    return Colors.PapayaWhip;
                case Category.Humor:
                    return Colors.Green;
                case Category.Movies:
                    return Colors.Orange;
                case Category.Music:
                    return Colors.PaleGoldenrod;
                case Category.Politics:
                    return Colors.BlueViolet;
                case Category.Programming:
                    return Colors.Blue;
                case Category.Science:
                    return Colors.Brown;
                case Category.Sports:
                    return Colors.Purple;
                case Category.Technology:
                    return Colors.Red;
                case Category.Travel:
                    return Colors.OliveDrab;
                case Category.TV:
                    return Colors.Olive;
                case Category.USNews:
                    return Colors.Moccasin;
                case Category.WorldNews:
                    return Colors.OrangeRed;
                case Category.Yoga:
                    return Colors.MintCream;
                case Category.Custom1:
                    break;
                case Category.Custom2:
                    break;
                case Category.Custom3:
                    break;
                default:
                    break;
            }
            return Colors.Transparent;
        }

        // Blue
        // BlueViolet
        // Brown
        // Green
        // Orange
        // OrangeRed
        // Purple
        // Red
        // Sienna

        #endregion Methods
    }
}
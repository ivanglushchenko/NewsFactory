using NewsFactory.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NewsFactory.Foundation.Services
{
    public class NavigationService : Service
    {
        #region Properties

        public static bool IsMainPageCreated { get; set; }

        #endregion Properties

        #region Methods

        public void NavigateTo<T>()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(typeof(T));
            }
        }

        public void NavigateTo(Type viewerType, object arg)
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.Navigate(viewerType, arg);
            }
        }

        public void GoBack()
        {
            var rootFrame = Window.Current.Content as Frame;
            if (rootFrame != null)
            {
                rootFrame.GoBack();
            }
        }

        #endregion Methods
    }
}

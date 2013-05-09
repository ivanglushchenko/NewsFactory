using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;

namespace NewsFactory.Foundation.Controls
{
    public class BetterToggleButton : ToggleButton
    {
        public BetterToggleButton()
        {
            this.Click += AppBarToggleButton_Click;
            
        }



        public bool IsToggled
        {
            get { return (bool)GetValue(IsToggledProperty); }
            set { SetValue(IsToggledProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsToggled.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsToggledProperty =
            DependencyProperty.Register("IsToggled", typeof(bool), typeof(BetterToggleButton), new PropertyMetadata(false, (s, e) =>
                {
                    if ((bool)e.NewValue)
                        VisualStateManager.GoToState((BetterToggleButton)s, "Checked", false);
                    else
                        VisualStateManager.GoToState((BetterToggleButton)s, "Unchecked", false);
                }));


        void AppBarToggleButton_Click(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, IsChecked.Value ? "Checked" : "Unchecked", false);
        }
    }
}

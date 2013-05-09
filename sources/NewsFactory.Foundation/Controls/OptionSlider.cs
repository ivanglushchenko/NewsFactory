using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace NewsFactory.Foundation.Controls
{
    public sealed class OptionSlider : Control
    {
        #region .ctors

        public OptionSlider()
        {
            this.DefaultStyleKey = typeof(OptionSlider);
        }

        #endregion .ctors

        #region Properties

        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Caption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register("Caption", typeof(string), typeof(OptionSlider), new PropertyMetadata(null));



        public List<Option> Options
        {
            get { return (List<Option>)GetValue(OptionsProperty); }
            set { SetValue(OptionsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Options.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OptionsProperty =
            DependencyProperty.Register("Options", typeof(List<Option>), typeof(OptionSlider), new PropertyMetadata(null, (s, e) =>
                {
                    ((OptionSlider)s).OnOptionsChanged(e);
                }));

        private void OnOptionsChanged(DependencyPropertyChangedEventArgs e)
        {
            if (Options != null)
                ValueConverter = new OptionConverter(Options);

            SetupSlider();
            UpdateTargetCaption();
        }

        private void SetupSlider()
        {
            if (Options != null)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    Options[i].SliderValue = i;
                }

                if (_slider != null)
                    _slider.Maximum = Options.Count * 2;
            }
        }

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int), typeof(OptionSlider), new PropertyMetadata(int.MinValue, (s, e) =>
            {
                ((OptionSlider)s).OnValueChanged(e);
            }));

        private void OnValueChanged(DependencyPropertyChangedEventArgs e)
        {
            UpdateTargetCaption();
        }

        private void UpdateTargetCaption()
        {
            if (Options != null && _slider != null)
            {
                var opt = Options.FirstOrDefault(o => o.Value == Value);
                if (opt != null)
                {
                    TargetCaption = opt.Caption;
                    //_slider.Value = opt.SliderValue;
                }
            }
        }



        public string TargetCaption
        {
            get { return (string)GetValue(TargetCaptionProperty); }
            set { SetValue(TargetCaptionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TargetCaption.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TargetCaptionProperty =
            DependencyProperty.Register("TargetCaption", typeof(string), typeof(OptionSlider), new PropertyMetadata(null));



        public IValueConverter ValueConverter
        {
            get { return (IValueConverter)GetValue(ValueConverterProperty); }
            set { SetValue(ValueConverterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ValueConverter.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueConverterProperty =
            DependencyProperty.Register("ValueConverter", typeof(IValueConverter), typeof(OptionSlider), new PropertyMetadata(null));



        #endregion Properties


        private Slider _slider;

        #region Methods

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var c1 = VisualTreeHelper.GetChild(this, 0) as Border;
            var c2 = VisualTreeHelper.GetChild(c1, 0) as StackPanel;
            _slider = VisualTreeHelper.GetChild(c2, 1) as Slider;
            //if (_slider != null)
            //    _slider.ValueChanged += _slider_ValueChanged;

            SetupSlider();
            UpdateTargetCaption();
        }

        void _slider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            //if (Options == null) return;

            //var opt = Options.FirstOrDefault(t => t.SliderValue == (int)e.NewValue);
            //if (opt != null)
            //{
                
            //    Value = opt.Value;
            //}
        }

        #endregion Methods
    }
}

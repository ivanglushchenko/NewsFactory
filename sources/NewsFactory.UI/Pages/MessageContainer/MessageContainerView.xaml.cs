
using NewsFactory.UI.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Items Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234233

namespace NewsFactory.UI.Pages.MessageContainer
{
    /// <summary>
    /// A page that displays a collection of item previews.  In the Split Application this page
    /// is used to display and select one of the available groups.
    /// </summary>
    public sealed partial class MessageContainerView : LayoutAwarePage
    {
        #region .ctors

        public MessageContainerView()
        {
            this.InitializeComponent();

            DataContext = new MessageContainerViewModel();
        }

        #endregion .ctors

        #region Properties

        public object InternalContent
        {
            get { return (object)GetValue(InternalContentProperty); }
            set { SetValue(InternalContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InternalContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InternalContentProperty =
            DependencyProperty.Register("InternalContent", typeof(object), typeof(MessageContainerView), new PropertyMetadata(null));



        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(MessageContainerView), new PropertyMetadata(null));



        private MessageContainerViewModel Model { get { return (MessageContainerViewModel)DataContext; } }

        #endregion Properties

        #region Methods

        public override void SetBackCommand(Action action)
        {
            base.SetBackCommand(action);

            if (InternalContent is LayoutAwarePage)
            {
                (InternalContent as LayoutAwarePage).SetBackCommand(action);
            }
        }

        #endregion Methods
    }
}

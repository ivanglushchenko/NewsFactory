using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Model;
using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace NewsFactory.Foundation.Base
{
    public partial class PageModel : TrackableObject, IDisposable
    {
        #region .ctors

        public PageModel()
        {
        }

        #endregion .ctors

        #region Fields

        private bool _isSettingsEventRegistered;
        private List<Tuple<string, UICommandInvokedHandler>> _settings = new List<Tuple<string, UICommandInvokedHandler>>();

        // Desired width for the settings UI. UI guidelines specify this should be 346 or 646 depending on your needs.
        private double _settingsWidth = 646;
        private Popup _settingsPopup;

        #endregion Fields

        #region Properties

        public DataService DataService
        {
            get
            {
                return DataService.Instance;
            }
        }

        protected NavigationService NavigationService
        {
            get
            {
                if (_navigationService == null) _navigationService = new NavigationService();
                return _navigationService;
            }
        }
        private static NavigationService _navigationService;

        protected MsgService MsgService
        {
            get
            {
                if (_msgService == null) _msgService = new MsgService();
                return _msgService;
            }
        }
        private static MsgService _msgService;

        /// <summary>
        /// Gets/sets IsBusy.
        /// </summary>
        public bool IsBusy
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsBusy; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsBusy != value)
                {
                    p_IsBusy = value;
                    OnPropertyChanged("IsBusy");
                    OnIsBusyChanged();
                }
            }
        }
        private bool p_IsBusy;
        protected virtual void OnIsBusyChanged() { }

        /// <summary>
        /// Gets/sets Status.
        /// </summary>
        public string Status
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Status; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Status != value)
                {
                    p_Status = value;
                    OnPropertyChanged("Status");
                    OnStatusChanged();
                }
            }
        }
        private string p_Status;
        partial void OnStatusChanged();

        /// <summary>
        /// Gets/sets AddsSettings.
        /// </summary>
        public bool AddsSettings
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AddsSettings; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AddsSettings != value)
                {
                    p_AddsSettings = value;
                    OnPropertyChanged("AddsSettings");
                    OnAddsSettingsChanged();
                }
            }
        }
        private bool p_AddsSettings;
        partial void OnAddsSettingsChanged();

        public Settings Settings
        {
            get { return DataService.Settings; }
        }

        public NewsStore NewsStore
        {
            get { return DataService.NewsStore; }
        }

        #endregion Properties

        #region Methods

        public async Task RaiseCommandAsync(AsyncDelegateCommand command)
        {
            if (command != null)
            {
                if (command.CanExecute(null))
                    await command.ExecuteAsync();
            }
        }

        public void RaiseCommand(ICommand command)
        {
            if (command != null)
            {
                if (command.CanExecute(null))
                    command.Execute(null);
            }
        }

        public void RaiseCommand(ICommand command, object arg)
        {
            if (command != null)
            {
                if (command.CanExecute(arg))
                    command.Execute(arg);
            }
        }

        public virtual void SaveState()
        {
        }

        public virtual void Dispose()
        {
            if (_isSettingsEventRegistered)
            {
                SettingsPane.GetForCurrentView().CommandsRequested -= OnSettingsCommandsRequested;
                _isSettingsEventRegistered = false;
            }
        }

        partial void OnAddsSettingsChanged()
        {
            if (AddsSettings)
            {
                if (_isSettingsEventRegistered == false)
                {
                    SettingsPane.GetForCurrentView().CommandsRequested += OnSettingsCommandsRequested;
                    _isSettingsEventRegistered = true;
                }
            }
        }

        protected virtual void OnSettingsCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            foreach (var item in _settings)
            {
                args.Request.ApplicationCommands.Add(new SettingsCommand(item.Item1, item.Item1, item.Item2));
            }
        }

        protected void AddSettingsPane(string caption, UICommandInvokedHandler handler)
        {
            AddsSettings = true;
            _settings.Add(new Tuple<string, UICommandInvokedHandler>(caption, handler));
        }

        protected void AddSettingsPane<T>(string caption)
            where T : Page, new()
        {
            AddSettingsPane(caption, () => new T());
        }

        protected void AddSettingsPane(string caption, Func<Page> getPage)
        {
            AddsSettings = true;
            _settings.Add(new Tuple<string, UICommandInvokedHandler>(caption, command =>
            {            // Create a Popup window which will contain our flyout.
                _settingsPopup = new Popup();
                _settingsPopup.Closed += OnPopupClosed;
                Window.Current.Activated += OnWindowActivated;
                _settingsPopup.IsLightDismissEnabled = true;
                _settingsPopup.Width = _settingsWidth;
                _settingsPopup.Height = Window.Current.Bounds.Height;

                // Add the proper animation for the panel.
                _settingsPopup.ChildTransitions = new TransitionCollection();
                _settingsPopup.ChildTransitions.Add(new PaneThemeTransition()
                {
                    Edge = (SettingsPane.Edge == SettingsEdgeLocation.Right) ?
                           EdgeTransitionLocation.Right :
                           EdgeTransitionLocation.Left
                });

                // Create a SettingsFlyout the same dimenssions as the Popup.
                var mypane = getPage();
                mypane.Width = _settingsWidth;
                mypane.Height = Window.Current.Bounds.Height;

                // Place the SettingsFlyout inside our Popup window.
                _settingsPopup.Child = mypane;

                // Let's define the location of our Popup.
                _settingsPopup.SetValue(Canvas.LeftProperty, SettingsPane.Edge == SettingsEdgeLocation.Right ? (Window.Current.Bounds.Width - _settingsWidth) : 0);
                _settingsPopup.SetValue(Canvas.TopProperty, 0);
                _settingsPopup.IsOpen = true;
            }));
        }

        private void OnWindowActivated(object sender, Windows.UI.Core.WindowActivatedEventArgs e)
        {
            if (e.WindowActivationState == Windows.UI.Core.CoreWindowActivationState.Deactivated)
            {
                _settingsPopup.IsOpen = false;
            }
        }

        void OnPopupClosed(object sender, object e)
        {
            var popup = sender as Popup;
            if (popup.Child is FrameworkElement && (popup.Child as FrameworkElement).DataContext is IDisposable)
                ((popup.Child as FrameworkElement).DataContext as IDisposable).Dispose();

            Window.Current.Activated -= OnWindowActivated;
        }

        partial void OnStatusChanged()
        {
            IsBusy = Status != null;
        }

        #endregion Methods
    }
}

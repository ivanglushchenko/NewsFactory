using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.ArrangeFeeds
{
    [Bindable]
    public partial class ArrangeFeedsViewModel : PageModel
    {
        #region .ctors

        public ArrangeFeedsViewModel()
        {
            Feeds = DataService.FeedsStore.NewsFeeds;
            IsAutoSortedByName = DataService.Settings.FeedOrderMode == FeedOrderMode.SortedAlphabetically;
            IsManualOrdering = DataService.Settings.FeedOrderMode == FeedOrderMode.Manual;
            MoveFeedUpCommand = new DelegateCommand(MoveFeedUp);
            MoveFeedDownCommand = new DelegateCommand(MoveFeedDown);
            SortFeedsCommand = new DelegateCommand(SortFeeds);

            _hasChanges = false;
        }

        #endregion .ctors

        #region Events

        public event EventHandler SelectedFeedChanged;

        #endregion Events

        #region Fields

        private bool _hasChanges;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets IsAutoSortedByName.
        /// </summary>
        public bool IsAutoSortedByName
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsAutoSortedByName; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsAutoSortedByName != value)
                {
                    p_IsAutoSortedByName = value;
                    OnPropertyChanged("IsAutoSortedByName");
                    OnIsAutoSortedByNameChanged();
                }
            }
        }
        private bool p_IsAutoSortedByName;
        partial void OnIsAutoSortedByNameChanged();

        /// <summary>
        /// Gets/sets IsManualOrdering.
        /// </summary>
        public bool IsManualOrdering
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_IsManualOrdering; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_IsManualOrdering != value)
                {
                    p_IsManualOrdering = value;
                    OnPropertyChanged("IsManualOrdering");
                    OnIsManualOrderingChanged();
                }
            }
        }
        private bool p_IsManualOrdering;
        partial void OnIsManualOrderingChanged();

        /// <summary>
        /// Gets/sets Feeds.
        /// </summary>
        public ObservableCollection<NewsFeed> Feeds
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Feeds; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Feeds != value)
                {
                    p_Feeds = value;
                    OnPropertyChanged("Feeds");
                    OnFeedsChanged();
                }
            }
        }
        private ObservableCollection<NewsFeed> p_Feeds;
        partial void OnFeedsChanged();

        /// <summary>
        /// Gets/sets SelectedFeed.
        /// </summary>
        public NewsFeed SelectedFeed
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SelectedFeed; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SelectedFeed != value)
                {
                    p_SelectedFeed = value;
                    OnPropertyChanged("SelectedFeed");
                    OnSelectedFeedChanged();
                }
            }
        }
        private NewsFeed p_SelectedFeed;
        partial void OnSelectedFeedChanged();

        /// <summary>
        /// Gets/sets MoveFeedUpCommand.
        /// </summary>
        public DelegateCommand MoveFeedUpCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_MoveFeedUpCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_MoveFeedUpCommand != value)
                {
                    p_MoveFeedUpCommand = value;
                    OnPropertyChanged("MoveFeedUpCommand");
                    OnMoveFeedUpCommandChanged();
                }
            }
        }
        private DelegateCommand p_MoveFeedUpCommand;
        partial void OnMoveFeedUpCommandChanged();

        /// <summary>
        /// Gets/sets MoveFeedDownCommand.
        /// </summary>
        public DelegateCommand MoveFeedDownCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_MoveFeedDownCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_MoveFeedDownCommand != value)
                {
                    p_MoveFeedDownCommand = value;
                    OnPropertyChanged("MoveFeedDownCommand");
                    OnMoveFeedDownCommandChanged();
                }
            }
        }
        private DelegateCommand p_MoveFeedDownCommand;
        partial void OnMoveFeedDownCommandChanged();

        /// <summary>
        /// Gets/sets SortFeedsCommand.
        /// </summary>
        public DelegateCommand SortFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_SortFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_SortFeedsCommand != value)
                {
                    p_SortFeedsCommand = value;
                    OnPropertyChanged("SortFeedsCommand");
                    OnSortFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_SortFeedsCommand;
        partial void OnSortFeedsCommandChanged();

        #endregion Properties

        #region Methods

        public override async void Dispose()
        {
            base.Dispose();

            if (_hasChanges = true && IsAutoSortedByName)
                SortFeeds();

            await DataService.SaveSettings();
            await DataService.FeedsStore.Save();
        }

        partial void OnIsAutoSortedByNameChanged()
        {
            if (IsAutoSortedByName)
            {
                _hasChanges = true;
                DataService.Settings.FeedOrderMode = FeedOrderMode.SortedAlphabetically;
            }
        }

        partial void OnIsManualOrderingChanged()
        {
            if (IsManualOrdering)
            {
                _hasChanges = true;
                DataService.Settings.FeedOrderMode = FeedOrderMode.Manual;
            }
        }

        private async void MoveFeedUp()
        {
            if (SelectedFeed == null) return;

            var i = Feeds.IndexOf(SelectedFeed);
            if (i == 0) return;

            var f = SelectedFeed;
            Feeds.RemoveAt(i);
            Feeds.Insert(i - 1, f);
            SelectedFeed = f;
            DataService.FeedsStore.SwapFeeds(i, i - 1);

            await DataService.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (SelectedFeedChanged != null)
                    SelectedFeedChanged(this, null);
            });
        }

        private async void MoveFeedDown()
        {
            if (SelectedFeed == null) return;

            var i = Feeds.IndexOf(SelectedFeed);
            if (i == Feeds.Count - 1) return;

            var f = SelectedFeed;
            Feeds.RemoveAt(i);
            Feeds.Insert(i + 1, f);
            SelectedFeed = f;
            DataService.FeedsStore.SwapFeeds(i, i + 1);

            await DataService.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (SelectedFeedChanged != null)
                    SelectedFeedChanged(this, null);
            });
        }

        private void SortFeeds()
        {
            QuickSort(Feeds, 0, Feeds.Count - 1);
        }

        private void QuickSort(IList<NewsFeed> array, int left, int right)
        {
            if (right <= left) return;

            var l_hold = left;
            var r_hold = right;
            var pivot = left;
            var pivotEl = array[pivot];

            while (left < right)
            {
                while ((string.Compare(array[right].FeedInfo.Title, array[pivot].FeedInfo.Title) >= 0) && (left < right))
                {
                    right--;
                }

                if (left != right)
                {
                    array[left] = array[right];
                    left++;
                }

                while ((string.Compare(array[left].FeedInfo.Title, array[pivot].FeedInfo.Title) <= 0) && (left < right))
                {
                    left++;
                }

                if (left != right)
                {
                    array[right] = array[left];
                    right--;
                }
            }

            array[left] = pivotEl;
            pivot = left;
            left = l_hold;
            right = r_hold;

            if (left < pivot)
            {
                QuickSort(array, left, pivot - 1);
            }

            if (right > pivot)
            {
                QuickSort(array, pivot + 1, right);
            }
        }

        #endregion Methods
    }
}

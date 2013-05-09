using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsFactory.UI.Pages.ImportFeeds
{
    public partial class ImportFeedsViewModel : PageModel
    {
        #region .ctors

        public ImportFeedsViewModel()
        {
            AddCommand = new DelegateCommand(Add, () => FeedsCount > 0, "ImportFeeds.Add");
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets Feeds.
        /// </summary>
        public List<FeedModel> Feeds
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
        private List<FeedModel> p_Feeds;
        partial void OnFeedsChanged();

        /// <summary>
        /// Gets/sets FeedsCount.
        /// </summary>
        public int FeedsCount
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_FeedsCount; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_FeedsCount != value)
                {
                    p_FeedsCount = value;
                    OnPropertyChanged("FeedsCount");
                    OnFeedsCountChanged();
                }
            }
        }
        private int p_FeedsCount;
        partial void OnFeedsCountChanged();

        /// <summary>
        /// Gets/sets AddCommand.
        /// </summary>
        public DelegateCommand AddCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AddCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AddCommand != value)
                {
                    p_AddCommand = value;
                    OnPropertyChanged("AddCommand");
                    OnAddCommandChanged();
                }
            }
        }
        private DelegateCommand p_AddCommand;
        partial void OnAddCommandChanged();

        /// <summary>
        /// Gets/sets ImportResults.
        /// </summary>
        public ObservableCollection<ImportResult> ImportResults
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ImportResults; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ImportResults != value)
                {
                    p_ImportResults = value;
                    OnPropertyChanged("ImportResults");
                    OnImportResultsChanged();
                }
            }
        }
        private ObservableCollection<ImportResult> p_ImportResults;
        partial void OnImportResultsChanged();

        /// <summary>
        /// Gets/sets RunningTasks.
        /// </summary>
        public int RunningTasks
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_RunningTasks; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_RunningTasks != value)
                {
                    p_RunningTasks = value;
                    OnPropertyChanged("RunningTasks");
                    OnRunningTasksChanged();
                }
            }
        }
        private int p_RunningTasks;
        partial void OnRunningTasksChanged();

        #endregion Properties

        #region Methods

        public void SetFeeds(List<Tuple<string, string>> feeds)
        {
            Feeds = feeds.Select(t => new FeedModel(this) { Title = t.Item2.Beautify(), Url = t.Item1, IsSelected = true }).ToList();
        }

        private void Add()
        {
            MsgService.HideDialog();

            DataService.NewsStore.UpdateMany(Feeds
                        .Where(t => t.IsSelected)
                        .Select(t => t.ToFeedInfo())
                        .ToList());
        }

        #endregion Methods
    }
}

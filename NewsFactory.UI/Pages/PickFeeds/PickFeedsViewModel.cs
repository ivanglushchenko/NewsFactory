using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Components;
using NewsFactory.Foundation.Utils;
using NewsFactory.UI.Pages.ImportFeeds;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using NewsFactory.Foundation.Model;

namespace NewsFactory.UI.Pages.PickFeeds
{
    public partial class PickFeedsViewModel : PageModel
    {
        #region .ctors

        public PickFeedsViewModel()
        {
            AddFeedsCommand = new DelegateCommand(AddFeeds, () => ItemsSelected > 0);

            var names = typeof(PickFeedsViewModel).GetTypeInfo().Assembly.GetManifestResourceNames();
            var name = names.First(t => t.EndsWith(".Feeds.xml"));
            var stream = typeof(PickFeedsViewModel).GetTypeInfo().Assembly.GetManifestResourceStream(name);
            var doc = XDocument.Load(stream);

            Groups = new ObservableCollection<Group>(doc.Document.Root.Elements().Select(el => new Group()
                {
                    Title = el.Attribute("name").Value,
                    Category = (Category)Enum.Parse(typeof(Category), el.Attribute("category") != null ? el.Attribute("category").Value : el.Attribute("name").Value),
                    NewsFeeds = new ObservableCollection<NewsFeed>(el.Elements().Select(f => new NewsFeed(new FeedInfo()
                        {
                            Title = f.Attribute("title").Value,
                            Url = f.Attribute("url").Value.ToUri(),
                            FavIconUrl = f.Attribute("favIconUrl") != null ? f.Attribute("favIconUrl").Value.ToUri() : null,
                            Category = (Category)Enum.Parse(typeof(Category), el.Attribute("category") != null ? el.Attribute("category").Value : el.Attribute("name").Value)
                        }, null, null, null)))
                }));

            foreach (var group in Groups)
            {
                foreach (var item in group.NewsFeeds)
                {
                    item.IsSelectedChanged += item_IsSelectedChanged;
                }
                group.IsSelected = true;
            }
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets/sets Groups.
        /// </summary>
        public ObservableCollection<Group> Groups
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_Groups; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_Groups != value)
                {
                    p_Groups = value;
                    OnPropertyChanged("Groups");
                    OnGroupsChanged();
                }
            }
        }
        private ObservableCollection<Group> p_Groups;
        partial void OnGroupsChanged();

        /// <summary>
        /// Gets/sets ItemsSelected.
        /// </summary>
        public int ItemsSelected
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_ItemsSelected; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_ItemsSelected != value)
                {
                    p_ItemsSelected = value;
                    OnPropertyChanged("ItemsSelected");
                    OnItemsSelectedChanged();
                }
            }
        }
        private int p_ItemsSelected;
        partial void OnItemsSelectedChanged();

        /// <summary>
        /// Gets/sets AddFeedsCommand.
        /// </summary>
        public DelegateCommand AddFeedsCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AddFeedsCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AddFeedsCommand != value)
                {
                    p_AddFeedsCommand = value;
                    OnPropertyChanged("AddFeedsCommand");
                    OnAddFeedsCommandChanged();
                }
            }
        }
        private DelegateCommand p_AddFeedsCommand;
        partial void OnAddFeedsCommandChanged();

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

        private void AddFeeds()
        {
            MsgService.HideDialog();

            DataService.NewsStore.UpdateMany(Groups.SelectMany(g => g.NewsFeeds.Where(f => f.IsSelected)).Select(f => f.FeedInfo).ToList());
        }

        void item_IsSelectedChanged(bool obj)
        {
            if (obj)
                ItemsSelected++;
            else
                ItemsSelected--;
            AddFeedsCommand.RaiseCanExecuteChanged();
        }

        #endregion Methods
    }
}

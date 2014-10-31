using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;

namespace NewsFactory.UI.Pages.Analysis
{
    [Bindable]
    public partial class AnalysisPageModel : PageModel
    {
        #region .ctors

        public AnalysisPageModel()
        {
            Task.Factory.StartNew(AnalyzeNews);
        }

        #endregion .ctors

        #region Properties

        /// <summary>
        /// Gets/sets Status.
        /// </summary>
        public string Status
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Status; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
                    OnStatusChanged();
                }
            }
        }
        string _Status;
        partial void OnStatusChanged();

        /// <summary>
        /// Gets/sets ProgressMax.
        /// </summary>
        public double ProgressMax
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ProgressMax; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ProgressMax != value)
                {
                    _ProgressMax = value;
                    OnPropertyChanged("ProgressMax");
                    OnProgressMaxChanged();
                }
            }
        }
        double _ProgressMax;
        partial void OnProgressMaxChanged();

        /// <summary>
        /// Gets/sets ProgressCurrent.
        /// </summary>
        public double ProgressCurrent
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ProgressCurrent; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ProgressCurrent != value)
                {
                    _ProgressCurrent = value;
                    OnPropertyChanged("ProgressCurrent");
                    OnProgressCurrentChanged();
                }
            }
        }
        double _ProgressCurrent;
        partial void OnProgressCurrentChanged();


        #endregion Properties
        
        #region Methods

        async void AnalyzeNews()
        {
            var wordIndex = new WordIndex();

            var items = DataService.NewsStore.GetItems(DataService.FeedsStore.All).Take(100).ToList();

            await Invoke(() => {
                ProgressMax = items.Count;
                ProgressCurrent = 0;
                Status = "Building a dictionary...";
            });
            
            foreach (var item in items)
            {
                wordIndex.BuildDictionary(item);
                await Invoke(() => ProgressCurrent = ProgressCurrent + 1);
            }

            wordIndex.PrepareWordIndex(0, 0);

            await Invoke(() =>
            {
                ProgressMax = items.Count;
                ProgressCurrent = 0;
                Status = "Calculating document vectors...";
            });

            foreach (var item in items)
            {
                wordIndex.AssignUniformVector(item);
                await Invoke(() => ProgressCurrent = ProgressCurrent + 1);
            }

            await Invoke(() =>
            {
                ProgressMax = items.Count;
                ProgressCurrent = 0;
                Status = "Clustering...";
            });

            wordIndex.Cluster(2);
        }

        async Task Invoke(Action action)
        {
            await DataService.Invoke(CoreDispatcherPriority.Low, new DispatchedHandler(action));
        }

        #endregion Methods
    }
}

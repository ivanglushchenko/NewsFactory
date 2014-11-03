using NewsFactory.Foundation.Base;
using NewsFactory.Foundation.Common;
using NewsFactory.Foundation.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace NewsFactory.UI.Pages.Analysis
{
    [Bindable]
    public partial class AnalysisPageModel : PageModel
    {
        #region .ctors

        public AnalysisPageModel()
        {
            Task.Factory.StartNew(ClusterNews);
            ReclusterCommand = new DelegateCommand(() => Task.Factory.StartNew(ClusterNews), () => Status == null);
        }

        #endregion .ctors

        #region Fields

        WordIndex _wordIndex;

        #endregion Fields
        
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

        /// <summary>
        /// Gets/sets Points.
        /// </summary>
        public List<TestPoint> Points
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Points; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Points != value)
                {
                    _Points = value;
                    OnPropertyChanged("Points");
                    OnPointsChanged();
                }
            }
        }
        List<TestPoint> _Points;
        partial void OnPointsChanged();

        /// <summary>
        /// Gets/sets ClustersCount.
        /// </summary>
        public int ClustersCount
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ClustersCount; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ClustersCount != value)
                {
                    _ClustersCount = value;
                    OnPropertyChanged("ClustersCount");
                    OnClustersCountChanged();
                }
            }
        }
        int _ClustersCount = 2;
        partial void OnClustersCountChanged();

        /// <summary>
        /// Gets/sets Clusters.
        /// </summary>
        public List<DocumentCluster> Clusters
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Clusters; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Clusters != value)
                {
                    _Clusters = value;
                    OnPropertyChanged("Clusters");
                    OnClustersChanged();
                }
            }
        }
        List<DocumentCluster> _Clusters;
        partial void OnClustersChanged();

        /// <summary>
        /// Gets/sets SelectedCluster.
        /// </summary>
        public DocumentCluster SelectedCluster
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _SelectedCluster; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_SelectedCluster != value)
                {
                    _SelectedCluster = value;
                    OnPropertyChanged("SelectedCluster");
                    OnSelectedClusterChanged();
                }
            }
        }
        DocumentCluster _SelectedCluster;
        partial void OnSelectedClusterChanged();

        /// <summary>
        /// Gets/sets ReclusterCommand.
        /// </summary>
        public DelegateCommand ReclusterCommand
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _ReclusterCommand; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_ReclusterCommand != value)
                {
                    _ReclusterCommand = value;
                    OnPropertyChanged("ReclusterCommand");
                    OnReclusterCommandChanged();
                }
            }
        }
        DelegateCommand _ReclusterCommand;
        partial void OnReclusterCommandChanged();

        #endregion Properties
        
        #region Methods

        async void ClusterNews()
        {
            var clustersCount = ClustersCount;
            var items = DataService.NewsStore.GetItems(DataService.FeedsStore.All).ToList();

            if (_wordIndex == null)
            {
                _wordIndex = new WordIndex();

                await ResetProgress(items.Count, "Building a dictionary...");

                foreach (var item in items)
                {
                    _wordIndex.BuildDictionary(item);
                    await IncreaseProgress();
                }
            }

            await ResetProgress(items.Count, "Calculating document vectors...");

            await _wordIndex.PrepareWordIndex(2, 2, IncreaseProgress);

            var iterationCount = 10;
            await ResetProgress(iterationCount, "Clustering...");

            var vectors = items.Select(t => _wordIndex.UniformVectors[t]).ToList();
            var clusters = Clustering.SelectSeedClusters(clustersCount, vectors);
            var assignments = default(int[]);

            for (int i = 0; i < iterationCount; i++)
            {
                assignments = Clustering.Cluster(clusters, vectors);
                await IncreaseProgress();
            }

            var results =
                Enumerable
                .Range(0, assignments.Length)
                .Select(t => new { Assignment = assignments[t], Item = items[t] })
                .GroupBy(t => t.Assignment)
                .Select(t => new DocumentCluster() { NewsItems = t.Select(t2 => t2.Item).ToList() })
                .ToList();

            await Invoke(() =>
                {
                    Status = null;
                    Clusters = results;
                    SelectedCluster = Clusters.FirstOrDefault();
                    ReclusterCommand.RaiseCanExecuteChanged();
                });
        }

        Task ResetProgress(int max, string status)
        {
            return DataService.Invoke(CoreDispatcherPriority.Low, new DispatchedHandler(() =>
                {
                    ProgressCurrent = 0;
                    ProgressMax = max;
                    Status = status;
                    ReclusterCommand.RaiseCanExecuteChanged();
                }));
        }

        Task IncreaseProgress()
        {
            return DataService.Invoke(CoreDispatcherPriority.Low, new DispatchedHandler(() => ProgressCurrent++));
        }

        async Task Invoke(Action action)
        {
            await DataService.Invoke(CoreDispatcherPriority.Low, new DispatchedHandler(action));
        }

        async void TestClustering()
        {
            var rnd = new Random();
            var items = Enumerable.Range(0, 10000).Select(i => new double[] { rnd.NextDouble(), rnd.NextDouble() }).ToList();
            var clusters = Clustering.SelectSeedClusters(2, items);

            await Invoke(() => ShowPoints(items, clusters));

            for (int i = 0; i < 100; i++)
            {
                Clustering.Cluster(clusters, items);
                await Invoke(() => ShowPoints(items, clusters));
            }
        }

        void ShowPoints(List<double[]> points, List<double[]> clusters)
        {
            var p = points.Select(i => new TestPoint() { Size = 4, Left = i[0], Top = i[1], Color = new SolidColorBrush(Colors.Green) }).ToList();
            p.AddRange(clusters.Select(i => new TestPoint() { Size = 9, Left = i[0], Top = i[1], Color = new SolidColorBrush(Colors.Red) }));
            Points = p;
        }

        #endregion Methods
    }

    public partial class TestPoint : TrackableObject
    {
        /// <summary>
        /// Gets/sets Left.
        /// </summary>
        public double Left
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Left; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Left != value)
                {
                    _Left = value;
                    OnPropertyChanged("Left");
                    OnLeftChanged();
                }
            }
        }
        double _Left;
        partial void OnLeftChanged();

        /// <summary>
        /// Gets/sets Top.
        /// </summary>
        public double Top
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Top; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Top != value)
                {
                    _Top = value;
                    OnPropertyChanged("Top");
                    OnTopChanged();
                }
            }
        }
        double _Top;
        partial void OnTopChanged();

        /// <summary>
        /// Gets/sets Name.
        /// </summary>
        public string Name
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Name; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                    OnNameChanged();
                }
            }
        }
        string _Name;
        partial void OnNameChanged();

        /// <summary>
        /// Gets/sets Color.
        /// </summary>
        public Brush Color
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Color; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Color != value)
                {
                    _Color = value;
                    OnPropertyChanged("Color");
                    OnColorChanged();
                }
            }
        }
        Brush _Color;
        partial void OnColorChanged();

        /// <summary>
        /// Gets/sets Size.
        /// </summary>
        public int Size
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return _Size; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (_Size != value)
                {
                    _Size = value;
                    OnPropertyChanged("Size");
                    OnSizeChanged();
                }
            }
        }
        int _Size;
        partial void OnSizeChanged();

    }
}

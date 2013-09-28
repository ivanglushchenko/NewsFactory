using NewsFactory.Foundation.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace NewsFactory.Foundation.Components
{
    public partial class JobAggregator<T>
    {
        #region .ctors

        public JobAggregator(Func<List<T>, Task> processor)
            : this(processor, TimeSpan.Zero)
        {
        }

        public JobAggregator(Func<List<T>, Task> processor, TimeSpan delay)
        {
            AllowBatching = true;
            AllowDuplicates = true;
            _processor = processor;
            _delay = delay;
        }

        #endregion .ctors

        #region Events

        public event Action<JobAggregator<T>> Empty;

        #endregion Events

        #region Fields

        private object _syncObject = new object();
        private bool _isJobRunning = false;
        private Queue<T> _jobData = new Queue<T>();
        private Func<List<T>, Task> _processor;
        private ThreadPoolTimer _timer;
        private TimeSpan _delay;
        private HashSet<T> _hashSet;

        #endregion Fields

        #region Properties

        public bool AllowBatching { get; set; }

        /// <summary>
        /// Gets/sets AllowDuplicates.
        /// </summary>
        public bool AllowDuplicates
        {
            [System.Diagnostics.DebuggerStepThrough]
            get { return p_AllowDuplicates; }
            [System.Diagnostics.DebuggerStepThrough]
            set
            {
                if (p_AllowDuplicates != value)
                {
                    p_AllowDuplicates = value;
                    OnAllowDuplicatesChanged();
                }
            }
        }
        private bool p_AllowDuplicates;
        partial void OnAllowDuplicatesChanged();

        #endregion Properties

        #region Methods

        public void Add(T data)
        {
            lock (_syncObject)
            {
                if (AllowDuplicates || !_hashSet.Contains(data))
                {
                    _jobData.Enqueue(data);
                    if (_hashSet != null) _hashSet.Add(data);
                }
            }

            StartJobIfNeeded();
        }

        public void AddRange(List<T> data)
        {
            lock (_syncObject)
            {
                foreach (var item in data)
                {
                    if (AllowDuplicates || !_hashSet.Contains(item))
                    {
                        _jobData.Enqueue(item);
                        if (_hashSet != null) _hashSet.Add(item);
                    }
                }
            }

            StartJobIfNeeded();
        }

        public void Clear()
        {
            lock (_syncObject)
            {
                _jobData.Clear();
            }
        }

        private void StartJobIfNeeded()
        {
            lock (_syncObject)
            {
                if (!_isJobRunning && _jobData.Count > 0 && _timer == null)
                {
                    if (_delay == TimeSpan.Zero)
                        StartJob();
                    else
                        _timer = ThreadPoolTimer.CreateTimer(OnTimerElapsed, _delay);
                }
                if (_jobData.Count == 0 && Empty != null)
                    Empty(this);
            }
        }

        private void StartJob()
        {
            lock (_syncObject)
            {
                _isJobRunning = true;

                var batchItems = AllowBatching ? _jobData.ToList() : new List<T>() { _jobData.Dequeue() };
                if (AllowBatching)
                    _jobData.Clear();
                if (!AllowDuplicates)
                {
                    foreach (var item in batchItems)
                    {
                        _hashSet.Remove(item);
                    }
                }

                Task.Run(async () =>
                {
                    await _processor(batchItems);
                    lock (_syncObject)
                    {
                        _isJobRunning = false;
                    }

                    StartJobIfNeeded();
                });
            }
        }

        private void OnTimerElapsed(ThreadPoolTimer timer)
        {
            lock (_syncObject)
            {
                _timer = null;
                StartJob();
            }
        }

        partial void OnAllowDuplicatesChanged()
        {
            lock (_syncObject)
            {
                if (AllowDuplicates)
                    _hashSet = null;
                else
                    _hashSet = new HashSet<T>(_jobData);
            }
        }

        #endregion Methods
    }

    public class JobAggregator : JobAggregator<object>
    {
        #region .ctors

        public JobAggregator(Func<Task> processor)
            : this(processor, TimeSpan.Zero)
        {
        }

        public JobAggregator(Func<Task> processor, TimeSpan delay)
            : base(data => processor(), delay)
        {
        }

        #endregion .ctors

        #region Methods

        public void Trigger()
        {
            Add(new object());
        }

        #endregion Methods
    }
}

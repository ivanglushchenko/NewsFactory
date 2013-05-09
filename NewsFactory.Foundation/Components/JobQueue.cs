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
    public class JobAggregator<T>
    {
        #region .ctors

        public JobAggregator(Func<List<T>, Task> processor)
            : this(processor, TimeSpan.Zero)
        {
        }

        public JobAggregator(Func<List<T>, Task> processor, TimeSpan delay)
        {
            _processor = processor;
            _delay = delay;
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();
        private bool _isJobRunning = false;
        private List<T> _jobData = new List<T>();
        private Func<List<T>, Task> _processor;
        private ThreadPoolTimer _timer;
        private TimeSpan _delay;

        #endregion Fields

        #region Methods

        public void Add(T data)
        {
            lock (_syncObject)
            {
                _jobData.Add(data);
            }

            StartJobIfNeeded();
        }

        public void AddRange(List<T> data)
        {
            lock (_syncObject)
            {
                _jobData.AddRange(data);
            }

            StartJobIfNeeded();
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
            }
        }

        private void StartJob()
        {
            lock (_syncObject)
            {
                _isJobRunning = true;

                var t = _jobData;
                _jobData = new List<T>();

                Task.Run(async () =>
                {
                    await _processor(t);
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

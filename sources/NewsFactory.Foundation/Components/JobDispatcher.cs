using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NewsFactory.Foundation.Components
{
    public class JobDispatcher<T>
    {
        #region .ctors

        public JobDispatcher(int maxJobs, Func<T, Task> processor)
        {
            _maxJobs = maxJobs;
            _processor = processor;
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();
        private int _maxJobs;
        private Func<T, Task> _processor;
        private int _currentJobs;
        private AutoResetEvent _event = new AutoResetEvent(false);

        private List<T> _jobs = new List<T>();

        #endregion Fields

        #region Methods

        public Task Start(IEnumerable<T> source)
        {
            lock (_syncObject)
            {
                _jobs.AddRange(source);
            }
            return Task.Run(() =>
            {
                while (true)
                {
                    lock (_syncObject)
                    {
                        if (_maxJobs > _currentJobs)
                        {
                            var nextJobs = _jobs.Take(_maxJobs - _currentJobs);
                            if (nextJobs.Count() > 0)
                            {
                                _jobs = _jobs.Skip(nextJobs.Count()).ToList();
                                _currentJobs += nextJobs.Count();
                                foreach (var nextJob in nextJobs)
                                {
                                    Task.Factory.StartNew(async () =>
                                    {
                                        try
                                        {
                                            await _processor(nextJob);
                                        }
                                        catch (Exception exc)
                                        {
                                            LogService.Error(exc);
                                        }

                                        lock (_syncObject)
                                            _currentJobs--;
                                        _event.Set();
                                        
                                    }, TaskCreationOptions.AttachedToParent);
                                }
                            }
                            else
                            {
                                if (_currentJobs == 0)
                                    break;
                            }
                        }

                    }

                    _event.WaitOne();
                }
            });
        }

        #endregion Methods
    }

    public class JobDispatcher<T, U>
    {
        #region .ctors

        public JobDispatcher(int maxJobs, Func<T, Task<U>> processor)
        {
            _maxJobs = maxJobs;
            _processor = processor;
        }

        #endregion .ctors

        #region Fields

        private object _syncObject = new object();
        private int _maxJobs;
        private Func<T, Task<U>> _processor;
        private int _currentJobs;
        private AutoResetEvent _event = new AutoResetEvent(false);

        private List<T> _jobs = new List<T>();

        #endregion Fields

        #region Methods

        public Task<IEnumerable<U>> Start(IEnumerable<T> source)
        {
            lock (_syncObject)
                _jobs.AddRange(source);

            return Task.Run<IEnumerable<U>>(() =>
            {
                var returns = new List<U>();
                while (true)
                {
                    lock (_syncObject)
                    {
                        if (_maxJobs > _currentJobs)
                        {
                            var nextJobs = _jobs.Take(_maxJobs - _currentJobs);
                            if (nextJobs.Count() > 0)
                            {
                                _jobs = _jobs.Skip(nextJobs.Count()).ToList();
                                _currentJobs += nextJobs.Count();
                                foreach (var nextJob in nextJobs)
                                {
                                    Task.Factory.StartNew(async () =>
                                    {
                                        try
                                        {
                                            var ret = await _processor(nextJob);
                                            if (ret != null)
                                                returns.Add(ret);
                                        }
                                        catch (Exception exc)
                                        {
                                            LogService.Error(exc);
                                        }

                                        lock (_syncObject)
                                            _currentJobs--;
                                        _event.Set();

                                    }, TaskCreationOptions.AttachedToParent);
                                }
                            }
                            else
                            {
                                if (_currentJobs == 0)
                                    break;
                            }
                        }
                    }

                    _event.WaitOne();
                }
                return returns;
            });
        }

        #endregion Methods
    }
}

using NewsFactory.Foundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NewsFactory.Foundation.Base
{
    public class DelegateCommand : ICommand
    {
        #region .ctors

        public DelegateCommand(Action action, Func<bool> canExecute, string description = null)
        {
            _action = action;
            _canExecute = canExecute;
            Description = description;
        }

        public DelegateCommand(Action action, string description = null)
        {
            _action = action;
            Description = description;
        }

        #endregion .ctors

        #region Fields

        private Action _action;
        private Func<bool> _canExecute;

        #endregion Fields

        #region Properties

        public string Description { get; private set; }

        #endregion Properties

        #region Methods

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            try
            {
                _action();
            }
            catch (Exception exc)
            {
                if (Description == null)
                    LogService.Error(exc, null);
                else
                    LogService.Error(exc, "Exc while executing command {0}", Description);
            }
        }

        #endregion Methods
    }

    public class DelegateCommand<T> : ICommand
    {
        #region .ctors

        public DelegateCommand(Action<T> action)
        {
            _action = action;
        }

        #endregion .ctors

        #region Fields

        private Action<T> _action;

        #endregion Fields

        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            _action((T)parameter);
        }

        #endregion Methods
    }

    public class AsyncDelegateCommand : ICommand
    {
        #region .ctors

        public AsyncDelegateCommand(Func<Task> action)
        {
            _action = action;
        }

        #endregion .ctors

        #region Fields

        private Func<Task> _action;

        #endregion Fields

        #region Methods

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null) CanExecuteChanged(this, new EventArgs());
        }

        public void Execute(object parameter)
        {
            _action();
        }

        public async Task ExecuteAsync()
        {
            await _action();
        }

        #endregion Methods
    }
}

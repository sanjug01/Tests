using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// This is a helper class which gives us a simple, generic implementation
    /// of ICommand. Its purpose is to feed it to XAML bindings so we can
    /// bind to actions.
    /// </summary>
    /// <remarks>The class is disposable because it has a disposable member - a reader-writer
    /// monitor used to make subscribing and emitting of the event thread-safe.</remarks>
    public class RelayCommand : Helpers.DisposableObject, ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;
        private readonly ReaderWriterLockSlim _monitor;
        private EventHandler _canExecuteChanged;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            Contract.Requires(execute != null);
            Contract.Ensures(null != _monitor);
            Contract.Ensures(null != _execute);

            _monitor = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            ThrowIfDisposed();
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                ThrowIfDisposed();
                using (Helpers.ReadWriteMonitor.Write(_monitor))
                    _canExecuteChanged += value;
            }

            remove
            {
                ThrowIfDisposed();
                using (Helpers.ReadWriteMonitor.Write(_monitor))
                    _canExecuteChanged -= value;
            }
        }

        public void EmitCanExecuteChanged()
        {
            EventHandler handler;

            ThrowIfDisposed();
            using (Helpers.ReadWriteMonitor.Read(_monitor))
                handler = _canExecuteChanged;

            if(null != handler)
            {
                handler(this, EventArgs.Empty);
            }
        }

        protected override void DisposeManagedState()
        {
            _monitor.Dispose();
        }
    }
}
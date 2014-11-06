using System;
using System.Diagnostics.Contracts;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    // This is a helper class which gives us a simple, generic implementation
    // of ICommand. Its purpose is to feed it to XAML bindings so we can
    // bind to actions.
    public class RelayCommand : ICommand
    {
        readonly Action<object> _execute;
        readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            Contract.Requires(execute != null);

            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null) ? true : _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void EmitCanExecuteChanged(object sender, EventArgs args)
        {
            if(CanExecuteChanged != null)
            {
                CanExecuteChanged(sender, args);
            }
        }
    }
}
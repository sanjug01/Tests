namespace RdClient.Shared.Helpers
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Adapter for ICommand objects that exposes ICommand.CanExecute() as
    /// an observable read-only property CanExecute. The property can be bound in XAML
    /// to properties of visual elements.
    /// </summary>
    public sealed class CommandBinding : MutableObject
    {
        private ICommand _command;
        private object _parameter;
        private bool _canExecute;

        public CommandBinding()
        {
        }

        public bool CanExecute
        {
            get { return _canExecute; }
            private set { SetProperty(ref _canExecute, value); }
        }

        public ICommand Command
        {
            get { return _command; }
            set
            {
                ICommand oldCommand = _command;

                if(SetProperty(ref _command, value))
                {
                    if (null != oldCommand)
                        oldCommand.CanExecuteChanged -= this.OnCanExecuteChanged;

                    if (null != _command)
                    {
                        this.CanExecute = _command.CanExecute(_parameter);
                        _command.CanExecuteChanged += this.OnCanExecuteChanged;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
            }
        }

        public object Parameter
        {
            get { return _parameter; }
            set
            {
                if(SetProperty(ref _parameter, value))
                {
                    this.CanExecute = null != _command ? _command.CanExecute(_parameter) : false;
                }
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            this.CanExecute = ((ICommand)sender).CanExecute(_parameter);
        }
    }
}

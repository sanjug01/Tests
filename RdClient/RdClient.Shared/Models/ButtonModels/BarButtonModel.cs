namespace RdClient.Shared.Models
{
    using System;
    using System.Windows.Input;

    public class BarButtonModel : BarItemModel
    {
        private ICommand _command;
        private bool _canExecute;
        private object _commandParameter;

        private string _labelStringId;

        public string LabelStringId
        {
            get
            {
                return _labelStringId;
            }
            set
            {
                _labelStringId = value;
            }
        }

        public ICommand Command
        {
            get { return _command; }
            set
            {
                ICommand oldCommand = _command;

                if (SetProperty<ICommand>(ref _command, value))
                {
                    if (null != oldCommand)
                        oldCommand.CanExecuteChanged -= this.OnCanExecuteCommandChanged;

                    if (null != _command)
                    {
                        this.CanExecute = _command.CanExecute(_commandParameter);
                        _command.CanExecuteChanged += this.OnCanExecuteCommandChanged;
                    }
                    else
                    {
                        this.CanExecute = false;
                    }
                }
            }
        }

        public object CommandParameter
        {
            get { return _commandParameter; }
            set
            {
                if (SetProperty(ref _commandParameter, value) && null != _command)
                {
                    this.CanExecute = _command.CanExecute(_commandParameter);
                }
            }
        }

        public bool CanExecute
        {
            get { return _canExecute; }
            set
            {
                SetProperty(ref _canExecute, value);
            }
        }

        private void OnCanExecuteCommandChanged(object sender, EventArgs e)
        {
            this.CanExecute = ((ICommand)sender).CanExecute(_commandParameter);
        }
    }
}

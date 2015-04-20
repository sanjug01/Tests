namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Windows.Input;

    public class SymbolBarButtonModel : MutableObject
    {
        private SegoeGlyph _glyph;
        private ICommand _command;
        private object _commandParameter;
        private bool _canExecute;

        public SymbolBarButtonModel()
        {
            _glyph = SegoeGlyph.Home;
            _canExecute = false;
        }

        public SegoeGlyph Glyph
        {
            get { return _glyph; }
            set { SetProperty(ref _glyph, value); }
        }

        public ICommand Command
        {
            get { return _command; }
            set
            {
                ICommand oldCommand = _command;

                if(SetProperty<ICommand>(ref _command, value))
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
                if(SetProperty(ref _commandParameter, value) && null != _command)
                {
                    this.CanExecute = _command.CanExecute(_commandParameter);
                }
            }
        }

        public bool CanExecute
        {
            get { return _canExecute; }
            set { SetProperty(ref _canExecute, value); }
        }

        private void OnCanExecuteCommandChanged(object sender, EventArgs e)
        {
            this.CanExecute = ((ICommand)sender).CanExecute(_commandParameter);
        }
    }
}

namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public abstract class BarButtonModel : BarItemModel
    {
        private readonly ICommand _command;
        private readonly string _label;

        public string Label { get { return _label; } }
        public ICommand Command { get { return _command; } }

        protected BarButtonModel(ICommand command, string label)
        {
            Contract.Requires(null != command);

            _command = command;
            _command.CanExecuteChanged += this.OnCanExecuteCommandChanged;
            _label = label;
            this.IsVisible = _command.CanExecute(null);
        }

        private void OnCanExecuteCommandChanged(object sender, EventArgs e)
        {
            Contract.Assert(object.ReferenceEquals(sender, _command));
            this.IsVisible = _command.CanExecute(null);
        }
    }
}

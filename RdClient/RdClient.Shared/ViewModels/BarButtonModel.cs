namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public abstract class BarButtonModel : BarItemModel
    {
        private readonly ICommand _command;
        private readonly string _labelStringId;

        public string LabelStringId { get { return _labelStringId; } }
        public ICommand Command { get { return _command; } }

        protected BarButtonModel(ICommand command, string labelStringId, ItemAlignment alignment = ItemAlignment.Left) : base(alignment)
        {
            Contract.Requires(null != command);

            _command = command;
            _command.CanExecuteChanged += this.OnCanExecuteCommandChanged;
            _labelStringId = labelStringId;
            this.IsVisible = _command.CanExecute(null);
        }

        private void OnCanExecuteCommandChanged(object sender, EventArgs e)
        {
            Contract.Assert(object.ReferenceEquals(sender, _command));
            this.IsVisible = _command.CanExecute(null);
        }
    }
}

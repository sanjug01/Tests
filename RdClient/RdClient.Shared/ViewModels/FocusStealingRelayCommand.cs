namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Extension of RelayCommand that uses a focus controller object to move input focus to the default UI element
    /// after executing the command.
    /// </summary>
    public sealed class FocusStealingRelayCommand : RelayCommand
    {
        private readonly IInputFocusController _focusController;

        public FocusStealingRelayCommand(IInputFocusController focusController, Action<object> execute, Predicate<object> canExecute = null)
            : base(execute, canExecute)
        {
            Contract.Assert(null != focusController);
            Contract.Ensures(null != _focusController);
            _focusController = focusController;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);
            _focusController.SetDefault();
        }
    }
}

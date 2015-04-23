namespace RdClient.Shared.Navigation
{
    using System.Windows.Input;

    /// <summary>
    /// Optional interface implemented by view models that behave as modal dialogs and provide
    /// commands that submit and dismiss the models' views.
    /// </summary>
    public interface IDialogViewModel
    {
        /// <summary>
        /// Command executed when the Escape key is pressed on the keyboard.
        /// </summary>
        ICommand Cancel { get; }

        /// <summary>
        /// Command executed when the Enter key is pressed on the keyboard.
        /// </summary>
        ICommand DefaultAction { get; }
    }
}

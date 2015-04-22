namespace RdClient.Shared.Input.Keyboard
{
    using System;

    /// <summary>
    /// Interface that mimics the InputPane class for testability and adds some convenience.
    /// </summary>
    public interface IInputPanel
    {
        /// <summary>
        /// Event emitted after the value of the IsVisible property has changed.
        /// </summary>
        /// <remarks>A dedicated event is emitted instead of implementing the INotifyPropertyChanged interface
        /// to simplify tracking of the input panel's visibility.</remarks>
        event EventHandler IsVisibleChanged;

        /// <summary>
        /// Current visibility of the input panel.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Show the panel.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the panel.
        /// </summary>
        void Hide();
    }
}

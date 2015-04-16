namespace RdClient.Shared.Navigation
{
    using System.ComponentModel;

    /// <summary>
    /// Interface of an object that controls visibility of one UI element. The interface is designed to be
    /// passed from view models to controls through a single binding.
    ///  A control then may call methods of the interface in the code behind to change visibility of itself
    ///  of some other UI element.
    /// </summary>
    public interface IViewVisibility : INotifyPropertyChanged
    {
        /// <summary>
        /// Read-only property that indicates the current visibility conrtolled by the object.
        /// </summary>
        bool IsVisible { get; }

        /// <summary>
        /// Show the view - change IsVisible to true.
        /// </summary>
        void Show();

        /// <summary>
        /// Hide the view - change IsVisible to false.
        /// </summary>
        void Hide();
    }
}

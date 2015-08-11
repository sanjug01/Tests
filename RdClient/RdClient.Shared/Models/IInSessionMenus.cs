namespace RdClient.Shared.Models
{
    using System;
    using System.Windows.Input;

    /// <summary>
    /// Interface of a model that backs InSessionMenusViewModel. The model is injected in the view model
    /// when the view is presented.
    /// </summary>
    public interface IInSessionMenus : IDisposable
    {
        /// <summary>
        /// Disconnect the current session. Methot is called when user clicks the "Disconnect" button in the right side bar.
        /// </summary>
        void Disconnect();

        /// <summary>
        /// Command switches the application window to full screen size.
        /// </summary>
        ICommand EnterFullScreen { get; }

        /// <summary>
        /// Command switchws the application window to normal (non-full screen) size.
        /// </summary>
        ICommand ExitFullScreen { get; }

        /// <summary>
        /// Command switches the in-session input mode to "Touch" - local pointer and touch events are forwarded to the server.
        /// </summary>
        ICommand TouchMode { get; }

        /// <summary>
        /// Command switches the in-session input mode to "Pointer" - local pointer interactions move the remote mouse.
        /// </summary>
        ICommand PointerMode { get; }
    }
}

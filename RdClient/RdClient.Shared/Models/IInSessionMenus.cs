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

        ICommand EnterFullScreen { get; }

        ICommand ExitFullScreen { get; }
    }
}

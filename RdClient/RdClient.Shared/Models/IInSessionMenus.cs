namespace RdClient.Shared.Models
{
    /// <summary>
    /// Interface of a model that backs InSessionMenusViewModel. The model is injected in the view model
    /// when the view is presented.
    /// </summary>
    public interface IInSessionMenus
    {
        /// <summary>
        /// Disconnect the current session. Methot is called when user clicks the "Disconnect" button in the right side bar.
        /// </summary>
        void Disconnect();
    }
}

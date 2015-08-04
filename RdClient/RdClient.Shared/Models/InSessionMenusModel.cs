namespace RdClient.Shared.Models
{
    /// <summary>
    /// Model injected in the view model of the in-session menus view.
    /// </summary>
    public sealed class InSessionMenusModel : IInSessionMenus
    {
        private readonly IRemoteSession _session;

        public InSessionMenusModel(IRemoteSession session)
        {
            _session = session;
        }

        void IInSessionMenus.Disconnect()
        {
            _session.Disconnect();
        }
    }
}

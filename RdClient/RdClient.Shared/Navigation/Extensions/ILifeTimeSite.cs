namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.LifeTimeManagement;

    /// <summary>
    /// Interface of a site of the ILifeTimeManager interface.
    /// The interface is used by the LifeTime extension (LifeTimeExtension class)
    /// to attach and detach the ILifeTimeManager object to and from the site.
    /// </summary>
    public interface ILifeTimeSite
    {
        void SetLifeTimeManager(ILifeTimeManager lftManager);
    }
}

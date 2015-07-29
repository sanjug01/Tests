namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;

    /// <summary>
    /// Interface of a site of the IDeferredExecution interface.
    /// The interface is used by the deferred execution extension (DeferredExecutionExtension class)
    /// to attach and detach the IDeferredExecution object to and from the site.
    /// </summary>
    public interface IDeferredExecutionSite
    {
        void SetDeferredExecution(IDeferredExecution dispatcher);
    }
}

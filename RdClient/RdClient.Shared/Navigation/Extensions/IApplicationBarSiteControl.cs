namespace RdClient.Shared.Navigation.Extensions
{
    /// <summary>
    /// Control interface that may be implemented by the application bar site
    /// that supports deactivation (the stock bar site can be deactivated).
    /// Deactivated application bar site object cannot change the application bar.
    /// Deactivation is irreversible - the site cannot be re-activated.
    /// Upon deactivation, the site object may release all resources needed to
    /// update the bar.
    /// </summary>
    public interface IApplicationBarSiteControl
    {
        void Deactivate();
    }
}

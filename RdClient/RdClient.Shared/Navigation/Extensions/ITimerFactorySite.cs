namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;

    public interface ITimerFactorySite
    {
        void SetTimerFactory(ITimerFactory timerFactory);
    }
}

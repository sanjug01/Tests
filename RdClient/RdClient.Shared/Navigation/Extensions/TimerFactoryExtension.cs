namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Navigation extension that injects a shared ITimerFactory object into view models implementing
    /// the ITimerFactorySite interface.
    /// </summary>
    public sealed class TimerFactoryExtension : INavigationExtension
    {
        private readonly ITimerFactory _timerFactory;

        public TimerFactoryExtension(ITimerFactory timerFactory)
        {
            Contract.Assert(null != timerFactory);
            _timerFactory = timerFactory;
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetTimerFactory(_timerFactory));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetTimerFactory(null));
        }
    }
}

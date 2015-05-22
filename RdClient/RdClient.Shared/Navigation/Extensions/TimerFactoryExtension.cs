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
        private readonly ITimerFactory _dispatcherTimerFactory;

        public TimerFactoryExtension(ITimerFactory timerFactory, ITimerFactory dispatcherTimerFactory)
        {
            Contract.Assert(null != timerFactory);
            _timerFactory = timerFactory;
            _dispatcherTimerFactory = dispatcherTimerFactory;
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetTimerFactory(_timerFactory));
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetDispatcherTimerFactory(_dispatcherTimerFactory));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetTimerFactory(null));
            viewModel.CastAndCall<ITimerFactorySite>(site => site.SetDispatcherTimerFactory(null));
        }
    }
}

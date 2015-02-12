namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Telemetry;
    using System;

    public sealed class ApplicationTelemetryExtension : INavigationExtension
    {
        private readonly IApplicationTelemetry _applicationTelemetry;

        public ApplicationTelemetryExtension(IApplicationTelemetry applicationTelemetry)
        {
            _applicationTelemetry = applicationTelemetry ?? new DummyApplicationTelemetry();
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            viewModel.CastAndCall<IApplicationTelemetrySite>(site => site.SetApplicationTelemetry(_applicationTelemetry));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
        }
    }
}

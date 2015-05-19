namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Telemetry;

    public sealed class TelemetryExtension : INavigationExtension
    {
        private ITelemetryClient _client;

        /// <summary>
        /// Telemetry client object injected into the extension in XAML or in the application initialization code.
        /// </summary>
        public ITelemetryClient Client
        {
            get { return _client; }
            set { _client = value; }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            //
            // Attach the telemetry client to the view model if the model wants it.
            //
            viewModel.CastAndCall<ITelemetryClientSite>(site => site.SetTelemetryClient(_client));
        }

        void INavigationExtension.Dismissed(IViewModel viewModel)
        {
            //
            // Do not remove the telemetry client,
            // the view model may need it even when it is not presented.
            //
        }
    }
}

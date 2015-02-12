namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Telemetry;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify ViewModels.
    /// </summary>

    public abstract class ViewModelBase : Helpers.MutableObject, IViewModel, IDataModelSite, IApplicationTelemetrySite
    {
        private INavigationService _navigationService;
        private IModalPresentationContext _presentationContext;
        private ApplicationDataModel _appDataModel;
        private IApplicationTelemetry _applicationTelemetry;
        private ITelemetryActivity _telemetryActivity;

        protected INavigationService NavigationService
        {
            get { return _navigationService; }
            private set { SetProperty<INavigationService>(ref _navigationService, value); }
        }

        protected ApplicationDataModel ApplicationDataModel
        {
            get { return _appDataModel; }
            set { SetProperty(ref _appDataModel, value); }
        }

        protected IApplicationTelemetry ApplicationTelemetry
        {
            get { return _applicationTelemetry; }
            set { SetProperty(ref _applicationTelemetry, value); }
        }


        void IDataModelSite.SetDataModel(ApplicationDataModel dataModel)
        {
            if(null != dataModel)
            {
                this.ApplicationDataModel = dataModel;
            }
        }

        void IApplicationTelemetrySite.SetApplicationTelemetry(IApplicationTelemetry applicationTelemetry)
        {
            this.ApplicationTelemetry = applicationTelemetry;
        }

        /// <summary>
        /// View models may call this method to dismiss themselves from the modal stack.
        /// </summary>
        /// <param name="result">Arbitrary object that will be passed to the presentation completion handler
        /// passed to INavigationService.PushModalView.</param>
        protected void DismissModal(object result)
        {
            Contract.Ensures(null == _presentationContext);

            if (null != _presentationContext)
            {
                _presentationContext.Dismiss(result);
                _presentationContext = null;
            }
        }

        /// <summary>
        /// Overridable called immediately after a navigation service has been attached to the view model.
        /// </summary>
        /// <param name="activationParameter"></param>
        /// <remarks>Default implementation does nothing.</remarks>
        protected virtual void OnPresenting(object activationParameter)
        {
        }

        /// <summary>
        /// Overridable called after detaching the navigation service from the view model.
        /// </summary>
        /// <remarks>
        /// Default implementation does nothing.
        /// After this method has been called, the view model must not use any services that may have
        /// been provided to it by navigation extensions. All extensions will have been called to clean up
        /// the view model before calling OnDismissed.
        /// </remarks>
        protected virtual void OnDismissed()
        {
        }

        void IViewModel.Presenting(INavigationService navigationService, object activationParameter, IModalPresentationContext presentationContext)
        {
            Contract.Requires(navigationService != null);
            Contract.Ensures(null != _navigationService);

            _telemetryActivity = _applicationTelemetry.PresentView(GetType().Name);
            Contract.Assert(null != _telemetryActivity, "IViewModel.Presenting|null telemetry activity returned");
            this.NavigationService = navigationService;
            _presentationContext = presentationContext;
            this.OnPresenting(activationParameter);
        }

        void IViewModel.Dismissing()
        {
            _telemetryActivity.Complete();
            _telemetryActivity = null;
            SetProperty<INavigationService>(ref _navigationService, null, "NavigationService");
            this.OnDismissed();
        }
    }

}

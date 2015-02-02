namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify ViewModels.
    /// </summary>

    public abstract class ViewModelBase : Helpers.MutableObject, IViewModel, IViewModelWithData
    {
        private INavigationService _navigationService;
        private IModalPresentationContext _presentationContext;
        private RdDataModel _dataModel;
        private ApplicationDataModel _appDataModel;

        protected INavigationService NavigationService
        {
            get { return _navigationService; }
            private set { SetProperty<INavigationService>(ref _navigationService, value); }
        }

        public RdDataModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
        }

        void IViewModelWithData.SetDataModel(ApplicationDataModel dataModel)
        {
            _appDataModel = dataModel;
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

            this.NavigationService = navigationService;
            _presentationContext = presentationContext;
            this.OnPresenting(activationParameter);
        }

        void IViewModel.Dismissing()
        {
            SetProperty<INavigationService>(ref _navigationService, null);
            this.OnDismissed();
        }
    }

}

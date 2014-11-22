using RdClient.Shared.Navigation;
using RdClient.Shared.Models;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace RdClient.Shared.ViewModels
{
    /// <summary>
    /// Implementation of <see cref="INotifyPropertyChanged"/> to simplify ViewModels.
    /// </summary>

    public abstract class ViewModelBase : Helpers.MutableObject, IViewModel, IViewModelWithData
    {
        private INavigationService _navigationService;
        private IDataModel _dataModel;

        protected INavigationService NavigationService
        {
            get { return _navigationService; }
            private set { SetProperty<INavigationService>(ref _navigationService, value); }
        }

        public IDataModel DataModel
        {
            get { return _dataModel; }
            set { SetProperty(ref _dataModel, value); }
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

        void IViewModel.Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);
            Contract.Ensures(null != _navigationService);

            this.NavigationService = navigationService;
            this.OnPresenting(activationParameter);
        }

        void IViewModel.Dismissing()
        {
            SetProperty<INavigationService>(ref _navigationService, null);
            this.OnDismissed();
        }
    }

}

using RdClient.Navigation;
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

    public abstract class ViewModelBase : Helpers.MutableObject, IViewModel
    {
        private INavigationService _navigationService;
        protected INavigationService NavigationService
        {
            get { return _navigationService; }
            private set { SetProperty<INavigationService>(ref _navigationService, value); }
        }

        protected abstract void OnPresenting(object activationParameter);

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);
            Contract.Ensures(null != _navigationService);

            this.NavigationService = navigationService;
            OnPresenting(activationParameter);
        }

        public virtual void Dismissing()
        {
            SetProperty<INavigationService>(ref _navigationService, null);
        }
    }

}

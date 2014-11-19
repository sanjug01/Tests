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

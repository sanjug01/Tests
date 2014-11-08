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

    public abstract class ViewModelBase : INotifyPropertyChanged, IViewModel
    {
        private INavigationService _navigationService;
        protected INavigationService NavigationService { get { return _navigationService; } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            Debug.WriteLine("propertyName: {0}, storage: {1}, value: {2}", propertyName, storage, value);

            if (object.Equals(storage, value))
            {
                return false;
            }
            else
            {
                storage = value;
                this.OnPropertyChanged(propertyName);
                return true;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        protected abstract void OnPresenting(object activationParameter);

        public void Presenting(INavigationService navigationService, object activationParameter)
        {
            Contract.Requires(navigationService != null);
            _navigationService = navigationService;
            OnPresenting(activationParameter);
        }

        public virtual void Dismissing()
        {
            _navigationService = null;
        }
    }

}

namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Windows.Input;

    /// <summary>
    /// Object passed to IApplicationBarItemsSource.GetItems implemented by view models as a proxy that controls
    /// the application bar. The object checks
    /// </summary>
    sealed class ApplicationBarSite : MutableObject, IApplicationBarSite
    {
        private readonly IApplicationBarViewModel _barViewModel;
        private readonly Predicate<object> _canChangeViewModel;
        private readonly object _predicateParameter;
        private readonly ICommand _showBarProxiedCommand;
        private readonly Action _hideBarAction;
        private readonly RelayCommand _showBarCommand, _hideBarCommand;

        public static IApplicationBarSite Create(IApplicationBarViewModel barViewModel,
            Predicate<object> canChangeViewModel, object predicateParameter,
            ICommand showBar,
            Action hideBar)
        {
            return new ApplicationBarSite(barViewModel, canChangeViewModel, predicateParameter, showBar, hideBar);
        }

        private ApplicationBarSite(IApplicationBarViewModel barViewModel,
            Predicate<object> canChangeViewModel, object predicateParameter,
            ICommand showBar,
            Action hideBar)
        {
            _barViewModel = barViewModel;
            _barViewModel.PropertyChanged += this.OnBarPropertyChanged;
            _canChangeViewModel = canChangeViewModel;
            _predicateParameter = predicateParameter;
            _showBarProxiedCommand = showBar;
            _hideBarAction = hideBar;
            _showBarCommand = new RelayCommand(o => ShowBar(), o => _canChangeViewModel(_predicateParameter) && _showBarProxiedCommand.CanExecute(null));
            _hideBarCommand = new RelayCommand(o => HideBar(), o => _canChangeViewModel(_predicateParameter));
        }

        bool IApplicationBarSite.IsBarSticky
        {
            get { return _barViewModel.IsBarSticky; }
            set
            {
                //
                // Change the property of the view model if the predicate permits.
                // If the view model's property will change, OnBarPropertyChanged will re-emit the change notification
                // event on behalf of the site object.
                //
                if (_canChangeViewModel(_predicateParameter))
                    _barViewModel.IsBarSticky = value;
            }
        }

        bool IApplicationBarSite.IsBarVisible { get { return _barViewModel.IsBarVisible; } }

        ICommand IApplicationBarSite.ShowBar { get { return _showBarCommand; } }

        ICommand IApplicationBarSite.HideBar { get { return _hideBarCommand; } }

        private void OnBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsBarSticky") || e.PropertyName.Equals("IsBarVisible"))
                EmitPropertyChanged(e);
        }

        private void ShowBar()
        {
            if (_canChangeViewModel(_predicateParameter) && _showBarProxiedCommand.CanExecute(null))
                _showBarProxiedCommand.Execute(null);
        }
        private void HideBar()
        {
            if (_canChangeViewModel(_predicateParameter))
                _hideBarAction();
        }
    }
}

namespace RdClient.Shared.Navigation
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// Object passed to IApplicationBarItemsSource.GetItems implemented by view models as a proxy that controls
    /// the application bar. The object checks
    /// </summary>
    sealed class ApplicationBarSite : MutableObject, IApplicationBarSite, IApplicationBarSiteControl
    {
        private readonly static string[] _forwardedPropertiesNames;
        private IApplicationBarViewModel _barViewModel;
        private ICommand _showBarProxiedCommand;
        private Action _hideBarAction;
        private readonly RelayCommand _showBarCommand, _hideBarCommand;

        public static IApplicationBarSite Create(IApplicationBarViewModel barViewModel, ICommand showBar, Action hideBar)
        {
            Contract.Requires(null != barViewModel);
            Contract.Requires(null != showBar);
            Contract.Requires(null != hideBar);
            Contract.Ensures(null != Contract.Result<IApplicationBarSite>());
            return new ApplicationBarSite(barViewModel, showBar, hideBar);
        }

        private ApplicationBarSite(IApplicationBarViewModel barViewModel, ICommand showBar, Action hideBar)
        {
            Contract.Requires(null != barViewModel);
            Contract.Requires(null != showBar);
            Contract.Requires(null != hideBar);
            Contract.Ensures(null != _barViewModel);
            Contract.Ensures(null != _showBarProxiedCommand);
            Contract.Ensures(null != _hideBarAction);
            Contract.Ensures(null != _showBarCommand);
            Contract.Ensures(null != _hideBarCommand);

            _barViewModel = barViewModel;
            _barViewModel.PropertyChanged += this.OnBarPropertyChanged;
            _showBarProxiedCommand = showBar;
            _hideBarAction = hideBar;
            _showBarCommand = new RelayCommand(o => ShowBar(), o => null != _barViewModel && _showBarProxiedCommand.CanExecute(null));
            _hideBarCommand = new RelayCommand(o => HideBar(), o => null != _barViewModel);
        }

        static ApplicationBarSite()
        {
            Contract.Ensures(null != _forwardedPropertiesNames);
            _forwardedPropertiesNames = new[] { "IsBarVisible", "IsBarSticky" };
        }

        bool IApplicationBarSite.IsBarSticky
        {
            get { return null != _barViewModel && _barViewModel.IsBarSticky; }
            set { if (null != _barViewModel) _barViewModel.IsBarSticky = value; }
        }

        bool IApplicationBarSite.IsBarVisible { get { return null != _barViewModel && _barViewModel.IsBarVisible; } }

        ICommand IApplicationBarSite.ShowBar
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                return _showBarCommand;
            }
        }

        ICommand IApplicationBarSite.HideBar
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                return _hideBarCommand;
            }
        }

        void IApplicationBarSiteControl.Deactivate()
        {
            Contract.Ensures(null == _barViewModel);
            Contract.Ensures(null == _showBarProxiedCommand);
            Contract.Ensures(null == _hideBarAction);

            if (null != _barViewModel)
            {
                //
                // Let the consumers of the bar control command objects know that
                // the commands cannot be executed anymore.
                //
                _showBarCommand.EmitCanExecuteChanged();
                _hideBarCommand.EmitCanExecuteChanged();
                //
                // Release all resources.
                //
                _barViewModel.PropertyChanged -= this.OnBarPropertyChanged;
                _barViewModel = null;
                _showBarProxiedCommand = null;
                _hideBarAction = null;
                //
                // Notify that all forwarded properties have changed.
                //
                foreach(string propertyName in _forwardedPropertiesNames)
                    EmitPropertyChanged(propertyName);
            }
        }

        private void OnBarPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Contract.Assert(null != e);
            Contract.Requires(null != _forwardedPropertiesNames);
            //
            // Check if the property change must be forwarded by looking in the
            // array of forwarded property names. The lookup is a linear search,
            // but the array alny has 2 names in it, so it is more effecient
            // than any other search algorithm.
            //
            if (-1 != Array.IndexOf<string>(_forwardedPropertiesNames, e.PropertyName))
                EmitPropertyChanged(e);
        }

        private void ShowBar()
        {
            if (null != _showBarProxiedCommand && _showBarProxiedCommand.CanExecute(null))
                _showBarProxiedCommand.Execute(null);
        }

        private void HideBar()
        {
            if (null != _hideBarAction)
                _hideBarAction();
        }
    }
}

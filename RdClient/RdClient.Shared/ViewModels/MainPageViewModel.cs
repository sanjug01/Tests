namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public sealed class MainPageViewModel : ViewModelBase, IApplicationBarViewModel
    {
        private IEnumerable<BarItemModel> _barItems;
        private bool _isShowBarButtonVisible;
        private bool _isBarVisible;
        private bool _isBarSticky;
        private readonly RelayCommand _showBar;

        public MainPageViewModel()
        {
            _isShowBarButtonVisible = false;
            _isBarVisible = false;
            _isBarSticky = false;
            _showBar = new RelayCommand(o => this.ShowApplicationBar(), o => _isShowBarButtonVisible);
        }

        public IEnumerable<BarItemModel> BarItems
        {
            get { return _barItems; }
            set { this.SetProperty<IEnumerable<BarItemModel>>(ref _barItems, value); }
        }

        public bool IsShowBarButtonVisible
        {
            get { return _isShowBarButtonVisible; }
            set
            {
                this.SetProperty<bool>(ref _isShowBarButtonVisible, value);
            }
        }

        public bool IsBarVisible
        {
            get { return _isBarVisible; }
            set
            {
                if(this.SetProperty<bool>(ref _isBarVisible, value))
                {
                    //
                    // If the bar got shown, hide the button that shows it
                    //
                    if (value)
                        this.IsShowBarButtonVisible = false;
                }
            }
        }

        public bool IsBarSticky
        {
            get { return _isBarSticky; }
            set { this.SetProperty<bool>(ref _isBarSticky, value); }
        }

        public ICommand ShowBar { get { return _showBar; } }

        private void ShowApplicationBar()
        {
            if (_isShowBarButtonVisible)
            {
                this.IsShowBarButtonVisible = false;
                this.IsBarVisible = true;
            }
        }

        protected override void OnPresenting(object activationParameter)
        {
            //throw new NotImplementedException();
        }
    }
}

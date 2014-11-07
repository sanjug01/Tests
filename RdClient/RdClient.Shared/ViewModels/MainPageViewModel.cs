namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Windows.Input;

    public sealed class MainPageViewModel : ViewModelBase, IApplicationBarViewModel
    {
        private bool _isShowBarButtonVisible;
        private bool _isBarVisible;
        private bool _isBarSticky;

        public MainPageViewModel()
        {
            _isShowBarButtonVisible = false;
            _isBarVisible = false;
            _isBarSticky = false;
        }

        public bool IsShowBarButtonVisible
        {
            get { return _isShowBarButtonVisible; }
            set { this.SetProperty<bool>(ref _isShowBarButtonVisible, value); }
        }

        public bool IsBarVisible
        {
            get { return _isBarVisible; }
            set { this.SetProperty<bool>(ref _isBarVisible, value); }
        }

        public bool IsBarSticky
        {
            get { return _isBarSticky; }
            set { this.SetProperty<bool>(ref _isBarSticky, value); }
        }
    }
}

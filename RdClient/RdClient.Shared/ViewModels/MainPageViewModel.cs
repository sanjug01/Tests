namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    public sealed class MainPageViewModel : Helpers.MutableObject, IApplicationBarViewModel
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
            set
            {
                if(this.SetProperty<IEnumerable<BarItemModel>>(ref _barItems, value))
                {
                    this.IsBarVisible = false;

                    if (!VisibleBarItemsPresent(_barItems))
                    {
                        //
                        // If there are no bar items or all items are hidden, hide the bar UI
                        //
                        this.IsShowBarButtonVisible = false;
                    }
                }
            }
        }

        public bool IsShowBarButtonVisible
        {
            get { return _isShowBarButtonVisible; }
            set
            {
                if (this.SetProperty<bool>(ref _isShowBarButtonVisible, value))
                    _showBar.EmitCanExecuteChanged();
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

        private static bool VisibleBarItemsPresent(IEnumerable<BarItemModel> items)
        {
            bool itemsPresent = false;

            if (null != items)
            {
                foreach (BarItemModel model in items)
                {
                    if (model.IsVisible)
                    {
                        itemsPresent = true;
                        break;
                    }
                }
            }

            return itemsPresent;
        }
    }
}

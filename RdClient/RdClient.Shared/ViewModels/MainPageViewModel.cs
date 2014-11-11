namespace RdClient.Shared.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Windows.Input;

    public sealed class MainPageViewModel : Helpers.MutableObject, IApplicationBarViewModel
    {
        private IEnumerable<BarItemModel> _barItems;
        private int _visibleItemsCount;
        private bool _isShowBarButtonVisible;
        private bool _isBarVisible;
        private bool _isBarSticky;
        private readonly RelayCommand _showBar;

        public MainPageViewModel()
        {
            _visibleItemsCount = 0;
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
                if(!object.ReferenceEquals(_barItems, value))
                {
                    //
                    // Unsubscribe from all item model property changed notifications
                    //
                    if(null != _barItems)
                    {
                        foreach (BarItemModel model in _barItems)
                            model.PropertyChanged -= this.OnItemModelPropertyChanged;
                    }

                    _visibleItemsCount = 0;
                    this.SetProperty<IEnumerable<BarItemModel>>(ref _barItems, value);

                    if (null != value)
                    {
                        foreach (BarItemModel model in value)
                        {
                            model.PropertyChanged += this.OnItemModelPropertyChanged;
                            if (model.IsVisible)
                                ++_visibleItemsCount;
                        }
                    }
                    this.IsBarVisible = false;
                    this.IsShowBarButtonVisible = 0 != _visibleItemsCount;
                    _showBar.EmitCanExecuteChanged();
                }
            }
        }

        public bool IsShowBarButtonVisible
        {
            get { return _isShowBarButtonVisible; }
            private set
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
                    else
                        this.IsShowBarButtonVisible = 0 != _visibleItemsCount;
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

        private void OnItemModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsVisible"))
            {
                if (((BarItemModel)sender).IsVisible)
                {
                    if (0 == _visibleItemsCount++)
                    {
                        this.IsBarVisible = false;
                        this.IsShowBarButtonVisible = true;
                        _showBar.EmitCanExecuteChanged();
                    }
                }
                else
                {
                    if (0 == --_visibleItemsCount)
                    {
                        this.IsBarVisible = false;
                        this.IsShowBarButtonVisible = false;
                        _showBar.EmitCanExecuteChanged();
                    }
                }
            }
        }
    }
}

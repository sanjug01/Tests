namespace RdClient.Shared.Navigation.Extensions
{
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;
    
    public sealed class ApplicationBarExtension : MutableObject, INavigationExtension
    {
        private IApplicationBarViewModel _barViewModel;
        private IApplicationBarSiteControl _lastCreatedSite;

        public IApplicationBarViewModel ViewModel
        {
            get { return _barViewModel; }
            set { this.SetProperty<IApplicationBarViewModel>(ref _barViewModel, value); }
        }

        void INavigationExtension.Presenting(IViewModel viewModel)
        {
            _barViewModel.IsBarSticky = false;
            _barViewModel.BarItems = QueryApplicationBarItems(viewModel);
        }

        private IEnumerable<BarItemModel> QueryApplicationBarItems( IViewModel viewModel )
        {
            IEnumerable<BarItemModel> barItems = null;

            viewModel.CastAndCall<IApplicationBarItemsSource>(itemsSource =>
            {
                if (null != _lastCreatedSite)
                    _lastCreatedSite.Deactivate();

                IApplicationBarSite site = ApplicationBarSite.Create(_barViewModel,
                    _barViewModel.ShowBar, () => _barViewModel.IsBarVisible = false);
                barItems = itemsSource.GetItems(site);

                _lastCreatedSite = site as IApplicationBarSiteControl;
            });

            return barItems;
        }
    }
}

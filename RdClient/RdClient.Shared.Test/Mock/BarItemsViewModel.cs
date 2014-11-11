namespace RdClient.Shared.Test.Mock
{
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    sealed class BarItemsViewModel : ViewModel, IApplicationBarItemsSource
    {
        public IApplicationBarSite ApplicationBarSite;
        public IEnumerable<BarItemModel> Models;

        public IEnumerable<BarItemModel> GetItems(IApplicationBarSite applicationBarSite)
        {
            this.ApplicationBarSite = applicationBarSite;
            return this.Models;
        }
    }
}

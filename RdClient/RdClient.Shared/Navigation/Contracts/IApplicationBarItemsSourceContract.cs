namespace RdClient.Shared.Navigation.Contracts
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(IApplicationBarItemsSource))]
    abstract class IApplicationBarItemsSourceContract : IApplicationBarItemsSource
    {
        public IEnumerable<ViewModels.BarItemModel> GetItems(IApplicationBarSite applicationBarSite)
        {
            Contract.Requires(null != applicationBarSite);
            throw new System.NotImplementedException();
        }
    }
}

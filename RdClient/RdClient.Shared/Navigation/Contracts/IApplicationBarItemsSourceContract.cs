namespace RdClient.Shared.Navigation.Contracts
{
    using RdClient.Shared.Navigation.Extensions;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;

    [DebuggerNonUserCode] // exclude from code coverage
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

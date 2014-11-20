namespace RdClient.Shared.Navigation.Contracts
{
    using System;
    using System.Diagnostics.Contracts;

    [ContractClassFor(typeof(INavigationExtension))]
    abstract class INavigationExtensionContract : INavigationExtension
    {
        public void Presenting(IViewModel viewModel)
        {
            Contract.Requires(null != viewModel);
            throw new NotImplementedException();
        }

        public void Dismissed(IViewModel viewModel)
        {
            Contract.Requires(null != viewModel);
            throw new NotImplementedException();
        }
    }
}

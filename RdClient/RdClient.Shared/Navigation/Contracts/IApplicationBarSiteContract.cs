namespace RdClient.Shared.Navigation.Contracts
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [ContractClassFor(typeof(IApplicationBarSite))]
    abstract class IApplicationBarSiteContract : IApplicationBarSite
    {
        public bool IsBarSticky
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public bool IsBarVisible
        {
            get { throw new NotImplementedException(); }
        }

        public ICommand ShowBar
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                throw new NotImplementedException();
            }
        }

        public ICommand HideBar
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                throw new NotImplementedException();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

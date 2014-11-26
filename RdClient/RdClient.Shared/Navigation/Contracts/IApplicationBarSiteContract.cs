namespace RdClient.Shared.Navigation.Contracts
{
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    [DebuggerNonUserCode] // exclude from code coverage
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

        public event PropertyChangedEventHandler PropertyChanged
        {
            add
            {
                throw new NotImplementedException();
            }

            remove
            {
                throw new NotImplementedException();
            }
        }
    }
}

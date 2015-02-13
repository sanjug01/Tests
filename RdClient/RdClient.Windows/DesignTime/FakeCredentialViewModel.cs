using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public sealed class FakeCredentialViewModel : ICredentialViewModel
    {
        private CredentialsModel _cred = new CredentialsModel() { Password = "1234AbCd", Username = "sampleUser" };

        public CredentialsModel Credentials
        {
            get { return _cred; }
        }

        public ICommand DeleteCommand {get; set; }

        public ICommand EditCommand {get; set; }

        public void Presented(INavigationService navService, ApplicationDataModel dataModel)
        {
            throw new NotImplementedException();
        }

        void ICredentialViewModel.Dismissed()
        {
        }
    }
}

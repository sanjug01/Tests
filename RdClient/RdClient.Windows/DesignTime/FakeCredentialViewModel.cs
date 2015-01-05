using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeCredentialViewModel : ICredentialViewModel
    {
        private Credentials _cred = new Credentials() { Domain = "contoso.com", HaveBeenPersisted = true, Password = "1234AbCd", Username = "sampleUser" };

        public Credentials Credential
        {
            get { return _cred; }
        }

        public ICommand DeleteCommand {get; set; }

        public ICommand EditCommand {get; set; }

        public void Presented(INavigationService navService, RdDataModel dataModel)
        {
            throw new NotImplementedException();
        }
    }
}

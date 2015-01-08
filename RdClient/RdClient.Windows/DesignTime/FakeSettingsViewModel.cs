using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeSettingsViewModel : ISettingsViewModel
    {
        private readonly ObservableCollection<ICredentialViewModel> _credVMs;

        public FakeSettingsViewModel()
        {
            this.ShowGeneralSettings = true;
            this.GeneralSettings = new GeneralSettings();
            this.GeneralSettings.UseThumbnails = true;
            _credVMs = new ObservableCollection<ICredentialViewModel>();
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
            _credVMs.Add(new FakeCredentialViewModel());
        }

        public ICommand GoBackCommand {get; set;}

        public bool ShowGatewaySettings {get; set;}

        public bool ShowGeneralSettings {get; set;}

        public bool ShowUserSettings {get; set;}

        public Shared.Models.GeneralSettings GeneralSettings {get; set;}


        public ICommand AddUserCommand
        {
            get { return null; }
        }

        public bool HasCredentials
        {
            get { return this.CredentialsViewModels.Count > 0; }
        }

        public ObservableCollection<ICredentialViewModel> CredentialsViewModels
        {
            get { return _credVMs; }
        }
    }
}

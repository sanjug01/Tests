using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeSettingsViewModel : ISettingsViewModel
    {
        private readonly ObservableCollection<ICredentialViewModel> _source;
        private readonly ReadOnlyObservableCollection<ICredentialViewModel> _credVMs;

        public FakeSettingsViewModel()
        {
            this.ShowGeneralSettings = true;
            this.GeneralSettings = new GeneralSettings();
            this.GeneralSettings.UseThumbnails = true;

            _source = new ObservableCollection<ICredentialViewModel>()
            {
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
                new FakeCredentialViewModel(),
            };

            _credVMs = new ReadOnlyObservableCollection<ICredentialViewModel>(_source);
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

        public ReadOnlyObservableCollection<ICredentialViewModel> CredentialsViewModels
        {
            get { return _credVMs; }
        }
    }
}

using RdClient.Shared.Navigation;
using RdClient.Shared.Models;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class DesktopViewModel : ViewModelBase
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private Desktop _desktop;

        public DesktopViewModel(Desktop desktop)
        {
            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            this.Desktop = desktop;
        }

        public Desktop Desktop
        {
            get { return _desktop; }
            private set 
            { 
                SetProperty(ref _desktop, value);
            }
        }

        public Credentials Credential
        {
            get 
            {
                Credentials cred;
                if (this.Desktop.HasCredential)
                {
                    cred = this.DataModel.Credentials.GetItemWithId(this.Desktop.CredentialId);
                }
                else
                {
                    cred = new Credentials() { Username = "tslabadmin", Domain = "", Password = "1234AbCd", HaveBeenPersisted = false };
                }
                return cred;
            }
        }

        public ICommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand ConnectCommand
        {
            get { return _connectCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        protected override void OnPresenting(object activationParameter)
        {

        }

        private void EditCommandExecute(object o)
        {
            NavigationService.PushModalView("AddOrEditDesktopView", new AddOrEditDesktopViewModelArgs(this.Desktop, null, false));
        }

        private void ConnectCommandExecute(object o)
        {
            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = this.Desktop,
                Credentials = this.Credential
            };

            NavigationService.NavigateToView("SessionView", connectionInformation);
        }

        private void DeleteCommandExecute(object o)
        {
            this.DataModel.Desktops.Remove(this.Desktop);
        }
    }
}

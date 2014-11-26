using RdClient.Shared.Navigation;
using RdClient.Shared.Models;

using System.Collections.Generic;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class DesktopViewModel : ViewModelBase, IDesktopViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private Desktop _desktop;
        private bool _isSelected;

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
                    cred = null;
                }
                return cred;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                SetProperty(ref _isSelected, value);
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
            NavigationService.PushModalView("AddOrEditDesktopView", new EditDesktopViewModelArgs(this.Desktop));
        }

        private void ConnectCommandExecute(object o)
        {            
            if(this.Credential != null)
            {
                InternalConnect(this.Credential, false);
            }
            else
            {
                AddUserViewArgs args = new AddUserViewArgs(this.Desktop, InternalConnect, true);
                NavigationService.PushModalView("AddUserView", args);
            }
        }

        private void InternalConnect(Credentials credentials, bool storeCredentials)
        {
            if(storeCredentials)
            {
                this.Desktop.CredentialId = credentials.Id;
                this.DataModel.Credentials.Add(credentials);
            }

            ConnectionInformation connectionInformation = new ConnectionInformation()
            {
                Desktop = this.Desktop,
                Credentials = credentials
            };

            NavigationService.NavigateToView("SessionView", connectionInformation);            
        }

        private void DeleteCommandExecute(object o)
        {            
            NavigationService.PushModalView("DeleteDesktopsView", new DeleteDesktopsArgs(this.Desktop));            
        }
    }
}

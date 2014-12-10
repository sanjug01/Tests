using RdClient.Shared.Navigation;
using RdClient.Shared.Models;

using System.Collections.Generic;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class DesktopViewModel : Helpers.MutableObject, IDesktopViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _connectCommand;
        private readonly RelayCommand _deleteCommand;
        private Desktop _desktop;
        private bool _isSelected;
        private RdDataModel _dataModel;

        public DesktopViewModel(Desktop desktop, INavigationService navService, RdDataModel dataModel)
        {
            _editCommand = new RelayCommand(EditCommandExecute);
            _connectCommand = new RelayCommand(ConnectCommandExecute);
            _deleteCommand = new RelayCommand(DeleteCommandExecute);
            this.Desktop = desktop;

            /// DesktopVieModel does not require Presenting/Dismissing, 
            //          but stil needs DataModel and NavigationService
            //          NavigationService may be initialized later while presenting the parent view
            _dataModel = dataModel;
            this.NavigationService = navService;
        }

        public INavigationService NavigationService { private get; set; }

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
                    cred = _dataModel.Credentials.GetItemWithId(this.Desktop.CredentialId);
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
                AddUserViewArgs args = new AddUserViewArgs(InternalConnect, true);
                NavigationService.PushModalView("AddUserView", args);
            }
        }

        private void InternalConnect(Credentials credentials, bool storeCredentials)
        {
            if(storeCredentials)
            {
                this.Desktop.CredentialId = credentials.Id;
                this._dataModel.Credentials.Add(credentials);
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

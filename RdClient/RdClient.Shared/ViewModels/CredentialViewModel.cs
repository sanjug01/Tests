using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class CredentialViewModel: MutableObject, ICredentialViewModel
    {
        private Credentials _cred;
        private INavigationService _nav;
        private RdDataModel _dataModel;
        private RelayCommand _editCommand;
        private RelayCommand _deleteCommand;

        public CredentialViewModel(Credentials cred)
        {
            this.Credential = cred;
            _editCommand = new RelayCommand((o) => this.EditCommandExecute());
            _deleteCommand = new RelayCommand((o) => this.DeleteCommandExecute());
        }

        public Credentials Credential
        {
            get { return _cred; }
            private set { SetProperty(ref _cred, value); }
        }

        public ICommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public void Presented(INavigationService navService, RdDataModel dataModel)
        {
            _nav = navService;
            _dataModel = dataModel;
        }

        private void EditCommandExecute()
        {
            AddUserViewArgs addUserArgs = new AddUserViewArgs(this.Credential, false);
            _nav.PushModalView("AddUserView", addUserArgs);        
        }

        private void DeleteCommandExecute()
        {
            _nav.PushModalView("DeleteUserView", this.Credential);
        }
    }
}

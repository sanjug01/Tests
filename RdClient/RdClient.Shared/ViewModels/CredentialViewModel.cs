using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class CredentialViewModel: MutableObject, ICredentialViewModel
    {
        private IModelContainer<CredentialsModel> _credentials;
        private INavigationService _nav;
        private ApplicationDataModel _dataModel;
        private RelayCommand _editCommand;
        private RelayCommand _deleteCommand;

        public CredentialViewModel(IModelContainer<CredentialsModel> credentials)
        {
            _credentials = credentials;
            _editCommand = new RelayCommand((o) => this.EditCommandExecute());
            _deleteCommand = new RelayCommand((o) => this.DeleteCommandExecute());
        }

        public CredentialsModel Credentials
        {
            get { return _credentials.Model; }
        }

        public ICommand EditCommand
        {
            get { return _editCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public void Presented(INavigationService navService, ApplicationDataModel dataModel)
        {
            _nav = navService;
            _dataModel = dataModel;
        }

        private void EditCommandExecute()
        {
            AddUserViewArgs addUserArgs = new AddUserViewArgs(this.Credentials, false);

            _nav.PushModalView("AddUserView", addUserArgs);        
        }

        private void DeleteCommandExecute()
        {
            _nav.PushModalView("DeleteUserView", _credentials);
        }
    }
}

using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public sealed class CredentialViewModel: MutableObject, ICredentialViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly IModelContainer<CredentialsModel> _credentials;
        private INavigationService _nav;
        private ApplicationDataModel _dataModel;

        public CredentialViewModel(IModelContainer<CredentialsModel> credentials)
        {
            _editCommand = new RelayCommand((o) => this.EditCommandExecute());
            _deleteCommand = new RelayCommand((o) => this.DeleteCommandExecute());
            _credentials = credentials;
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

        public void Dismissed()
        {
            _nav = null;
            _dataModel = null;
        }

        private void EditCommandExecute()
        {
            AddUserViewArgs addUserArgs = new AddUserViewArgs(this.Credentials, false, CredentialPromptMode.EditCredentials);

            _nav.PushModalView("AddUserView", addUserArgs);        
        }

        private void DeleteCommandExecute()
        {
            _nav.PushModalView("DeleteUserView", _credentials);
        }
    }
}

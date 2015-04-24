namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class DeleteUserViewModel : ViewModelBase, IDialogViewModel
    {
        private IModelContainer<CredentialsModel> _cred;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;

        public DeleteUserViewModel()
        {
            _deleteCommand = new RelayCommand(o => this.DeleteCommandExecute());
            _cancelCommand = new RelayCommand(o => this.CancelCommandExecute());        
        }

        public CredentialsModel Credentials
        {
            get { return _cred.Model; }
        }

        public ICommand DefaultAction
        {
            get { return _deleteCommand; }
        }

        public ICommand Cancel
        {
            get { return _cancelCommand; }
        }
        

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Assert(activationParameter is IModelContainer<CredentialsModel>);
            _cred = (IModelContainer<CredentialsModel>)activationParameter;
        }

        private void DeleteCommandExecute()
        {
            Contract.Assert(null != this.ApplicationDataModel);
            Contract.Assert(null != this.Credentials);
            //
            // Remove the credentials from the data model.
            // The data model will remove references to the removed credentials from all desktops, gateways and workspaces
            //
            this.ApplicationDataModel.Credentials.RemoveModel(_cred.Id);

            this.DismissModal(null);
        }

        private void CancelCommandExecute()
        {
            this.DismissModal(null);
        }
    }
}

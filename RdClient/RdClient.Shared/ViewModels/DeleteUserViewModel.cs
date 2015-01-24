using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public class DeleteUserViewModel : ViewModelBase
    {
        private Credentials _cred;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;

        public DeleteUserViewModel()
        {
            _deleteCommand = new RelayCommand(o => this.DeleteCommandExecute());
            _cancelCommand = new RelayCommand(o => this.CancelCommandExecute());        
        }

        public IPresentableView PresentableView { private get; set; }

        public Credentials Credential
        {
            get { return _cred; }
            private set{ SetProperty(ref _cred, value); }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public ICommand CancelCommand
        {
            get { return _cancelCommand; }
        }
        

        protected override void OnPresenting(object activationParameter)
        {
            Contract.Requires(null != activationParameter as Credentials);
            Contract.Assert(activationParameter is Credentials);
            this.Credential = activationParameter as Credentials;
        }

        private void DeleteCommandExecute()
        {
            Contract.Requires(null != this.DataModel);
            Contract.Requires(null != this.Credential);
            //Remove all references to this credential first
            List<Desktop> desktops = this.DataModel.LocalWorkspace.Connections.OfType<Desktop>().Where(d => d.CredentialId.Equals(this.Credential.Id)).ToList();
            foreach (Desktop desktop in desktops)
            {
                desktop.CredentialId = Guid.Empty;
            }
            //remove this credential
            this.DataModel.LocalWorkspace.Credentials.Remove(this.Credential);
            
            NavigationService.DismissModalView(this.PresentableView);
        }

        private void CancelCommandExecute()
        {
            Contract.Requires(null != this.NavigationService);
            NavigationService.DismissModalView(this.PresentableView);
        }
    }
}

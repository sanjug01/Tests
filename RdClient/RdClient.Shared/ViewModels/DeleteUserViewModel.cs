using RdClient.Shared.Data;
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
        private IModelContainer<CredentialsModel> _cred;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;

        public DeleteUserViewModel()
        {
            _deleteCommand = new RelayCommand(o => this.DeleteCommandExecute());
            _cancelCommand = new RelayCommand(o => this.CancelCommandExecute());        
        }

        public IPresentableView PresentableView { private get; set; }

        public IModelContainer<CredentialsModel> Credentials
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
            Contract.Assert(activationParameter is IModelContainer<CredentialsModel>);
            this.Credentials = (IModelContainer<CredentialsModel>)activationParameter;
        }

        private void DeleteCommandExecute()
        {
            Contract.Assert(null != this.ApplicationDataModel);
            Contract.Assert(null != this.Credentials);
            //
            // Remove all references to this credential first
            //
            foreach (IModelContainer<RemoteConnectionModel> container in this.ApplicationDataModel.LocalWorkspace.Connections.Models)
            {
                if(container.Model is DesktopModel)
                {
                    IModelContainer<DesktopModel> d = new TemporaryModelContainer<DesktopModel>(container.Id, (DesktopModel)container.Model);

                    if (this.Credentials.Id.Equals(d.Model.CredentialsId))
                        d.Model.CredentialsId = Guid.Empty;
                }
            }
            //remove this credential
            this.ApplicationDataModel.LocalWorkspace.Credentials.RemoveModel(this.Credentials.Id);
            
            NavigationService.DismissModalView(this.PresentableView);
        }

        private void CancelCommandExecute()
        {
            Contract.Requires(null != this.NavigationService);
            NavigationService.DismissModalView(this.PresentableView);
        }
    }
}

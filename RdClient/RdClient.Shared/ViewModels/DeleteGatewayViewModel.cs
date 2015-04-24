namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    public class DeleteGatewayViewModel : ViewModelBase, IDialogViewModel
    {
        private IModelContainer<GatewayModel> _gatewayContainer;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _cancelCommand;

        public DeleteGatewayViewModel()
        {
            _deleteCommand = new RelayCommand(o => this.DeleteCommandExecute());
            _cancelCommand = new RelayCommand(o => this.CancelCommandExecute());        
        }

        public GatewayModel Gateway
        {
            get { return _gatewayContainer.Model; }
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
            Contract.Assert(activationParameter is IModelContainer<GatewayModel>);
            _gatewayContainer = (IModelContainer<GatewayModel>)activationParameter;
        }

        private void DeleteCommandExecute()
        {
            Contract.Assert(null != this.ApplicationDataModel);
            Contract.Assert(null != this.Gateway);
            
            // Remove the gateway from the data model.
            // The data model will remove references to the removed gateway from all desktops.
            this.ApplicationDataModel.Gateways.RemoveModel(_gatewayContainer.Id);
            this.DismissModal(null);
        }

        private void CancelCommandExecute()
        {
            this.DismissModal(null);
        }
    }
}

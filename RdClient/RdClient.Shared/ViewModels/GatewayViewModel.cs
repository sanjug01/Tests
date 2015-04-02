using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public sealed class GatewayViewModel: MutableObject, IGatewayViewModel
    {
        private readonly RelayCommand _editCommand;
        private readonly RelayCommand _deleteCommand;
        private readonly IModelContainer<GatewayModel> _gatewayContainer;
        private INavigationService _nav;
        private ApplicationDataModel _dataModel;

        public GatewayViewModel(IModelContainer<GatewayModel> gatewayContainer)
        {
            _editCommand = new RelayCommand((o) => this.EditCommandExecute());
            _deleteCommand = new RelayCommand((o) => this.DeleteCommandExecute());
            _gatewayContainer = gatewayContainer;
        }

        public GatewayModel Gateway
        {
            get { return _gatewayContainer.Model; }
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
            EditGatewayViewModelArgs editGatewayArgs = new EditGatewayViewModelArgs(this.Gateway);

            _nav.PushModalView("AddOrEditGatewayView", editGatewayArgs);        
        }

        private void DeleteCommandExecute()
        {
            _nav.PushModalView("DeleteGatewayView", _gatewayContainer);
        }
    }
}

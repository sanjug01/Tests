using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface IGatewayViewModel
    {
        GatewayModel Gateway { get; }
        ICommand DeleteCommand { get; }
        ICommand EditCommand { get; }
        void Presented(INavigationService navService, ApplicationDataModel dataModel);
        void Dismissed();
    }
}

namespace RdClient.Shared.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    // interface for managing a ReadOnlyObservableCollection of GatewayComboBoxElement instances
    public interface IGatewaysCollector
    {
        ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways { get; }
        GatewayComboBoxElement SelectedGateway { get; set; }
        ICommand EditGateway { get; }
        ICommand AddGateway { get; }
    }
}

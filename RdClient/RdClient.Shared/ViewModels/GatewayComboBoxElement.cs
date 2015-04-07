namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;

    public enum GatewayComboBoxType
    {
        Gateway,
        None,
        AddNew
    }

    public class GatewayComboBoxElement
    {
        private readonly IModelContainer<GatewayModel> _gateway;

        public IModelContainer<GatewayModel> Gateway { get { return _gateway; } }

        private readonly GatewayComboBoxType _gatewayComboBoxType;
        public GatewayComboBoxType GatewayComboBoxType { get { return _gatewayComboBoxType; } }

        public GatewayComboBoxElement(GatewayComboBoxType gatewayComboBoxType, IModelContainer<GatewayModel> gateway = null)
        {            
            _gatewayComboBoxType  = gatewayComboBoxType;
            _gateway = gateway;
        }
    }
}

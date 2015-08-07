using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using RdClient.Shared.Data;

namespace RdClient.DesignTime
{
    public class FakeUsersAndGatewaysCollectorBase : FakeUsersCollectorBase, IGatewaysCollector
    {
        protected ObservableCollection<GatewayComboBoxElement> _gateways;
        protected GatewayComboBoxElement _selectedGateway;

        public FakeUsersAndGatewaysCollectorBase() 
        {
            // load test data for the gateways collection
            _gateways = new ObservableCollection<GatewayComboBoxElement>();
            for (int i = 0; i < 5; i++)
            {
                var gateway = new GatewayModel() { HostName = "gateway" + i };
                var gatewayModel = TemporaryModelContainer<GatewayModel>.WrapModel(Guid.NewGuid(), gateway);
                _gateways.Add(new GatewayComboBoxElement(GatewayComboBoxType.Gateway, gatewayModel));
            }
            _selectedGateway = _gateways[1];
        }

        public ICommand AddGateway
        {
            get { return new RelayCommand(o => { }, o=> true); }
        }
        
        public ICommand EditGateway
        {
            get { return new RelayCommand(o => { }, o => false); }
        }

        public ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways
        {
            get { return new ReadOnlyObservableCollection<GatewayComboBoxElement>(_gateways); }            
        }
        
        public GatewayComboBoxElement SelectedGateway
        {
            get { return _selectedGateway; }
            set { _selectedGateway = value; }
        }        
    }
}

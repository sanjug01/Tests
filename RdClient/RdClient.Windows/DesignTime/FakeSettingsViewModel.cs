using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using RdClient.Shared.Data;

namespace RdClient.DesignTime
{
    public class FakeSettingsViewModel : ISettingsViewModel
    {
        private ObservableCollection<GatewayComboBoxElement> _gateways;
        private GeneralSettings _general;
        private GatewayComboBoxElement _selectedGateway;
        private UserComboBoxElement _selectedUser;
        private ObservableCollection<UserComboBoxElement> _users;

        public FakeSettingsViewModel()
        {
            _general = new GeneralSettings();
            _general.UseThumbnails = true;

            _gateways = new ObservableCollection<GatewayComboBoxElement>();
            _gateways.Add(new GatewayComboBoxElement(GatewayComboBoxType.AddNew));
            for (int i = 0; i < 5; i++)
            {
                var gateway = new GatewayModel() { HostName = "gateway" + i };
                var gatewayModel = TemporaryModelContainer<GatewayModel>.WrapModel(Guid.NewGuid(), gateway);
                _gateways.Add(new GatewayComboBoxElement(GatewayComboBoxType.Gateway, gatewayModel));
            }
            _selectedGateway = _gateways[1];

            _users = new ObservableCollection<UserComboBoxElement>();
            _users.Add(new UserComboBoxElement(UserComboBoxType.AddNew));
            for (int i = 0; i < 10; i++)
            {
                var user = new CredentialsModel() { Username = "user" + i, Password = "12345" };
                var userModel = TemporaryModelContainer<CredentialsModel>.WrapModel(Guid.NewGuid(), user);
                _users.Add(new UserComboBoxElement(UserComboBoxType.Credentials, userModel));
            }
            _selectedUser = _users[4];
        }

        public ICommand AddGatewayCommand
        {
            get { return new RelayCommand(o => { }, o=> true); }
        }


        public ICommand AddUserCommand
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ICommand Cancel
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ICommand DeleteGatewayCommand
        {
            get { return new RelayCommand(o => { }, o => false); }
        }

        public ICommand DeleteUserCommand
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ICommand EditGatewayCommand
        {
            get { return new RelayCommand(o => { }, o => false); }
        }

        public ICommand EditUserCommand
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ReadOnlyObservableCollection<GatewayComboBoxElement> Gateways
        {
            get { return new ReadOnlyObservableCollection<GatewayComboBoxElement>(_gateways); }            
        }

        public GeneralSettings GeneralSettings
        {
            get { return _general; }
        }

        public GatewayComboBoxElement SelectedGateway
        {
            get { return _selectedGateway; }
            set { _selectedGateway = value; }
        }

        public UserComboBoxElement SelectedUser
        {
            get{ return _selectedUser; }
            set { _selectedUser = value; }
        }

        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return new ReadOnlyObservableCollection<UserComboBoxElement>(_users); }
        }
    }
}

using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.ValidationRules;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public sealed class FakeAddOrEditDesktopViewModel : IAddOrEditDesktopViewModel, IDisposable
    {
        private ObservableCollection<GatewayComboBoxElement> _gateways;
        private ObservableCollection<UserComboBoxElement> _users;
        private GatewayComboBoxElement _selectedGateway;
        private UserComboBoxElement _selectedUser;
        private ValidatedProperty<string> _host;

        public FakeAddOrEditDesktopViewModel()
        {
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

            IsAddingDesktop = true;
            _host = new ValidatedProperty<string>(new HostNameValidationRule());
            Host.Value = "TestHost";
            FriendlyName = "TestFriendlyName";
            IsExpandedView = true;
            IsSwapMouseButtons = true;
            IsUseAdminSession = true;
            AudioMode = 1;

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
        
        public ICommand EditGatewayCommand
        {
            get { return new RelayCommand(o => { }, o => false); }
        }

        public ICommand EditUserCommand
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ObservableCollection<GatewayComboBoxElement> GatewayOptions
        {
            get { return _gateways; }            
        }
        
        public GatewayComboBoxElement SelectedGateway
        {
            get { return _selectedGateway; }
            set { _selectedGateway = value; }
        }


        public ObservableCollection<UserComboBoxElement> UserOptions
        {
            get { return _users; }
        }

        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; }
        }

        public ValidatedProperty<string> Host
        {
            get { return _host; }
        }

        public bool IsAddingDesktop { get; set; }
        public bool IsExpandedView { get; set; }
        public string FriendlyName { get; set; }
        public bool IsUseAdminSession { get; set; }
        public bool IsSwapMouseButtons { get; set; }
        public int AudioMode { get; set; }

        //To remove a build warning
        public void Dispose()
        {
            _host.Dispose();
        }
    }
}

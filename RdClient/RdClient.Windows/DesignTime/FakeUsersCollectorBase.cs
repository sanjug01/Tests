using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using RdClient.Shared.Data;

namespace RdClient.DesignTime
{
    public class FakeUsersCollectorBase : IUsersCollector
    {
        protected ObservableCollection<UserComboBoxElement> _users;
        protected UserComboBoxElement _selectedUser;

        public FakeUsersCollectorBase()
        {
            // load test data for the users collection
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
        
        public ICommand AddUser
        {
            get { return new RelayCommand(o => { }, o => true); }
        }
        
        public ICommand EditUser
        {
            get { return new RelayCommand(o => { }, o => true); }
        }
        
        public ReadOnlyObservableCollection<UserComboBoxElement> Users
        {
            get { return new ReadOnlyObservableCollection<UserComboBoxElement>(_users); }
        }

        public UserComboBoxElement SelectedUser
        {
            get { return _selectedUser; }
            set { _selectedUser = value; }
        }        
    }
}

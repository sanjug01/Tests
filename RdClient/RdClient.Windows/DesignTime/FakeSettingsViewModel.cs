using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System;
using System.Collections.Generic;
using RdClient.Shared.Data;

namespace RdClient.DesignTime
{
    public class FakeSettingsViewModel : FakeUsersAndGatewaysCollectorBase, ISettingsViewModel
    {
        private GeneralSettings _general;

        public FakeSettingsViewModel()
        {
            _general = new GeneralSettings();
            _general.UseThumbnails = true;
        }

        public ICommand Cancel
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public ICommand DeleteGateway
        {
            get { return new RelayCommand(o => { }, o => false); }
        }

        public ICommand DeleteUser
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public GeneralSettings GeneralSettings
        {
            get { return _general; }
        }
    }
}

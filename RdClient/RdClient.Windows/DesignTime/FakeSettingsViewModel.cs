using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeSettingsViewModel : ISettingsViewModel
    {
        public FakeSettingsViewModel()
        {
            this.ShowGeneralSettings = true;
        }

        public ICommand GoBackCommand {get; set;}

        public bool ShowGatewaySettings {get; set;}

        public bool ShowGeneralSettings {get; set;}

        public bool ShowUserSettings {get; set;}
    }
}

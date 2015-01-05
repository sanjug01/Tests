using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System;

namespace RdClient.DesignTime
{
    public class FakeDesktopViewModel : IDesktopViewModel
    {
        private Credentials _cred = new Credentials() { Domain = "adomain", HaveBeenPersisted = false, Password = "1234AbCd", Username = "exampleUser" };
        private Desktop _desktop = new Desktop(new RdDataModel().LocalWorkspace) { HostName = "ExampleHostname" };

        public FakeDesktopViewModel()
        {
            this.IsSelected = true;
            this.SelectionEnabled = true;
        }

        public Desktop Desktop
        {
            get { return _desktop; }
        }

        public Credentials Credential
        {
            get { return _cred; }
        }

        public bool IsSelected { get; set; }

        public System.Windows.Input.ICommand EditCommand
        {
            get { return null; }
        }

        public System.Windows.Input.ICommand ConnectCommand
        {
            get { return null; }
        }

        public System.Windows.Input.ICommand DeleteCommand
        {
            get { return null; }
        }

        public bool SelectionEnabled { get; set; }
    }
}

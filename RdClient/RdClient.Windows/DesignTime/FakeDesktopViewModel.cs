using RdClient.Shared.Models;
using RdClient.Shared.ViewModels;
using System;
using System.Windows.Input;

namespace RdClient.DesignTime
{
    public class FakeDesktopViewModel : IDesktopViewModel
    {
        private CredentialsModel _cred = new CredentialsModel() { Domain = "adomain", Password = "1234AbCd", Username = "exampleUser" };
        private DesktopModel _desktop = new DesktopModel() { HostName = "ExampleHostname" };

        public FakeDesktopViewModel()
        {
            this.IsSelected = true;
            this.SelectionEnabled = true;
        }

        public DesktopModel Desktop
        {
            get { return _desktop; }
        }

        public CredentialsModel Credentials
        {
            get { return _cred; }
        }

        public bool IsSelected { get; set; }

        public ICommand EditCommand
        {
            get { return null; }
        }

        public ICommand ConnectCommand
        {
            get { return null; }
        }

        public ICommand DeleteCommand
        {
            get { return null; }
        }

        public bool SelectionEnabled { get; set; }
    }
}

namespace RdClient.DesignTime
{
    using RdClient.Shared.ValidationRules;
    using RdClient.Shared.ViewModels;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public class FakeAddOrEditDesktopViewModel : FakeUsersAndGatewaysCollectorBase, IAddOrEditDesktopViewModel
    {
        private ValidatedProperty<string> _host;
        public FakeAddOrEditDesktopViewModel()
        {            
            IsAddingDesktop = true;
            _host = new ValidatedProperty<string>(new HostnameValidationRule());
            Host.Value = "TestHost";
            FriendlyName = "TestFriendlyName";
            IsExpandedView = true;
            IsSwapMouseButtons = true;
            IsUseAdminSession = true;
            AudioMode = 1;
        }

        public ICommand Cancel
        {
            get { return new RelayCommand(o => { }, o => true); }
        }

        public IValidatedProperty<string> Host
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

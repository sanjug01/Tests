namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.ValidationRules;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface IAddOrEditDesktopViewModel : IUsersCollector, IGatewaysCollector
    {
        ICommand Cancel { get; }
        
        bool IsAddingDesktop { get; }
        IValidatedProperty<string> Host { get; }
        bool IsExpandedView { get; }
        string FriendlyName { get; }
        bool IsUseAdminSession { get; set; }
        bool IsSwapMouseButtons { get; set; }
        int AudioMode { get; set; }
    }
}

namespace RdClient.Shared.ViewModels
{
    using RdClient.Shared.Models;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface ISettingsViewModel : IUsersCollector, IGatewaysCollector
    {
        ICommand Cancel { get; }
        GeneralSettings GeneralSettings { get; }

        ICommand DeleteUser { get; }
        ICommand DeleteGateway { get; }
    }
}

using RdClient.Navigation;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;
using System.Windows.Input;

namespace RdClient.Shared.ViewModels
{
    public interface ITestsViewModel
    {
        ConnectionInformation ConnectionInformation { set; }
        ICommand GoHomeCommand { get; }
        INavigationService NavigationService { set; }
        IRdpConnectionFactory RdpConnectionFactory { set; }
        ICommand StressTestCommand { get; }
    }
}

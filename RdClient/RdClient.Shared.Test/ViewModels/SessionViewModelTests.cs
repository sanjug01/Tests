using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        [TestMethod]
        public void SessionViewModel_ShouldConnect()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };
                
                sessionModel.Expect("Connect", new List<object>() { connectionInformation }, 0);
                //
                // TODO:    REFACTOR THIS!
                //          Present the view model using the nav service
                //
                ((IViewModel)svm).Presenting(navigation, connectionInformation);
                svm.ConnectCommand.Execute(null);
            }
        }

        [TestMethod]
        public void SessionViewModel_ShouldDisconnect_ShouldNavigateHome()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Disconnect", new List<object>() { }, 0);
                navigation.Expect("NavigateToView", new List<object>() { "ConnectionCenterView", null }, 0);

                //
                // TODO:    REFACTOR THIS!
                //          Present the view model using the nav service
                //
                ((IViewModel)svm).Presenting(navigation, connectionInformation);
                svm.DisconnectCommand.Execute(null);
            }
        }
    }
}

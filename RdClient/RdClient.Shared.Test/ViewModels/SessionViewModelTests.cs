using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        private PersistentData _dataModel;

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new PersistentData();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
        }

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
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };
                
                sessionModel.Expect("Connect", new List<object>() { connectionInformation }, 0);
                //
                // TODO:    REFACTOR THIS!
                //          Present the view model using the nav service
                //
                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);
                svm.ConnectCommand.Execute(null);
            }
        }

        [TestMethod]
        public void SessionViewModel_ShouldDisconnect()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Disconnect", new List<object>() { }, 0);

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);
                svm.DisconnectCommand.Execute(null);
            }
        }

        [TestMethod]
        public void SessionViewModel_ShouldCallHandleDisconnect_ShouldNavigateToHome()
        {
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            {                
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Connect", new List<object> { connectionInformation }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0);
                ClientDisconnectedArgs clientDisconnectedArgs = new ClientDisconnectedArgs(reason);

                eventSource.EmitClientDisconnected(rdpConnection, clientDisconnectedArgs);
            }
        }

        [TestMethod]
        public void SessionViewModel_ShouldCallHandleDisconnect_ShouldDisplayError()
        {
            RdpEventSource eventSource = new RdpEventSource();
            DisconnectString disconnectString = new DisconnectString(new Mock.LocalizedString());

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            {
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Connect", new List<object> { connectionInformation }, null);
                navigation.Expect("PushModalView", new List<object> { "DialogMessage", null, null }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel();
                svm.SessionModel = sessionModel;
                svm.DisconnectString = disconnectString;

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.ServerDeniedConnection, 0, 0);
                ClientDisconnectedArgs clientDisconnectedArgs = new ClientDisconnectedArgs(reason);

                eventSource.EmitClientDisconnected(rdpConnection, clientDisconnectedArgs);
            }
        }
    }
}

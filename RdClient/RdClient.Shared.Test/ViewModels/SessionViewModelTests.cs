using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Input;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        private RdDataModel _dataModel;
        private MouseViewModel _mouseViewModel;

        private sealed class DummyKeyboardCapture : IKeyboardCapture
        {

            event System.EventHandler<KeystrokeEventArgs> IKeyboardCapture.Keystroke
            {
                add { }
                remove { }
            }

            void IKeyboardCapture.Start()
            {
            }

            void IKeyboardCapture.Stop()
            {
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _dataModel = new RdDataModel();
            _mouseViewModel = new MouseViewModel();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
            _mouseViewModel = null;
        }

        [TestMethod]
        public void SessionViewModel_ShouldConnect()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.SessionModel sessionModel = new Mock.SessionModel())
            {
                SessionViewModel svm = new SessionViewModel() { 
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };
                
                sessionModel.Expect("Connect", new List<object>() { connectionInformation, null, null }, 0);
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
                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

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

                sessionModel.Expect("Connect", new List<object> { connectionInformation, null, null }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

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

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            {
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Connect", new List<object> { connectionInformation, null, null }, null);
                navigation.Expect("PushModalView", new List<object> { "ErrorMessageView", null, null }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.ServerDeniedConnection, 0, 0);
                ClientDisconnectedArgs clientDisconnectedArgs = new ClientDisconnectedArgs(reason);

                eventSource.EmitClientDisconnected(rdpConnection, clientDisconnectedArgs);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ConnectionShouldHandleDisconnect()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
            bool connectionShouldReconnect = false;

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            {
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                sessionModel.Expect("Connect", new List<object> { connectionInformation, null, null }, null);
                rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                eventSource.EmitClientAsyncDisconnect(rdpConnection, clientAsyncDisconnectArgs);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ShouldDisplayCertificateValidation()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
            bool isCertificateAccepted = false;

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                rdpConnection.Expect("GetServerCertificate", new List<object> { }, rdpCertificate);
                sessionModel.Expect("Connect", new List<object> { connectionInformation, null, null }, null);
                sessionModel.Expect("IsCertificateAccepted", new List<object> { rdpCertificate }, isCertificateAccepted);
                navigation.Expect("PushModalView", new List<object> { "CertificateValidationView", null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                eventSource.EmitClientAsyncDisconnect(rdpConnection, clientAsyncDisconnectArgs);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ShouldReconnectIfCertificateAccepted()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
            bool isCertificateAccepted = true;
            bool connectionShouldReconnect = true;

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                ConnectionInformation connectionInformation = new ConnectionInformation()
                {
                    Desktop = new Desktop() { HostName = "narf" },
                    Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
                };

                rdpConnection.Expect("GetServerCertificate", new List<object> { }, rdpCertificate);
                sessionModel.Expect("Connect", new List<object> { connectionInformation, null, null }, null);
                sessionModel.Expect("IsCertificateAccepted", new List<object> { rdpCertificate }, isCertificateAccepted);
                rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, connectionInformation, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                eventSource.EmitClientAsyncDisconnect(rdpConnection, clientAsyncDisconnectArgs);
            }
        }

    }
}

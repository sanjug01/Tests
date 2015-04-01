using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Data;
using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Keyboard;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        private ApplicationDataModel _dataModel;
        private MouseViewModel _mouseViewModel;
        private TestDeferredExecution _testDispatcher;
        private ConnectionInformation _testConnectionInfo;
        private Mock.NavigationService _nav;
        private Mock.SessionModel _sessionModel;
        private Mock.RdpConnection _rdpConnection;
        private Mock.RdpCertificate _rdpCertificate;
        private RdpEventSource _eventSource;
        private SessionViewModel _vm;
        private TestData _testData;

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

        private sealed class TestDeferredExecution : IDeferredExecution
        {
            void IDeferredExecution.Defer(Action action)
            {
                action.Invoke();
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _testData = new TestData();
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };
            _mouseViewModel = new MouseViewModel();
            // use the test dispatcher to avoid deferring to UI
            _testDispatcher = new TestDeferredExecution();
            _testConnectionInfo = new ConnectionInformation()
            {
                Desktop = new DesktopModel() { HostName = "narf" },
                Credentials = new CredentialsModel() { Username = "don pedro", Password = "Chorizo" }
            };
            _eventSource = new RdpEventSource();
            _nav = new Mock.NavigationService();
            _sessionModel = new Mock.SessionModel();
            _rdpConnection = new Mock.RdpConnection(_eventSource);
            _rdpCertificate = new Mock.RdpCertificate();
            _vm = new SessionViewModel()
            {
                KeyboardCapture = new DummyKeyboardCapture(),
                SessionModel = _sessionModel,
                MouseViewModel = _mouseViewModel,
                ZoomPanViewModel = new ZoomPanViewModel(),
                PanKnobViewModel = new PanKnobViewModel()
            };
            ((IDataModelSite)_vm).SetDataModel(_dataModel);
            ((IDeferredExecutionSite)_vm).SetDeferredExecution(_testDispatcher);
            ((ITimerFactorySite)_vm).SetTimerFactory(new WinrtThreadPoolTimerFactory());
            ((IViewModel)_vm).Presenting(_nav, _testConnectionInfo, null);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _nav.Dispose();
            _sessionModel.Dispose();
            _rdpConnection.Dispose();
            _rdpCertificate.Dispose();
            _dataModel = null;
            _mouseViewModel = null;
            _testDispatcher = null;
        }

        [TestMethod]
        public void SessionViewModel_ShouldConnect()
        {

            _sessionModel.Expect("Connect", new List<object>() { _testConnectionInfo, null, null }, 0);
            _vm.ConnectCommand.Execute(null);            
            }

        [TestMethod]
        public void SessionViewModel_ShouldDisconnect()
        {
            _sessionModel.Expect("Disconnect", new List<object>() { }, 0);            
            _vm.DisconnectCommand.Execute(null);            
        }

        [TestMethod]
        public void SessionViewModel_ShouldHandleBackNavigationByDisconnecting()
        {
            _sessionModel.Expect("Disconnect", new List<object>() { }, 0);       
            IBackCommandArgs backArgs = new BackCommandArgs();
            Assert.IsFalse(backArgs.Handled);
            (_vm as IViewModel).NavigatingBack(backArgs);
            Assert.IsTrue(backArgs.Handled);
        }

        [TestMethod]
        public void SessionViewModel_ShouldCallHandleDisconnect_ShouldNavigateToHome()
        {
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _rdpConnection.Expect("Cleanup", new List<object> { }, null);
            _nav.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0);
            ClientDisconnectedArgs clientDisconnectedArgs = new ClientDisconnectedArgs(reason);

            _eventSource.EmitClientDisconnected(_rdpConnection, clientDisconnectedArgs);
        }

        [TestMethod]
        public void SessionViewModel_ShouldCallHandleDisconnect_ShouldDisplayError()
        {
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _rdpConnection.Expect("Cleanup", new List<object> { }, null);
            _nav.Expect("PushModalView", new List<object> { "ErrorMessageView", null, null }, null);
            _nav.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.ServerDeniedConnection, 0, 0);
            ClientDisconnectedArgs clientDisconnectedArgs = new ClientDisconnectedArgs(reason);

            _eventSource.EmitClientDisconnected(_rdpConnection, clientDisconnectedArgs);
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ConnectionShouldHandleDisconnect()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.ServerOutOfMemory, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool connectionShouldReconnect = false;

            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            _eventSource.EmitClientAsyncDisconnect(_rdpConnection, clientAsyncDisconnectArgs);            
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ShouldDisplayCertificateValidation()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool isCertificateAccepted = false;

            _rdpConnection.Expect("GetServerCertificate", new List<object> { }, _rdpCertificate);
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _sessionModel.Expect("IsCertificateAccepted", new List<object> { _rdpCertificate }, isCertificateAccepted);
            _nav.Expect("PushModalView", new List<object> { "CertificateValidationView", null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            _eventSource.EmitClientAsyncDisconnect(_rdpConnection, clientAsyncDisconnectArgs);            
        }

        [TestMethod]
        public void SessionViewModel_HandleAsyncDisconnect_ShouldReconnectIfCertificateAccepted()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool isCertificateAccepted = true;
            bool connectionShouldReconnect = true;

            _rdpConnection.Expect("GetServerCertificate", new List<object> { }, _rdpCertificate);
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _sessionModel.Expect("IsCertificateAccepted", new List<object> { _rdpCertificate }, isCertificateAccepted);
            _rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            _eventSource.EmitClientAsyncDisconnect(_rdpConnection, clientAsyncDisconnectArgs);            
        }

        /* ***********************************************************
         * *************** Reconnecting tests
         * ***********************************************************
         */
        [TestMethod]
        public void SessionViewModel_ShouldNotBeReconnecting()
        {
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

            Assert.IsFalse(_vm.IsReconnecting);
            Assert.IsTrue(0 == _vm.ReconnectAttempts);            
        }

        [TestMethod]
        public void SessionViewModel_HandleFirstAutoReconnecting_ShouldBeReconnecting()
        {
            bool eventEmitted = false;

            // first clientAutoReconnectingArgs would be null and attempts == 0
            int attempts = 0;
            ClientAutoReconnectingArgs clientAutoReconnectingArgs = null;

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
            Assert.IsFalse(_vm.IsReconnecting);
            Assert.IsTrue(0 == _vm.ReconnectAttempts);
                Assert.IsFalse(eventEmitted);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
            Assert.IsTrue(_vm.IsReconnecting);
            Assert.IsTrue(attempts == _vm.ReconnectAttempts);                

                // After a second attempt
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { eventEmitted = true; });
                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

            Assert.IsTrue(_vm.IsReconnecting);
            Assert.IsTrue(attempts == _vm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);
            }

        [TestMethod]
        public void SessionViewModel_HandleAutoReconnecting_ShouldBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool eventEmitted = false;
            int attempts = 1;

            ClientAutoReconnectingArgs clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                new AutoReconnectError(1),
                attempts,
                (b) => { eventEmitted = true; });

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
            Assert.IsFalse(_vm.IsReconnecting);
            Assert.IsTrue(0 == _vm.ReconnectAttempts);
                Assert.IsFalse(eventEmitted);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
            Assert.IsTrue(_vm.IsReconnecting);
            Assert.IsTrue(attempts == _vm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);

                // After a second attempt
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { eventEmitted = true; });
                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

            Assert.IsTrue(_vm.IsReconnecting);
            Assert.IsTrue(attempts == _vm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);
            }

        [TestMethod]
        public void SessionViewModel_HandleAutoReconnectComplete_ShouldNotBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool reconnectingEventEmitted = false;
            int attempts = 3;

            ClientAutoReconnectingArgs clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                new AutoReconnectError(1),
                attempts,
                (b) => { reconnectingEventEmitted = true; });

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            ConnectionAutoReconnectCompleteArgs connectionAutoReconnectCompleteArgs =
                new ConnectionAutoReconnectCompleteArgs(new ClientAutoReconnectCompleteArgs());

            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
            Assert.IsFalse(_vm.IsReconnecting);
            Assert.IsTrue(0 == _vm.ReconnectAttempts);
                Assert.IsFalse(reconnectingEventEmitted);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
            Assert.IsTrue(_vm.IsReconnecting);
            Assert.IsTrue(attempts == _vm.ReconnectAttempts);
                Assert.IsTrue(reconnectingEventEmitted);

                // After completion
            _sessionModel.EmitConnectionAutoReconnectComplete(connectionAutoReconnectCompleteArgs);
            Assert.IsFalse(_vm.IsReconnecting);            
            }

        [TestMethod]
        public void SessionViewModel_CancelAutoReconnect_ShouldNotBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            bool shouldContinueReconnecting = false;
            int attempts = 5;

            ClientAutoReconnectingArgs clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                new AutoReconnectError(1),
                attempts,
                (b) => { shouldContinueReconnecting = b; });

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            ConnectionAutoReconnectCompleteArgs connectionAutoReconnectCompleteArgs =
                new ConnectionAutoReconnectCompleteArgs(new ClientAutoReconnectCompleteArgs());

            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

            _vm.ConnectCommand.Execute(null);

            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
            Assert.IsFalse(_vm.IsReconnecting);
            Assert.IsTrue(0 == _vm.ReconnectAttempts);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);
                Assert.IsTrue(shouldContinueReconnecting);

                // After cancel
            _vm.CancelReconnectCommand.Execute(null);
            Assert.IsFalse(_vm.IsReconnecting);


                // next event should set shouldCancelReconnecting
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { shouldContinueReconnecting = b; });

                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
            _sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

            Assert.IsFalse(_vm.IsReconnecting);
                Assert.IsFalse(shouldContinueReconnecting);
            }

        private void SimulateConnect()
        {
            _sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
            _vm.ConnectCommand.Execute(null);
            ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(_rdpConnection);
            _sessionModel.EmitConnectionCreated(connectionCreatedArgs);            
        }
        private void SimulateAsyncDisconnect(RdpDisconnectCode code)
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(code, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            _eventSource.EmitClientAsyncDisconnect(_rdpConnection, clientAsyncDisconnectArgs);
        }

        private void SimulateDisconnect(RdpDisconnectCode code)
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(code, 0, 0);
            ClientDisconnectedArgs disconnectArgs = new ClientDisconnectedArgs(reason);
            //should cleanup connection on disconnect
            _rdpConnection.Expect("Cleanup", new List<object> { }, null);
            _eventSource.EmitClientDisconnected(_rdpConnection, disconnectArgs);
        }

        [TestMethod]
        public void PreAuthLogonFailedShowsAddUserViewWithCorrectParameters()
        {
            SimulateConnect();
            _nav.Expect("PushModalView", 
                p => 
                {
                    Assert.AreEqual("AddUserView", p[0]);
                    AddUserViewArgs args = p[1] as AddUserViewArgs;
                    Assert.IsNotNull(args);
                    Assert.AreEqual(CredentialPromptMode.InvalidCredentials, args.Mode);
                    Assert.AreEqual(true, args.ShowSave);
                    Assert.AreNotSame(_testConnectionInfo.Credentials, args.Credentials, "AddUser should not be directly passed credentials so they aren't overwritten");
                    Assert.AreEqual(_testConnectionInfo.Credentials.Username, args.Credentials.Username);
                    Assert.AreEqual(_testConnectionInfo.Credentials.Password, args.Credentials.Password);
                    return null;
                });
            SimulateAsyncDisconnect(RdpDisconnectCode.PreAuthLogonFailed);
        }

        [TestMethod]
        public void FreshCredsRequiredShowsAddUserViewWithCorrectParameters()
        {
            SimulateConnect();
            _nav.Expect("PushModalView",
                p =>
                {
                    Assert.AreEqual("AddUserView", p[0]);
                    AddUserViewArgs args = p[1] as AddUserViewArgs;
                    Assert.IsNotNull(args);
                    Assert.AreEqual(CredentialPromptMode.FreshCredentialsNeeded, args.Mode);
                    Assert.AreEqual(true, args.ShowSave);
                    Assert.AreNotSame(_testConnectionInfo.Credentials, args.Credentials, "AddUser should not be directly passed credentials so they aren't overwritten");
                    Assert.AreEqual(_testConnectionInfo.Credentials.Username, args.Credentials.Username);
                    Assert.AreEqual(_testConnectionInfo.Credentials.Password, args.Credentials.Password);
                    return null;
                });
            SimulateAsyncDisconnect(RdpDisconnectCode.FreshCredsRequired);
        }

        [TestMethod]
        public void EnteredCredsNotSavedIfUserChoosesNotTo()
        {
            _testConnectionInfo.Desktop = _testData.NewValidDesktop(Guid.Empty);
            _testConnectionInfo.Credentials = _testData.NewValidCredential().Model;

            CredentialPromptResult credentialPromptResult = CredentialPromptResult.CreateWithCredentials(_testData.NewValidCredential().Model, false);

            SimulateConnect();
            _nav.Expect("PushModalView",
                p =>
                {
                    Assert.AreEqual("AddUserView", p[0]);
                    IPresentationCompletion completionContext = p[2] as IPresentationCompletion;
                    //connection credentials should be set
                    _rdpConnection.Expect("SetCredentials", new List<object> { credentialPromptResult.Credentials, false }, null);
                    //reconnect should be attempted
                    _rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object> { null, true }, null);
                    completionContext.Completed(null, credentialPromptResult);
                    return null;
                });
            SimulateAsyncDisconnect(RdpDisconnectCode.FreshCredsRequired);

            Assert.AreEqual(credentialPromptResult.Credentials, _testConnectionInfo.Credentials, "connection credentials should be overwritten");
            Assert.AreEqual(Guid.Empty, _testConnectionInfo.Desktop.CredentialsId, "desktop should not be updated to use the unsaved credentials");
            //Assert.IsFalse(_dataModel.LocalWorkspace.Credentials.ContainsItemWithId(_testConnectionInfo.Credentials.Id), "credentials should not be saved to data model");
        }
    }
}

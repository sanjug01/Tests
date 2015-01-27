using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.CxWrappers.Errors;
using RdClient.Shared.Input.Keyboard;
using RdClient.Shared.Input.ZoomPan;
using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.Navigation.Extensions;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class SessionViewModelTests
    {
        private RdDataModel _dataModel;
        private MouseViewModel _mouseViewModel;
        private TestDeferredExecution _testDispatcher;
        private ConnectionInformation _testConnectionInfo;

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
            _dataModel = new RdDataModel();
            _mouseViewModel = new MouseViewModel();

            // use the test dispatcher to avoid deferring to UI
            _testDispatcher = new TestDeferredExecution();
            _testConnectionInfo = new ConnectionInformation()
            {
                Desktop = new Desktop(_dataModel.LocalWorkspace) { HostName = "narf" },
                Credentials = new Credentials() { Username = "don pedro", Domain = "Spain", Password = "Chorizo" }
            };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dataModel = null;
            _mouseViewModel = null;
            _testDispatcher = null;
        }

        [TestMethod]
        public void SessionViewModel_ShouldConnect()
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

                sessionModel.Expect("Connect", new List<object>() { _testConnectionInfo, null, null }, 0);
                //
                // TODO:    REFACTOR THIS!
                //          Present the view model using the nav service
                //
                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
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

                sessionModel.Expect("Disconnect", new List<object>() { }, 0);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
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
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
                rdpConnection.Expect("Cleanup", new List<object> { }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

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
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
                rdpConnection.Expect("Cleanup", new List<object> { }, null);
                navigation.Expect("PushModalView", new List<object> { "ErrorMessageView", null, null }, null);
                navigation.Expect("NavigateToView", new List<object> { "ConnectionCenterView", null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

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
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
                rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

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
                rdpConnection.Expect("GetServerCertificate", new List<object> { }, rdpCertificate);
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
                sessionModel.Expect("IsCertificateAccepted", new List<object> { rdpCertificate }, isCertificateAccepted);
                navigation.Expect("PushModalView", new List<object> { "CertificateValidationView", null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

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
                rdpConnection.Expect("GetServerCertificate", new List<object> { }, rdpCertificate);
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);
                sessionModel.Expect("IsCertificateAccepted", new List<object> { rdpCertificate }, isCertificateAccepted);
                rdpConnection.Expect("HandleAsyncDisconnectResult", new List<object>() { reason, connectionShouldReconnect }, 0);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                eventSource.EmitClientAsyncDisconnect(rdpConnection, clientAsyncDisconnectArgs);
            }
        }

        /* ***********************************************************
         * *************** Reconnecting tests
         * ***********************************************************
         */
        [TestMethod]
        public void SessionViewModel_ShouldNotBeReconnecting()
        {
            RdpEventSource eventSource = new RdpEventSource();

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsTrue(0 == svm.ReconnectAttempts);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleFirstAutoReconnecting_ShouldBeReconnecting()
        {
            RdpEventSource eventSource = new RdpEventSource();
            bool eventEmitted = false;

            // first clientAutoReconnectingArgs would be null and attempts == 0
            int attempts = 0;
            ClientAutoReconnectingArgs clientAutoReconnectingArgs = null;

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsTrue(0 == svm.ReconnectAttempts);
                Assert.IsFalse(eventEmitted);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
                Assert.IsTrue(svm.IsReconnecting);
                Assert.IsTrue(attempts == svm.ReconnectAttempts);

                // After a second attempt
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { eventEmitted = true; });
                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                Assert.IsTrue(svm.IsReconnecting);
                Assert.IsTrue(attempts == svm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleAutoReconnecting_ShouldBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
            bool eventEmitted = false;
            int attempts = 1;

            ClientAutoReconnectingArgs clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                new AutoReconnectError(1),
                attempts,
                (b) => { eventEmitted = true; });

            ConnectionAutoReconnectingArgs connectionAutoReconnectingArgs =
                new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsTrue(0 == svm.ReconnectAttempts);
                Assert.IsFalse(eventEmitted);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
                Assert.IsTrue(svm.IsReconnecting);
                Assert.IsTrue(attempts == svm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);

                // After a second attempt
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { eventEmitted = true; });
                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                Assert.IsTrue(svm.IsReconnecting);
                Assert.IsTrue(attempts == svm.ReconnectAttempts);
                Assert.IsTrue(eventEmitted);
            }
        }

        [TestMethod]
        public void SessionViewModel_HandleAutoReconnectComplete_ShouldNotBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
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

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsTrue(0 == svm.ReconnectAttempts);
                Assert.IsFalse(reconnectingEventEmitted);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                // After
                Assert.IsTrue(svm.IsReconnecting);
                Assert.IsTrue(attempts == svm.ReconnectAttempts);
                Assert.IsTrue(reconnectingEventEmitted);

                // After completion
                sessionModel.EmitConnectionAutoReconnectComplete(connectionAutoReconnectCompleteArgs);
                Assert.IsFalse(svm.IsReconnecting);
            }
        }

        [TestMethod]
        public void SessionViewModel_CancelAutoReconnect_ShouldNotBeReconnecting()
        {
            RdpDisconnectReason reason = new RdpDisconnectReason(RdpDisconnectCode.CertValidationFailed, 0, 0);
            ClientAsyncDisconnectArgs clientAsyncDisconnectArgs = new ClientAsyncDisconnectArgs(reason);
            RdpEventSource eventSource = new RdpEventSource();
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

            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {
                sessionModel.Expect("Connect", new List<object> { _testConnectionInfo, null, null }, null);

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };
                ((IDeferredExecutionSite)svm).SetDeferredExecution(_testDispatcher);

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);

                svm.ConnectCommand.Execute(null);

                ConnectionCreatedArgs connectionCreatedArgs = new ConnectionCreatedArgs(rdpConnection);
                sessionModel.EmitConnectionCreated(connectionCreatedArgs);

                // Before 
                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsTrue(0 == svm.ReconnectAttempts);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);
                Assert.IsTrue(shouldContinueReconnecting);

                // After cancel
                svm.CancelReconnectCommand.Execute(null);
                Assert.IsFalse(svm.IsReconnecting);


                // next event should set shouldCancelReconnecting
                attempts++;
                clientAutoReconnectingArgs = new ClientAutoReconnectingArgs(
                    new AutoReconnectError(1),
                    attempts,
                    (b) => { shouldContinueReconnecting = b; });

                connectionAutoReconnectingArgs = new ConnectionAutoReconnectingArgs(clientAutoReconnectingArgs);
                sessionModel.EmitConnectionAutoReconnecting(connectionAutoReconnectingArgs);

                Assert.IsFalse(svm.IsReconnecting);
                Assert.IsFalse(shouldContinueReconnecting);
            }
        }


        /* ***********************************************************
         * * *************** Tap&Zoom tests
         * * ***********************************************************
         * */

        [TestMethod]
        public void SessionViewModel_ZoomIn_ShouldNotifyZoomUpdate()
        {
            bool notificationFired = false;
            string zoomParam = SessionViewModel.ZOOM_IN_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) => 
                {
                    if ("ZoomUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                } );

                svm.ToggleZoomCommand.Execute(zoomParam);

                Assert.IsTrue(notificationFired);
                Assert.AreEqual(ZoomUpdateType.ZoomIn, svm.ZoomUpdate.ZoomType);
            }
        }

        [TestMethod]
        public void SessionViewModel_ZoomOut_ShouldNotifyZoomUpdate()
        {
            bool notificationFired = false;
            string zoomParam = SessionViewModel.ZOOM_OUT_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) =>
                {
                    if ("ZoomUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                });


                svm.ToggleZoomCommand.Execute(zoomParam);

                Assert.IsTrue(notificationFired);
                Assert.AreEqual(ZoomUpdateType.ZoomOut, svm.ZoomUpdate.ZoomType);
            }
        }

        [TestMethod]
        public void SessionViewModel_PanLeft_ShouldNotifyPanUpdate()
        {
            bool notificationFired = false;
            string panParam = SessionViewModel.PAN_LEFT_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) =>
                {
                    if ("PanUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                });


                svm.PanCommand.Execute(panParam);

                Assert.IsTrue(notificationFired);
                Assert.IsTrue(0.0 > svm.PanUpdate.X);
                Assert.IsTrue(0.0 == svm.PanUpdate.Y);
            }
        }

        [TestMethod]
        public void SessionViewModel_PanRight_ShouldNotifyPanUpdate()
        {
            bool notificationFired = false;
            string panParam = SessionViewModel.PAN_RIGHT_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) =>
                {
                    if ("PanUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                });


                svm.PanCommand.Execute(panParam);

                Assert.IsTrue(notificationFired);
                Assert.IsTrue(0.0 < svm.PanUpdate.X);
                Assert.IsTrue(0.0 == svm.PanUpdate.Y);
            }
        }

        [TestMethod]
        public void SessionViewModel_PanUp_ShouldNotifyPanUpdate()
        {
            bool notificationFired = false;
            string panParam = SessionViewModel.PAN_UP_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) =>
                {
                    if ("PanUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                });


                svm.PanCommand.Execute(panParam);

                Assert.IsTrue(notificationFired);
                Assert.IsTrue(0.0 == svm.PanUpdate.X);
                Assert.IsTrue(0.0 < svm.PanUpdate.Y);
            }
        }

        [TestMethod]
        public void SessionViewModel_PanDown_ShouldNotifyPanUpdate()
        {
            bool notificationFired = false;
            string panParam = SessionViewModel.PAN_DOWN_PARAM;
            RdpEventSource eventSource = new RdpEventSource();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.SessionModel sessionModel = new Mock.SessionModel())
            using (Mock.RdpConnection rdpConnection = new Mock.RdpConnection(eventSource))
            using (Mock.RdpCertificate rdpCertificate = new Mock.RdpCertificate())
            {

                SessionViewModel svm = new SessionViewModel()
                {
                    KeyboardCapture = new DummyKeyboardCapture(),
                    SessionModel = sessionModel,
                    DataModel = _dataModel,
                    MouseViewModel = _mouseViewModel
                };

                ((IViewModel)svm).Presenting(navigation, _testConnectionInfo, null);
                Assert.IsFalse(notificationFired);
                svm.PropertyChanged += ((sender, e) =>
                {
                    if ("PanUpdate".Equals(e.PropertyName))
                    {
                        notificationFired = true;
                    }
                });


                svm.PanCommand.Execute(panParam);

                Assert.IsTrue(notificationFired);
                Assert.IsTrue(0.0 == svm.PanUpdate.X);
                Assert.IsTrue(0.0 > svm.PanUpdate.Y);
            }
        }
    }
}

namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.UI.Xaml;

    [TestClass]
    public sealed partial class RemoteSessionViewModelTests
    {
        [TestInitialize]
        public void SetUpTest()
        {
            _inputPanelFactory = new TestInputPanelFactory();
            _vm = new RemoteSessionViewModel()
            {
                KeyboardCapture = new TestKeyboardCapture(),
                RightSideBarViewModel = new RightSideBarViewModel() { FullScreenModel = new TestFullScreenModel() },
                PointerPosition = new TestPointerPosition(),
                ScrollBarModel = new TestScrollBarModel(),
            };
            _vm.CastAndCall<IInputPanelFactorySite>(site => site.SetInputPanelFactory(_inputPanelFactory));

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();

            Guid credId = _dataModel.Credentials.AddNewModel(new CredentialsModel() { Username = "user", Password = "password" });
            _dataModel.LocalWorkspace.Connections.AddNewModel(new DesktopModel() { CredentialsId = credId, HostName = "192.168.2.2", FriendlyName = "localhost" });

            _timerFactory = new TestTimerFactory();
            _connectionSource = new TestConnectionSource();
            ((ITimerFactorySite)_vm).SetTimerFactory(_timerFactory);
            _viewFactory = new TestViewFactory(_vm);

            _defex = new TestDeferredExecution();
            _devCaps = new TestDeviceCapabilities();
            _lftManager = new TestLifeTimeManager();

            _nav = new NavigationService()
            {
                Presenter = new TestViewPresenter(),
                ViewFactory = _viewFactory,
                Extensions =
                {
                    new DataModelExtension() { AppDataModel = _dataModel },
                    new DeferredExecutionExtension(){ DeferredExecution = _defex },
                    new SessionFactoryExtension(){ SessionFactory = new TestSessionFactory() },
                    new DeviceCapabilitiesExtension() { DeviceCapabilities = _devCaps },
                    new LifeTimeExtension() { LifeTimeManager = _lftManager }
                }
            };
        }


        [TestCleanup]
        public void TearDownTest()
        {
            _nav = null;
            _vm = null;
            _inputPanelFactory = null;
            _viewFactory = null;
            _devCaps = null;
            _defex = null;
            _dataModel = null;
            _timerFactory = null;
            _connectionSource = null;
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentNewSession_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            int connectCount = 0;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    ++connectCount;
                };
            };

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            _defex.ExecuteAll();

            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsNotNull(_vm.BellyBandViewModel);
            Assert.IsInstanceOfType(_vm.BellyBandViewModel, typeof(RemoteSessionConnectingViewModel));
            Assert.AreEqual(Visibility.Collapsed, _vm.RightSideBarViewModel.Visibility);
            Assert.IsNotNull(connection);
            Assert.AreEqual(1, connectCount);

            SymbolBarButtonModel ellipsis = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.More);
            Assert.IsNotNull(ellipsis.Command);
            Assert.IsTrue(ellipsis.Command.CanExecute(null));

            // keyboard button is no longer a toggle button.
            SymbolBarButtonModel keyboard = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.Keyboard);

            Assert.IsNotNull(keyboard.Command);
            Assert.IsFalse(keyboard.Command.CanExecute(null));
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentNewSessionWithTouch_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            int connectCount = 0;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    ++connectCount;
                };
            };

            _devCaps.TouchPoints = 10;
            _devCaps.TouchPresent = true;

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            _defex.ExecuteAll();

            SymbolBarButtonModel keyboard = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.Keyboard);

            Assert.IsNotNull(keyboard.Command);
            // Assert.IsTrue(keyboard.Command.CanExecute(null));
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentWithVisibleInputPanel_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            int connectCount = 0;

            _inputPanelFactory.ShowPanel();
            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    ++connectCount;
                };
            };

            _devCaps.TouchPoints = 10;
            _devCaps.TouchPresent = true;

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            _defex.ExecuteAll();

            SymbolBarButtonModel keyboard = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.Keyboard);

            Assert.IsNotNull(keyboard.Command);
            // Assert.IsTrue(keyboard.Command.CanExecute(null));
        }

        // TestMethod - TODO: Keyboard panel functionality has changed
        // updates to this test needed
        public void RemoteSessionViewModel_ShowInputPanel_PanelHides()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            int connectCount = 0;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    ++connectCount;
                };
            };

            _devCaps.TouchPoints = 10;
            _devCaps.TouchPresent = true;

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            _defex.ExecuteAll();

            SymbolBarButtonModel keyboard = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.Keyboard);

            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));

            keyboard.Command.Execute(null);
        }

        // TestMethod - TODO: Keyboard panel functionality has changed
        // updates to this test needed
        public void RemoteSessionViewModel_HideInputPanel_PanelHides()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            int connectCount = 0;

            _inputPanelFactory.ShowPanel();
            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    ++connectCount;
                };
            };

            _devCaps.TouchPoints = 10;
            _devCaps.TouchPresent = true;

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            _defex.ExecuteAll();

            SymbolBarButtonModel keyboard = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.Keyboard);

            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));

            keyboard.Command.Execute(null);
        }

        [TestMethod]
        public void RemoteSessionViewModel_EmitConnected_ConnectedState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            Task connectTask = null;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    Assert.AreSame(s, activity);
                    connectTask = activity.AsyncConnect();
                };
            };

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            Assert.IsNotNull(connectTask);
            connectTask.Wait();
            //connectTask.Dispose();
            _defex.ExecuteAll();

            Assert.IsTrue(_vm.IsConnectionBarVisible);
            Assert.IsNull(_vm.BellyBandViewModel);
            Assert.AreEqual(Visibility.Collapsed, _vm.RightSideBarViewModel.Visibility);
        }

        [TestMethod]
        public void RemoteSessionViewModel_ConnectDisconnect_Disconnected()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            Task task = null;
            int cleanupCount = 0;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    Assert.AreSame(s, activity);
                    task = activity.AsyncConnect();
                };

                activity.Disconnect += (s, a) =>
                {
                    Assert.AreSame(s, activity);
                    task = activity.AsyncDisconnect(new RdpDisconnectReason(RdpDisconnectCode.UserInitiated, 0, 0));
                };

                activity.Cleanup += (s, a) =>
                {
                    ++cleanupCount;
                };
            };

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            task.Wait();
            //task.Dispose();
            task = null;
            _defex.ExecuteAll();
            _vm.RightSideBarViewModel.ToggleVisiblity.Execute(null);
            _vm.RightSideBarViewModel.Disconnect.Execute(null);

            Assert.IsNotNull(task);
            task.Wait();
            //task.Dispose();
            task = null;
            _defex.ExecuteAll();

            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsNull(_vm.BellyBandViewModel);
            Assert.AreEqual(1, cleanupCount);
        }

        [TestMethod]
        public void RemoteSessionViewModel_ConnectShowSideBars_SideBarsShown()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            Task connectTask = null;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    Assert.AreSame(s, activity);
                    connectTask = activity.AsyncConnect();
                };
            };

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            Assert.IsNotNull(connectTask);
            connectTask.Wait();
            //connectTask.Dispose();
            _defex.ExecuteAll();
            Assert.IsTrue(_vm.RightSideBarViewModel.ToggleVisiblity.CanExecute(null));
            _vm.RightSideBarViewModel.ToggleVisiblity.Execute(null);

            Assert.AreEqual(Visibility.Visible, _vm.RightSideBarViewModel.Visibility);
            Assert.IsTrue(_vm.RightSideBarViewModel.Disconnect.CanExecute(true));
        }

        [TestMethod]
        public void RemoteSessionViewModel_RequestFreshPassword_PasswordRequested()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory, new Mock.TestTelemetryClient());
            IRdpConnection connection = null;
            Task connectTask = null;
            int credentialsRequestCount = 0;

            _connectionSource.ConnectionCreated += (sender, e) =>
            {
                connection = e.Connection;
                Assert.IsNotNull(connection.Events);
                IConnectionActivity activity = (IConnectionActivity)connection;
                //
                // Subscribe for connection activity events;
                //
                activity.Connect += (s, a) =>
                {
                    Assert.AreSame(s, activity);
                    connectTask = activity.AsyncRequestFreshPassword();
                };
            };

            session.CredentialsNeeded += (sender, e) =>
            {
                credentialsRequestCount++;
            };

            _nav.NavigateToView("RemoteSessionView", session);
            ((IRemoteSessionViewSite)_vm).SetRemoteSessionView(_viewFactory.View);
            Assert.IsNotNull(connectTask);
            connectTask.Wait();
            //connectTask.Dispose();
            Assert.AreEqual(0, credentialsRequestCount);
            _defex.ExecuteAll();

            Assert.AreEqual(1, credentialsRequestCount);
            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsNotNull(_vm.BellyBandViewModel);
            Assert.IsInstanceOfType(_vm.BellyBandViewModel, typeof(RemoteSessionConnectingViewModel));
        }
    }
}

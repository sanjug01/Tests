namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.LifeTimeManagement;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.Foundation;

    [TestClass]
    public sealed class RemoteSessionViewModelTests
    {
        private INavigationService _nav;
        private TestInputPanelFactory _inputPanelFactory;
        private RemoteSessionViewModel _vm;
        private ApplicationDataModel _dataModel;
        private TestDeferredExecution _defex;
        private TestDeviceCapabilities _devCaps;
        private TestTimerFactory _timerFactory;
        private TestConnectionSource _connectionSource;
        private TestLifeTimeManager _lftManager;
        private TestViewFactory _viewFactory;

        private sealed class TestKeyboardCapture : IKeyboardCapture
        {
            event EventHandler<KeystrokeEventArgs> IKeyboardCapture.Keystroke
            {
                add { }
                remove { }
            }

            void IKeyboardCapture.Start() { }

            void IKeyboardCapture.Stop() { }
        }

        private sealed class TestInputPanelFactory : IInputPanelFactory
            {
            private readonly IInputPanel _inputPanel;

            private sealed class TestInputPanel : IInputPanel
            {
                private bool _isVisible;

                public TestInputPanel()
                {
                    _isVisible = false;
            }

                bool IInputPanel.IsVisible
            {
                    get { return _isVisible; }
            }

                public event EventHandler IsVisibleChanged;

                void IInputPanel.Hide()
                {
                    if(_isVisible)
                    {
                        _isVisible = false;
                        if (null != this.IsVisibleChanged)
                            this.IsVisibleChanged(this, EventArgs.Empty);
        }
                }

                void IInputPanel.Show()
                {
                    if (!_isVisible)
                    {
                        _isVisible = true;
                        if (null != this.IsVisibleChanged)
                            this.IsVisibleChanged(this, EventArgs.Empty);
                    }
                }
            }

            public TestInputPanelFactory()
            {
                _inputPanel = new TestInputPanel();
            }

            public void ShowPanel()
            {
                _inputPanel.Show();
            }

            public void HidePanel()
            {
                _inputPanel.Hide();
            }

            IInputPanel IInputPanelFactory.GetInputPanel()
            {
                return _inputPanel;
            }
        }

        private sealed class TestViewPresenter : IViewPresenter, IStackedViewPresenter
        {
            void IViewPresenter.PresentView(IPresentableView view) { }
            void IStackedViewPresenter.PushView(IPresentableView view, bool animated) { }
            void IStackedViewPresenter.DismissView(IPresentableView view, bool animated) { }
        }

        private sealed class TestViewport : IViewport
        {
            Point IViewport.Offset
            {
                get { return new Point(100, 200); }
            }

            Size IViewport.Size
            {
                get { return Size.Empty;  }
            }

            double IViewport.ZoomFactor
            {
                get { return 2.0; }
            }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { }
                remove { }
            }

            void IViewport.PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
            {
                throw new NotImplementedException();
            }

            void IViewport.Set(double zoomFactor, Point offset)
            {
                throw new NotImplementedException();
            }

            Point IViewport.TransformPoint(Point point)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestHelperView : IPresentableView
        {
            private readonly IViewModel _vm;

            public TestHelperView(IViewModel vm)
            {
                _vm = vm;
            }

            IViewModel IPresentableView.ViewModel
            {
                get { return _vm; }
            }

            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }
        }

        private sealed class TestView : MutableObject, IPresentableView, IRemoteSessionView
        {
            private readonly IViewModel _vm;
            private readonly IRenderingPanel _renderingPanel;

            private sealed class TestRenderingPanel : IRenderingPanel
            {
                private EventHandler _ready;

                public event EventHandler<IPointerEventBase> PointerChanged;

                event EventHandler IRenderingPanel.Ready
                {
                    add
                    {
                        _ready += value;
                        value(this, EventArgs.Empty);
                    }

                    remove
                    {
                        _ready -= value;
                    }
                }

                IViewport IRenderingPanel.Viewport
                {
                    get { return new TestViewport(); }
                }

                void IRenderingPanel.ChangeMouseCursorShape(Shared.Input.Pointer.MouseCursorShape shape)
                {
                    throw new NotImplementedException();
                }

                void IRenderingPanel.MoveMouseCursor(Point point)
                {
                    throw new NotImplementedException();
                }
            }

            public TestView(IViewModel vm)
            {
                Assert.IsNotNull(vm);
                _vm = vm;
                _renderingPanel = new TestRenderingPanel();
            }

            public IRenderingPanel RenderingPanel
            {
                get { return _renderingPanel; }
            }

            IViewModel IPresentableView.ViewModel
            {
                get { return _vm; }
            }

            void IPresentableView.Activating(object activationParameter) { }
            void IPresentableView.Presenting(INavigationService navigationService, object activationParameter) { }
            void IPresentableView.Dismissing() { }

            Size IRemoteSessionView.Size
            {
                get { throw new NotImplementedException(); }
            }

            event EventHandler IRemoteSessionView.Closed
            {
                add { throw new NotImplementedException(); }
                remove { throw new NotImplementedException(); }
            }

            IRenderingPanel IRemoteSessionView.ActivateNewRenderingPanel()
            {
                return _renderingPanel;
            }

            void IRemoteSessionView.RecycleRenderingPanel(IRenderingPanel renderingPanel)
            {
            }
        }

        private sealed class TestViewFactory : IPresentableViewFactory
        {
            private readonly IViewModel _vm;
            private TestView _view;

            public TestView View
            {
                get
                {
                    if (null == _view)
                        _view = new TestView(_vm);
                    return _view;
                }
            }

            public TestViewFactory(IViewModel vm)
            {
                _vm = vm;
            }

            IPresentableView IPresentableViewFactory.CreateView(string name, object activationParameter)
            {
                IPresentableView view = null;

                if (name.Equals("RemoteSessionView", StringComparison.Ordinal))
                {
                    view = this.View;
                }
                else if (name.Equals("InSessionEditCredentialsView", StringComparison.Ordinal))
                {
                    return new TestHelperView(new EditCredentialsViewModel());
                }
                else if (name.Equals("ConnectionCenterView", StringComparison.Ordinal))
                {
                    return new TestHelperView(new ConnectionCenterViewModel());
                }
                else
                {
                    Assert.Fail(string.Format("Unexpected view name \"{0}\"", name));
                }

                return view;
            }

            void IPresentableViewFactory.AddViewClass(string name, System.Type viewClass, bool isSingleton) { }
        }

        private sealed class TestDeferredExecution : MutableObject, IDeferredExecution
        {
            private readonly IList<Action> _actions = new List<Action>();

            public void ExecuteAll()
            {
                using (LockUpgradeableRead())
                {
                    foreach (Action a in _actions.ToArray())
                        a();

                    using (LockWrite())
                        _actions.Clear();
                }
            }

            void IDeferredExecution.Defer(Action action)
            {
                using(LockWrite())
                    _actions.Add(action);
            }
        }

        private sealed class TestTimerFactory : ITimerFactory
        {
            private sealed class Timer : ITimer
            {
                void ITimer.Start(Action callback, TimeSpan period, bool recurring)
                {
                }

                void ITimer.Stop()
                {
                }
            }

            ITimer ITimerFactory.CreateTimer()
            {
                return new Timer();
            }
        }

        private interface IConnectionActivity
        {
            event EventHandler Connect;
            event EventHandler Disconnect;
            event EventHandler Cleanup;

            Task AsyncConnect();

            Task AsyncRequestFreshPassword();

            Task AsyncDisconnect(RdpDisconnectReason reason);
        }

        private sealed class TestSessionFactory : ISessionFactory
        {
            IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestDeviceCapabilities : MutableObject, IDeviceCapabilities
        {
            private uint _touchPoints = 0;
            private bool _touchPresent = false;
            private bool _canShowInputPanel;

            public uint TouchPoints { set { SetProperty(ref _touchPoints, value); } }
            public bool TouchPresent { set { SetProperty(ref _touchPresent, value); } }
            public bool CanShowInputPanel { set { SetProperty(ref _canShowInputPanel, value); } }

            uint IDeviceCapabilities.TouchPoints { get { return _touchPoints; } }
            bool IDeviceCapabilities.TouchPresent { get { return _touchPresent; } }
            bool IDeviceCapabilities.CanShowInputPanel { get { return _canShowInputPanel; } }
            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
        }

        private sealed class TestLifeTimeManager : ILifeTimeManager
        {
            event EventHandler<LaunchEventArgs> ILifeTimeManager.Launched { add { } remove { } }
            event EventHandler<ResumeEventArgs> ILifeTimeManager.Resuming { add { } remove { } }
            event EventHandler<SuspendEventArgs> ILifeTimeManager.Suspending { add { } remove { } }

            void ILifeTimeManager.OnLaunched(IActivationArgs e)
            {
                throw new NotImplementedException();
            }

            void ILifeTimeManager.OnResuming(object sender, IResumingArgs e)
            {
                throw new NotImplementedException();
            }

            void ILifeTimeManager.OnSuspending(object sender, ISuspensionArgs e)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestConnectionSource : IRdpConnectionSource
        {
            private readonly IRdpConnectionFactory _factory;

            public sealed class ConnectionEventArgs : EventArgs
            {
                private readonly IRdpConnection _connection;

                public IRdpConnection Connection { get { return _connection; } }

                public ConnectionEventArgs(IRdpConnection connection)
                {
                    _connection = connection;
                }
            }

            public event EventHandler<ConnectionEventArgs> ConnectionCreated;

            private sealed class Factory : IRdpConnectionFactory
            {
                IRdpConnection IRdpConnectionFactory.CreateDesktop(string rdpFile)
                {
                    return new Connection();
                }

                IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
                {
                    return new Connection();
                }
            }

            private sealed class Connection : IRdpConnection, IRdpProperties, IConnectionActivity
            {
                private readonly RdpEventSource _events;

                public Connection()
                {
                    _events = new RdpEventSource();
                }

                IRdpEvents IRdpConnection.Events
                {
                    get { return _events; }
                }

                /// <summary>
                /// Set credentials is called prior to Connect
                /// </summary>
                /// <param name="credentials">credentials</param>
                /// <param name="fUsingSavedCreds">indicates if credentials are saved </param>
                void IRdpConnection.SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
                {                
                }
                void IRdpConnection.SetGateway(GatewayModel gateway, CredentialsModel credentials)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Connect()
                {
                    if (null != this.Connect)
                        this.Connect(this, EventArgs.Empty);
                }

                void IRdpConnection.Disconnect()
                {
                    if (null != this.Disconnect)
                        this.Disconnect(this, EventArgs.Empty);
                }

                void IRdpConnection.Suspend()
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Resume()
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.TerminateInstance()
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Cleanup()
                {
                    if (null != this.Cleanup)
                        this.Cleanup(this, EventArgs.Empty);
                }

                void IRdpConnection.HandleAsyncDisconnectResult(RdpDisconnectReason disconnectReason, bool reconnectToServer)
                {
                    throw new NotImplementedException();
                }

                IRdpScreenSnapshot IRdpConnection.GetSnapshot()
                {
                    throw new NotImplementedException();
                }

                IRdpCertificate IRdpConnection.GetServerCertificate()
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.SendMouseEvent(MouseEventType type, float xPos, float yPos)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.SendKeyEvent(int keyValue, bool scanCode, bool extended, bool keyUp)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.SendTouchEvent(TouchEventType type, uint contactId, Windows.Foundation.Point position, ulong frameTime)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.SetLeftHandedMouseMode(bool value)
                {
                }

                int IRdpProperties.GetIntProperty(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetIntProperty(string propertyName, int value)
                {
                }

                string IRdpProperties.GetStringPropery(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetStringProperty(string propertyName, string value)
                {
                }

                bool IRdpProperties.GetBoolProperty(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetBoolProperty(string propertyName, bool value)
                {
                }
                //
                // IConnectionActivity
                //
                public event EventHandler Connect;
                public event EventHandler Disconnect;
                public event EventHandler Cleanup;

                Task IConnectionActivity.AsyncConnect()
                {
                    Task t = new Task(() =>
                    {
                        Task.Delay(1).Wait();
                        _events.EmitClientConnected(this, new ClientConnectedArgs());
                    }, TaskCreationOptions.LongRunning);
                    t.Start();
                    return t;
                }

                Task IConnectionActivity.AsyncRequestFreshPassword()
                {
                    Task t = new Task(() =>
                    {
                        Task.Delay(1).Wait();
                        _events.EmitClientAsyncDisconnect(this, new ClientAsyncDisconnectArgs(new RdpDisconnectReason(RdpDisconnectCode.FreshCredsRequired, 0, 0)));
                    }, TaskCreationOptions.LongRunning);
                    t.Start();

                    return t;
                }

                Task IConnectionActivity.AsyncDisconnect(RdpDisconnectReason reason)
                {
                    Task t = new Task(() =>
                    {
                        Task.Delay(1).Wait();
                        _events.EmitClientDisconnected(this, new ClientDisconnectedArgs(reason));
                    }, TaskCreationOptions.LongRunning);
                    t.Start();

                    return t;
                }
            }

            public TestConnectionSource()
            {
                _factory = new Factory();
            }

            IRdpConnection IRdpConnectionSource.CreateConnection(RemoteConnectionModel model, IRenderingPanel renderingPanel)
            {
                IRdpConnection connection = model.CreateConnection(_factory, renderingPanel);

                if (null != this.ConnectionCreated)
                    this.ConnectionCreated(this, new ConnectionEventArgs(connection));

                return connection;
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _inputPanelFactory = new TestInputPanelFactory();
            _vm = new RemoteSessionViewModel()
            {
                KeyboardCapture = new TestKeyboardCapture(),
                InputPanelFactory = _inputPanelFactory
            };

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
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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
            Assert.IsFalse(_vm.IsRightSideBarVisible);
            Assert.IsNotNull(connection);
            Assert.AreEqual(1, connectCount);

            SymbolBarButtonModel ellipsis = (SymbolBarButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarButtonModel && ((SymbolBarButtonModel)o).Glyph == SegoeGlyph.AllApps);
            Assert.IsNotNull(ellipsis.Command);
            Assert.IsTrue(ellipsis.Command.CanExecute(null));

            SymbolBarToggleButtonModel keyboard = (SymbolBarToggleButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarToggleButtonModel && ((SymbolBarToggleButtonModel)o).Glyph == SegoeGlyph.Keyboard);
            Assert.IsNotNull(keyboard.Command);
            Assert.IsFalse(keyboard.Command.CanExecute(null));
            Assert.IsFalse(keyboard.IsChecked);
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentNewSessionWithTouch_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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

            SymbolBarToggleButtonModel keyboard = (SymbolBarToggleButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarToggleButtonModel && ((SymbolBarToggleButtonModel)o).Glyph == SegoeGlyph.Keyboard);
            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));
            Assert.IsFalse(keyboard.IsChecked);
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentWithVisibleInputPanel_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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

            SymbolBarToggleButtonModel keyboard = (SymbolBarToggleButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarToggleButtonModel && ((SymbolBarToggleButtonModel)o).Glyph == SegoeGlyph.Keyboard);
            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));
            Assert.IsTrue(keyboard.IsChecked);
        }

        [TestMethod]
        public void RemoteSessionViewModel_ShowInputPanel_PanelHides()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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

            SymbolBarToggleButtonModel keyboard = (SymbolBarToggleButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarToggleButtonModel && ((SymbolBarToggleButtonModel)o).Glyph == SegoeGlyph.Keyboard);
            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));
            Assert.IsFalse(keyboard.IsChecked);
            keyboard.Command.Execute(null);
            Assert.IsTrue(keyboard.IsChecked);
        }

        [TestMethod]
        public void RemoteSessionViewModel_HideInputPanel_PanelHides()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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

            SymbolBarToggleButtonModel keyboard = (SymbolBarToggleButtonModel)_vm.ConnectionBarItems.First(
                o => o is SymbolBarToggleButtonModel && ((SymbolBarToggleButtonModel)o).Glyph == SegoeGlyph.Keyboard);
            Assert.IsNotNull(keyboard.Command);
            Assert.IsTrue(keyboard.Command.CanExecute(null));
            Assert.IsTrue(keyboard.IsChecked);
            keyboard.Command.Execute(null);
            Assert.IsFalse(keyboard.IsChecked);
        }

        [TestMethod]
        public void RemoteSessionViewModel_EmitConnected_ConnectedState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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
            Assert.IsFalse(_vm.IsRightSideBarVisible);
        }

        [TestMethod]
        public void RemoteSessionViewModel_ConnectDisconnect_Disconnected()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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
            _vm.ToggleSideBars.Execute(null);
            _vm.NavigateHome.Execute(null);

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
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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
            Assert.IsTrue(_vm.ToggleSideBars.CanExecute(null));
            _vm.ToggleSideBars.Execute(null);

            Assert.IsTrue(_vm.IsRightSideBarVisible);
            Assert.IsTrue(_vm.NavigateHome.CanExecute(true));
        }

        [TestMethod]
        public void RemoteSessionViewModel_RequestFreshPassword_PasswordRequested()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);
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
            Assert.IsFalse(_vm.IsRightSideBarVisible);
        }
    }
}

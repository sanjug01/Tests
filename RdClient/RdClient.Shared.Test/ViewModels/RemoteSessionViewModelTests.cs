namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.Test.Data;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Windows.Foundation;

    [TestClass]
    public sealed class RemoteSessionViewModelTests
    {
        private INavigationService _nav;
        private RemoteSessionViewModel _vm;
        private ApplicationDataModel _dataModel;
        private TestDeferredExecution _defex;
        private TestTimerFactory _timerFactory;
        private TestConnectionSource _connectionSource;
        private TestViewFactory _viewFactory;

        private sealed class TestKeyboardCapture : IKeyboardCapture
        {
            event EventHandler<KeystrokeEventArgs> IKeyboardCapture.Keystroke
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

        private sealed class TestViewPresenter : IViewPresenter
        {
            void IViewPresenter.PresentView(IPresentableView view) { }
            void IViewPresenter.PushModalView(IPresentableView view) { }
            void IViewPresenter.DismissModalView(IPresentableView view) { }
            void IViewPresenter.PresentingFirstModalView() { }
            void IViewPresenter.DismissedLastModalView() { }
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
                    foreach (Action a in _actions)
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

                void IRdpConnection.SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Connect(CredentialsModel credentials, bool fUsingSavedCreds)
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
            _vm = new RemoteSessionViewModel()
            {
                KeyboardCapture = new TestKeyboardCapture()
            };

            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer()
            };

            Guid credId = _dataModel.LocalWorkspace.Credentials.AddNewModel(new CredentialsModel() { Username = "user", Password = "password" });
            _dataModel.LocalWorkspace.Connections.AddNewModel(new DesktopModel() { CredentialsId = credId, HostName = "192.168.2.2", FriendlyName = "localhost" });

            _timerFactory = new TestTimerFactory();
            _connectionSource = new TestConnectionSource();
            _viewFactory = new TestViewFactory(_vm);

            _defex = new TestDeferredExecution();

            _nav = new NavigationService()
            {
                Presenter = new TestViewPresenter(),
                ViewFactory = _viewFactory,
                Extensions =
                {
                    new DataModelExtension() { AppDataModel = _dataModel },
                    new DeferredExecutionExtension(){ DeferredExecution = _defex },
                    new SessionFactoryExtension(){ SessionFactory = new TestSessionFactory() }
                }
            };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _nav = null;
            _vm = null;
            _viewFactory = null;
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

            Assert.IsTrue(_vm.IsConnected);
            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsFalse(_vm.IsFailureMessageVisible);
            Assert.IsFalse(_vm.IsRightSideBarVisible);
            Assert.IsFalse(_vm.IsInterrupted);
            Assert.IsNotNull(connection);
            Assert.AreEqual(1, connectCount);
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
            connectTask.Dispose();
            _defex.ExecuteAll();

            Assert.IsTrue(_vm.IsConnected);
            Assert.IsTrue(_vm.IsConnectionBarVisible);
            Assert.IsFalse(_vm.IsFailureMessageVisible);
            Assert.IsFalse(_vm.IsRightSideBarVisible);
            Assert.IsFalse(_vm.IsInterrupted);
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
            task.Dispose();
            task = null;
            _defex.ExecuteAll();
            _vm.ShowSideBars.Execute(null);
            _vm.NavigateHome.Execute(null);

            Assert.IsNotNull(task);
            task.Wait();
            task.Dispose();
            task = null;
            _defex.ExecuteAll();

            Assert.IsFalse(_vm.IsConnected);
            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsFalse(_vm.IsFailureMessageVisible);
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
            connectTask.Dispose();
            _defex.ExecuteAll();
            Assert.IsTrue(_vm.ShowSideBars.CanExecute(null));
            _vm.ShowSideBars.Execute(null);

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
            connectTask.Dispose();
            Assert.AreEqual(0, credentialsRequestCount);
            _defex.ExecuteAll();

            Assert.AreEqual(1, credentialsRequestCount);
            Assert.IsTrue(_vm.IsConnected);
            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsFalse(_vm.IsFailureMessageVisible);
            Assert.IsFalse(_vm.IsRightSideBarVisible);
            Assert.IsFalse(_vm.IsInterrupted);
        }
    }
}

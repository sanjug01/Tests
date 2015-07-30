namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Input.Keyboard;
    using RdClient.Shared.Input.Pointer;
    using RdClient.Shared.LifeTimeManagement;
    using RdClient.Shared.Models;
    using RdClient.Shared.Models.PanKnobModel;
    using RdClient.Shared.Models.Viewport;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using Windows.Foundation;
    using Windows.UI.ViewManagement;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Media;

    public sealed partial class RemoteSessionViewModelTests
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

        private sealed class TestPointerPosition : IPointerPosition
        {
            public Point SessionPosition { get; set; }

            public Point ViewportPosition { get; set; }

            public event EventHandler<Point> PositionChanged;
            public void EmitPositionChanged(Point point)
            {
                if (PositionChanged != null)
                {
                    PositionChanged(this, point);
                }
            }

            public void Reset(IRenderingPanel renderingPanel, IExecutionDeferrer executionDeferrer)
            {
                // no action
            }
        }

        private sealed class TestScrollBarModel : IScrollBarModel
        {
            public double HorizontalScrollBarWidth { set; private get; }
            public double VerticalScrollBarWidth { set; private get; }
            public double MaximumHorizontal { private set; get; }
            public double MaximumVertical { private set; get; }
            public double MinimumHorizontal { private set; get; }
            public double MinimumVertical { private set; get; }
            public double ValueHorziontal { set; get; }
            public double ValueVertical { set; get; }
            public IViewport Viewport { private get; set; }

            public double ViewportHeight { get { return 100; } }
            public double ViewportWidth { get { return 100; } }

            public Visibility VisibilityCorner { get { return Visibility.Visible; } }
            public Visibility VisibilityHorizontal { get { return Visibility.Collapsed; } }
            public Visibility VisibilityVertical { get { return Visibility.Visible; } }

            public void OnPointerChanged(object sender, IPointerEventBase e)
            {
                throw new NotImplementedException();
            }

            public void SetScrollbarVisibility(Visibility visibility)
            {
                // noop
            }
        }

        private sealed class TestPanKnob : IPanKnob
        {
            public bool IsVisible { get; set; }
            public Point Position { get; set; }
            public Size Size { get; private set; }
        }

        private sealed class TestFullScreenModel : IFullScreenModel
        {
            private EventHandler _fullScreenChange;
            private EventHandler _userInteractionModeChange;
            private EventHandler _enteringFullScreen;
            private EventHandler _exitingFullScreen;

            public event EventHandler EnteredFullScreen { add { } remove { } }
            public event EventHandler ExitedFullScreen { add { } remove { } }

            public TestFullScreenModel()
            {
                EnterFullScreenCommand = new RelayCommand(o => { });
                ExitFullScreenCommand = new RelayCommand(o => { });
            }

            public ICommand EnterFullScreenCommand { get; private set; }
            public ICommand ExitFullScreenCommand { get; private set; }
            public bool IsFullScreenMode { get; private set; }

            public UserInteractionMode UserInteractionMode { get; private set; }

            event EventHandler IFullScreenModel.EnteringFullScreen
            {
                add { _enteringFullScreen += value; }
                remove { _enteringFullScreen -= value; }
            }

            event EventHandler IFullScreenModel.ExitingFullScreen
            {
                add { _exitingFullScreen += value; }
                remove { _exitingFullScreen -= value; }
            }

            public event EventHandler FullScreenChange
            {
                add { _fullScreenChange += value; }
                remove { _fullScreenChange -= value; }
            }

            public event EventHandler UserInteractionModeChange
            {
                add { _userInteractionModeChange += value; }
                remove { _userInteractionModeChange -= value; }
            }

            public void ToggleFullScreen()
            {
                // noop
            }

            public void EnterFullScreen()
            {
            }

            public void ExitFullScreen()
            {
            }
        }

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
            private EventHandler _changed;

            public IViewportPanel SessionPanel
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

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

            public event EventHandler Changed
            {
                add { _changed += value; }
                remove { _changed -= value; }
            }

            event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
            {
                add { }
                remove { }
            }

            public void Set(double zoomFactor, Point anchorPoint)
            {
                throw new NotImplementedException();
            }

            void IViewport.PanAndZoom(Point anchorPoint, double dx, double dy, double scaleFactor)
            {
                throw new NotImplementedException();
            }

            void IViewport.SetZoom(double zoomFactor, Point anchorPoint)
            {
                throw new NotImplementedException();
            }

            void IViewport.SetPan(double x, double y)
            {
                throw new NotImplementedException();
            }

            public void Reset()
            {
                // noop - this is actually used implicitly
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
            private EventHandler<IPointerEventBase> _pointerChanged;

            public event EventHandler<IPointerEventBase> PointerChanged
            {
                add { _pointerChanged += value; }
                remove { _pointerChanged -= value; }
            }

            private sealed class TestRenderingPanel : IRenderingPanel
            {
                private EventHandler _ready;
                private EventHandler<IPointerEventBase> _pointerChanged;

                public event EventHandler<IPointerEventBase> PointerChanged
                {
                    add { _pointerChanged += value; }
                    remove { _pointerChanged -= value; }
                }

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

                public IScaleFactor ScaleFactor
                {
                    get
                    {
                        throw new NotImplementedException();
                    }
                }

                void IRenderingPanel.ChangeMouseCursorShape(ImageSource shape, Point hotspot)
                {
                    throw new NotImplementedException();
                }

                 void IRenderingPanel.MoveMouseCursor(Point point)
                {
                    throw new NotImplementedException();
                }

                public void OnMouseVisibilityChanged(object sender, PropertyChangedEventArgs e)
                {
                    throw new NotImplementedException();
                }

                public void ChangeMouseVisibility(Visibility visibility)
                {
                    // noop
                }

                public void ScaleMouseCursor(double scale)
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

            public double Width
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public double Height
            {
                get
                {
                    throw new NotImplementedException();
                }

                set
                {
                    throw new NotImplementedException();
                }
            }

            public IViewportTransform Transform
            {
                get
                {
                    throw new NotImplementedException();
                }
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

            public ITimer CreateDispatcherTimer()
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
            string IDeviceCapabilities.UserInteractionModeLabel { get { return "Mouse"; } }
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

                IRdpCertificate IRdpConnection.GetGatewayCertificate()
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

    }
}

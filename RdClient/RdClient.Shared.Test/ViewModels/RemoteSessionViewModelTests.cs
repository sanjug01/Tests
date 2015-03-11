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

    [TestClass]
    public sealed class RemoteSessionViewModelTests
    {
        private INavigationService _nav;
        private RemoteSessionViewModel _vm;
        private ApplicationDataModel _dataModel;
        private TestDeferredExecution _defex;
        private TestTimerFactory _timerFactory;
        private TestConnectionSource _connectionSource;

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

        private sealed class TestView : IPresentableView
        {
            private readonly IViewModel _vm;

            public TestView(IViewModel vm)
            {
                Assert.IsNotNull(vm);
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

        private sealed class TestViewFactory : IPresentableViewFactory
        {
            private readonly IViewModel _vm;

            public TestViewFactory(IViewModel vm)
            {
                _vm = vm;
            }

            IPresentableView IPresentableViewFactory.CreateView(string name, object activationParameter)
            {
                return new TestView(_vm);
            }

            void IPresentableViewFactory.AddViewClass(string name, System.Type viewClass, bool isSingleton) { }
        }

        private sealed class TestDeferredExecution : IDeferredExecution
        {
            private readonly IList<Action> _actions = new List<Action>();

            public void ExecuteAll()
            {
                foreach (Action a in _actions)
                    a();
                _actions.Clear();
            }

            void IDeferredExecution.Defer(Action action)
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestTimerFactory : ITimerFactory
        {
            private sealed class Timer : ITimer
            {
                void ITimer.Start(Action callback, TimeSpan period, bool recurring)
                {
                    throw new NotImplementedException();
                }

                void ITimer.Stop()
                {
                    throw new NotImplementedException();
                }
            }

            ITimer ITimerFactory.CreateTimer()
            {
                return new Timer();
            }
        }

        private sealed class TestConnectionSource : IRdpConnectionSource
        {
            private readonly IRdpConnectionFactory _factory;

            private sealed class Factory : IRdpConnectionFactory
            {
                IRdpConnection IRdpConnectionFactory.CreateDesktop()
                {
                    return new Connection();
                }

                IRdpConnection IRdpConnectionFactory.CreateApplication(string rdpFile)
                {
                    return new Connection();
                }
            }

            private sealed class Connection : IRdpConnection, IRdpProperties
            {

                IRdpEvents IRdpConnection.Events
                {
                    get { throw new NotImplementedException(); }
                }

                void IRdpConnection.SetCredentials(CredentialsModel credentials, bool fUsingSavedCreds)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Connect(CredentialsModel credentials, bool fUsingSavedCreds)
                {
                    throw new NotImplementedException();
                }

                void IRdpConnection.Disconnect()
                {
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
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
                    throw new NotImplementedException();
                }

                int IRdpProperties.GetIntProperty(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetIntProperty(string propertyName, int value)
                {
                    throw new NotImplementedException();
                }

                string IRdpProperties.GetStringPropery(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetStringProperty(string propertyName, string value)
                {
                    throw new NotImplementedException();
                }

                bool IRdpProperties.GetBoolProperty(string propertyName)
                {
                    throw new NotImplementedException();
                }

                void IRdpProperties.SetBoolProperty(string propertyName, bool value)
                {
                    throw new NotImplementedException();
                }
            }

            public TestConnectionSource()
            {
                _factory = new Factory();
            }

            IRdpConnection IRdpConnectionSource.CreateConnection(RemoteConnectionModel model, IRenderingPanel renderingPanel)
            {
                throw new NotImplementedException();
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

            _defex = new TestDeferredExecution();

            _nav = new NavigationService()
            {
                Presenter = new TestViewPresenter(),
                ViewFactory = new TestViewFactory(_vm),
                Extensions =
                {
                    new DataModelExtension() { AppDataModel = _dataModel }
                }
            };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _nav = null;
            _vm = null;
        }

        [TestMethod]
        public void RemoteSessionViewModel_PresentNewSession_CorrectInitialState()
        {
            RemoteSessionSetup setup = new RemoteSessionSetup(_dataModel, _dataModel.LocalWorkspace.Connections.Models[0].Model);
            IRemoteSession session = new RemoteSession(setup, _defex, _connectionSource, _timerFactory);

            _nav.NavigateToView("RemoteSessionView", session);

            Assert.IsFalse(_vm.IsConnected);
            Assert.IsFalse(_vm.IsConnectionBarVisible);
            Assert.IsFalse(_vm.IsFailureMessageVisible);
            Assert.IsFalse(_vm.IsRightSideBarVisible);
            Assert.IsFalse(_vm.IsInterrupted);
        }
    }
}

namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using System;
    using System.Collections.Generic;
    using Windows.UI.ViewManagement;

    [TestClass]
    public sealed class InSessionMenusModelTests
    {
        private sealed class TestSession : RdMock.MockBase, IRemoteSession
        {
            ICertificateTrust IRemoteSession.CertificateTrust
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            string IRemoteSession.HostName
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            bool IRemoteSession.IsServerTrusted
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

            IRemoteSessionState IRemoteSession.State
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<BadCertificateEventArgs> IRemoteSession.BadCertificate
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<BadServerIdentityEventArgs> IRemoteSession.BadServerIdentity
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler IRemoteSession.Closed
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<CredentialsNeededEventArgs> IRemoteSession.CredentialsNeeded
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<SessionFailureEventArgs> IRemoteSession.Failed
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<SessionInterruptedEventArgs> IRemoteSession.Interrupted
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<MouseCursorShapeChangedArgs> IRemoteSession.MouseCursorShapeChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            event EventHandler<MultiTouchEnabledChangedArgs> IRemoteSession.MultiTouchEnabledChanged
            {
                add
                {
                    throw new NotImplementedException();
                }

                remove
                {
                    throw new NotImplementedException();
                }
            }

            IRemoteSessionControl IRemoteSession.Activate(IRemoteSessionView sessionView)
            {
                throw new NotImplementedException();
            }

            IRenderingPanel IRemoteSession.Deactivate()
            {
                throw new NotImplementedException();
            }

            void IRemoteSession.Disconnect()
            {
                Invoke(new object[] { });
            }

            void IRemoteSession.Resume()
            {
                throw new NotImplementedException();
            }

            void IRemoteSession.Suspend()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestFullScreenModel : RdMock.MockBase, IFullScreenModel
        {
            private bool _isFullScreenMode;
            private UserInteractionMode _userInteractionMode;
            private EventHandler _enteredFullScreen;
            private EventHandler _enteringFullScreen;
            private EventHandler _exitedFullScreen;
            private EventHandler _exitingFullScreen;
            private EventHandler _fullScreenChange;
            private EventHandler _userInteractionModeChange;

            public void SetFullScreenMode(bool isFullScreen)
            {
                if(isFullScreen != _isFullScreenMode)
                {
                    EmitEnteringFullScreen();
                    _isFullScreenMode = isFullScreen;
                    EmitFullScreenChange();
                    EmitEnteredFullScreen();
                }
            }

            public void SetUserInteractionMode(UserInteractionMode mode)
            {
                if(mode != _userInteractionMode)
                {
                    _userInteractionMode = mode;
                    EmitUserInteractionModeChange();
                }
            }

            public bool IsFullScreenMode
            {
                get { return _isFullScreenMode; }
            }

            UserInteractionMode IFullScreenModel.UserInteractionMode
            {
                get { return _userInteractionMode; }
            }

            event EventHandler IFullScreenModel.EnteredFullScreen
            {
                add { _enteredFullScreen += value; }
                remove { _enteredFullScreen -= value; }
            }

            event EventHandler IFullScreenModel.EnteringFullScreen
            {
                add { _enteringFullScreen += value; }
                remove { _enteringFullScreen -= value; }
            }

            event EventHandler IFullScreenModel.ExitedFullScreen
            {
                add { _exitedFullScreen += value; }
                remove { _exitedFullScreen -= value; }
            }

            event EventHandler IFullScreenModel.ExitingFullScreen
            {
                add { _exitingFullScreen += value; }
                remove { _exitingFullScreen -= value; }
            }

            event EventHandler IFullScreenModel.FullScreenChange
            {
                add { _fullScreenChange += value; }
                remove { _fullScreenChange -= value; }
            }

            event EventHandler IFullScreenModel.UserInteractionModeChange
            {
                add { _userInteractionModeChange += value; }
                remove { _userInteractionModeChange -= value; }
            }

            void IFullScreenModel.EnterFullScreen()
            {
                Invoke(new object[] { });
            }

            void IFullScreenModel.ExitFullScreen()
            {
                Invoke(new object[] { });
            }

            private void EmitEnteringFullScreen()
            {
                if (null != _enteringFullScreen)
                    _enteringFullScreen(this, EventArgs.Empty);
            }

            private void EmitEnteredFullScreen()
            {
                if (null != _enteredFullScreen)
                    _enteredFullScreen(this, EventArgs.Empty);
            }

            private void EmitFullScreenChange()
            {
                if (null != _fullScreenChange)
                    _fullScreenChange(this, EventArgs.Empty);
            }

            private void EmitUserInteractionModeChange()
            {
                if (null != _userInteractionModeChange)
                    _userInteractionModeChange(this, EventArgs.Empty);
            }
        }

        private sealed class TestDeferredExecution : IDeferredExecution
        {
            public readonly IList<Action> Actions = new List<Action>();

            public void ExecuteDeferred()
            {
                foreach (Action action in this.Actions)
                    action();
                this.Actions.Clear();
            }

            void IDeferredExecution.Defer(Action action)
            {
                this.Actions.Add(action);
            }
        }

        private TestDeferredExecution _dispatcher;

        [TestInitialize]
        public void SetUpTest()
        {
            _dispatcher = new TestDeferredExecution();
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _dispatcher = null;
        }

        [TestMethod]
        public void InSessionMenusModel_Disconnect_CallsSession()
        {
            using (TestSession session = new TestSession())
            {
                session.Expect("Disconnect", new List<object>() { }, null);
                IInSessionMenus model = new InSessionMenusModel(_dispatcher, session, new TestFullScreenModel());

                model.Disconnect();
            }
        }

        [TestMethod]
        public void InSessionMenusModel_EnterFullScreen_CallsFullScreenModel()
        {
            using (TestSession session = new TestSession())
            using (TestFullScreenModel fullScreenModel = new TestFullScreenModel())
            {
                fullScreenModel.Expect("EnterFullScreen", new List<object>(), null);
                IInSessionMenus model = new InSessionMenusModel(_dispatcher, session, fullScreenModel);

                model.EnterFullScreen.Execute(null);
            }
        }

        [TestMethod]
        public void InSessionMenusModel_ExitFullScreen_CallsFullScreenModel()
        {
            using (TestSession session = new TestSession())
            using (TestFullScreenModel fullScreenModel = new TestFullScreenModel())
            {
                fullScreenModel.Expect("ExitFullScreen", new List<object>(), null);
                IInSessionMenus model = new InSessionMenusModel(_dispatcher, session, fullScreenModel);

                model.ExitFullScreen.Execute(null);
            }
        }

        [TestMethod]
        public void InSessionMenusModel_EnterFullScreen_CommandsUpdated()
        {
            using (TestSession session = new TestSession())
            using (TestFullScreenModel fullScreenModel = new TestFullScreenModel())
            {
                IInSessionMenus model = new InSessionMenusModel(_dispatcher, session, fullScreenModel);
                Assert.IsFalse(fullScreenModel.IsFullScreenMode);
                fullScreenModel.SetFullScreenMode(true);
                Assert.IsFalse(model.ExitFullScreen.CanExecute(null));
                _dispatcher.ExecuteDeferred();

                Assert.IsFalse(model.EnterFullScreen.CanExecute(null));
                Assert.IsTrue(model.ExitFullScreen.CanExecute(null));
            }
        }

        [TestMethod]
        public void InSessionMenusModel_ExitFullScreen_CommandsUpdated()
        {
            using (TestSession session = new TestSession())
            using (TestFullScreenModel fullScreenModel = new TestFullScreenModel())
            {
                fullScreenModel.SetFullScreenMode(true);
                _dispatcher.ExecuteDeferred();

                IInSessionMenus model = new InSessionMenusModel(_dispatcher, session, fullScreenModel);
                Assert.IsTrue(fullScreenModel.IsFullScreenMode);
                fullScreenModel.SetFullScreenMode(false);
                Assert.IsFalse(model.EnterFullScreen.CanExecute(null));
                _dispatcher.ExecuteDeferred();

                Assert.IsTrue(model.EnterFullScreen.CanExecute(null));
                Assert.IsFalse(model.ExitFullScreen.CanExecute(null));
            }
        }
    }
}

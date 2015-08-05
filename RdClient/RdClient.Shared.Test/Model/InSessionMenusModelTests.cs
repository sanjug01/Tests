namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
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

            bool IFullScreenModel.IsFullScreenMode
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
        }

        [TestMethod]
        public void InSessionMenusModel_Disconnect_CallsSession()
        {
            using (TestSession session = new TestSession())
            {
                session.Expect("Disconnect", new List<object>() { }, null);
                IInSessionMenus model = new InSessionMenusModel(session, new TestFullScreenModel());

                model.Disconnect();
            }
        }
    }
}

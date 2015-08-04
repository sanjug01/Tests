namespace RdClient.Shared.Test.Model
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.CxWrappers;
    using RdClient.Shared.Data;
    using RdClient.Shared.Models;
    using System;
    using System.Collections.Generic;

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

        [TestMethod]
        public void InSessionMenusModel_Disconnect_CallsSession()
        {
            using (TestSession session = new TestSession())
            {
                session.Expect("Disconnect", new List<object>() { }, null);
                IInSessionMenus model = new InSessionMenusModel(session);

                model.Disconnect();
            }
        }
    }
}

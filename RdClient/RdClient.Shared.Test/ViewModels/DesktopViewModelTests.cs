using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Data;
using RdClient.Shared.Models;
using RdClient.Shared.Test.Data;
using RdClient.Shared.Test.Helpers;
using RdClient.Shared.ViewModels;
using System;
using System.Linq;
using System.Collections.Generic;
using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DesktopViewModelTests
    {
        private TestData _testData;
        private ApplicationDataModel _dataModel;
        private Mock.NavigationService _navService;
        private DesktopModel _desktop;
        private CredentialsModel _cred;
        private ISessionFactory _sessionFactory;
        private IDesktopViewModel _vm;

        private sealed class TestSessionFactory : ISessionFactory
        {
            IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
            {
                return new Session();
            }

            private sealed class Session : IRemoteSession
            {
                private EventHandler<MouseCursorShapeChangedArgs> _mouseCursorShapeChanged;

                string IRemoteSession.HostName
                {
                    get { throw new NotImplementedException(); }
                }

                IRemoteSessionState IRemoteSession.State
                {
                    get { throw new NotImplementedException(); }
                }

                ICertificateTrust IRemoteSession.CertificateTrust
                {
                    get { throw new NotImplementedException(); }
                }

                bool IRemoteSession.IsServerTrusted { get; set; }

                event EventHandler<CredentialsNeededEventArgs> IRemoteSession.CredentialsNeeded
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                event EventHandler<BadCertificateEventArgs> IRemoteSession.BadCertificate
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                event EventHandler<BadServerIdentityEventArgs> IRemoteSession.BadServerIdentity
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                event EventHandler<SessionFailureEventArgs> IRemoteSession.Failed
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                event EventHandler<SessionInterruptedEventArgs> IRemoteSession.Interrupted
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
                }

                event EventHandler IRemoteSession.Closed
                {
                    add { throw new NotImplementedException(); }
                    remove { throw new NotImplementedException(); }
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

                public event EventHandler<MouseCursorShapeChangedArgs> MouseCursorShapeChanged
                {
                    add { _mouseCursorShapeChanged += value; }
                    remove { _mouseCursorShapeChanged -= value; }
                }

                IRemoteSessionControl IRemoteSession.Activate(IRemoteSessionView sessionView)
                {
                    throw new NotImplementedException();
                }

                IRenderingPanel IRemoteSession.Deactivate()
                {
                    throw new NotImplementedException();
                }

                void IRemoteSession.Suspend()
                {
                    throw new NotImplementedException();
                }

                void IRemoteSession.Resume()
                {
                    throw new NotImplementedException();
                }

                void IRemoteSession.Disconnect()
                {
                    throw new NotImplementedException();
                }


            }
        }

        [TestInitialize]
        public void TestSetup()
        {
            _testData = new TestData();
            _dataModel = new ApplicationDataModel()
            {
                RootFolder = new MemoryStorageFolder(),
                ModelSerializer = new SerializableModelSerializer(),
                DataScrambler = new Mock.DummyDataScrambler()
            };
            _dataModel.Compose();
            _navService = new Mock.NavigationService();
            _cred = _testData.NewValidCredential().Model;
            _desktop = _testData.NewValidDesktop(_dataModel.Credentials.AddNewModel(_cred)).Model;

            _dataModel.LocalWorkspace.Connections.AddNewModel(_desktop);

            _vm = DesktopViewModel.Create(_dataModel.LocalWorkspace.Connections.Models[0], _dataModel, _navService, null);
            _sessionFactory = new TestSessionFactory();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _navService.Dispose();
            _dataModel = null;
            _sessionFactory = null;
        }

        [TestMethod]
        public void TestDesktopMatches()
        {
            Assert.AreEqual(_desktop, _vm.Desktop);
        }

        [TestMethod]
        public void TestCredentialReturnsNullIfDesktopHasNoCredential()
        {
            _desktop.CredentialsId = Guid.Empty;
            Assert.IsNull(_vm.Credentials);
        }

        [TestMethod]
        public void TestCredentialReturnsCredentialForDesktop()
        {
            Assert.AreEqual(_cred, _vm.Credentials);
        }

        [TestMethod]
        public void TestIsSelectedInitiallyFalse()
        {
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestIsSelectedSetCorrectly()
        {
            _vm.SelectionEnabled = true;
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
            _vm.IsSelected = false;
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestEditCommandExecute()
        {

            _navService.Expect("PushAccessoryView", new List<object> { "AddOrEditDesktopView", null, null }, 0);
            _vm.EditCommand.Execute(null);
        }

        [TestMethod]
        public void TestConnectCommandExecuteNavigatesToSessionViewIfCredentialsExist()
        {
            _vm.Presenting(_sessionFactory);
            _navService.Expect("NavigateToView", new List<object> { "RemoteSessionView", null }, 0);
            _vm.ConnectCommand.Execute(null);
        }

        [TestMethod]
        public void TestConnectCommandExecuteShowsAddUserViewIfNoCredentials()
        {
            _vm.Presenting(_sessionFactory);
            _vm.Desktop.CredentialsId = Guid.Empty;
            _navService.Expect("PushModalView", new List<object> { "InSessionEditCredentialsView", null, null }, 0);
            _vm.ConnectCommand.Execute(null);
        }

        [TestMethod]
        public void DeleteCommandRemovesDesktop()
        {
            var expectedDesktops = _dataModel.LocalWorkspace.Connections.Models.Where(mc => mc.Model != _desktop).ToList();
            _vm.DeleteCommand.Execute(null);
            CollectionAssert.AreEqual(expectedDesktops, _dataModel.LocalWorkspace.Connections.Models);
        }

        [TestMethod]
        public void TestSelectionEnabledInitiallyFalse()
        {
            Assert.IsFalse(_vm.SelectionEnabled);
        }

        [TestMethod]
        public void TestSelectingFailsIfSelectionIsNotEnabled()
        {
            _vm.SelectionEnabled = false;
            Assert.IsFalse(_vm.IsSelected);
            _vm.IsSelected = true;
            Assert.IsFalse(_vm.IsSelected);
        }

        [TestMethod]
        public void TestSelectingSucceedsIfSelectionIsEnabled()
        {
            _vm.SelectionEnabled = true;
            Assert.IsFalse(_vm.IsSelected);
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
        }

        [TestMethod]
        public void TestDisablingSelectionSetsSelectedToFalse()
        {
            _vm.SelectionEnabled = true;            
            _vm.IsSelected = true;
            Assert.IsTrue(_vm.IsSelected);
            _vm.SelectionEnabled = false;
            Assert.IsFalse(_vm.IsSelected);
        }
    }
}
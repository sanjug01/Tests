namespace RdClient.Shared.Test.Navigation.Extensions
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Models;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System.Collections.Generic;

    [TestClass]
    public sealed class SessionFactoryExtensionTests
    {
        private sealed class TestSessionFactory : ISessionFactory
        {
            IDeferredExecution ISessionFactory.DeferedExecution
            {
                get { return null; }
                set { }
            }

            IRemoteSession ISessionFactory.CreateSession(RemoteSessionSetup sessionSetup)
            {
                throw new System.NotImplementedException();
            }
        }

        private sealed class TestViewModel : MutableObject, IViewModel, ISessionFactorySite
        {
            private ISessionFactory _sessionFactory;

            public ISessionFactory SessionFactory
            {
                get { return _sessionFactory; }
                private set { this.SetProperty(ref _sessionFactory, value); }
            }

            void IViewModel.Presenting(INavigationService navigationService, object activationParameter, IModalPresentationContext presentationContext)
            {
                throw new System.NotImplementedException();
            }

            void IViewModel.Dismissing()
            {
                throw new System.NotImplementedException();
            }

            void ISessionFactorySite.SetSessionFactory(ISessionFactory sessionFactory)
            {
                this.SessionFactory = sessionFactory;
            }
        }

        private ISessionFactory _factory;
        private TestViewModel _vm;
        private SessionFactoryExtension _ext;

        [TestInitialize]
        public void SetUpTest()
        {
            _vm = new TestViewModel();
            _factory = new TestSessionFactory();
            _ext = new SessionFactoryExtension() { SessionFactory = _factory };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _vm = null;
            _factory = null;
            _ext = null;
        }

        [TestMethod]
        public void NewSessionFactoryExtension_CorrectFactorySet()
        {
            Assert.AreSame(_factory, _ext.SessionFactory);
        }

        [TestMethod]
        public void SessionFactoryExtension_ChangeSessionFactory_ChangeReported()
        {
            List<string> changes = new List<string>();
            _ext.PropertyChanged += (sender, e) => changes.Add(e.PropertyName);

            _ext.SessionFactory = null;

            Assert.IsNull(_ext.SessionFactory);
            Assert.AreEqual(1, changes.Count);
            CollectionAssert.Contains(changes, "SessionFactory");
        }

        [TestMethod]
        public void SessionFactoryExtension_PresentViewModel_DeferredExecutionSet()
        {
            List<string> changes = new List<string>();
            _vm.PropertyChanged += (sender, e) => changes.Add(e.PropertyName);

            _ext.CastAndCall<INavigationExtension>(ext => ext.Presenting(_vm));

            Assert.AreSame(_vm.SessionFactory, _factory);
            Assert.AreEqual(1, changes.Count);
            CollectionAssert.Contains(changes, "SessionFactory");
        }

        [TestMethod]
        public void SessionFactoryExtension_DismissViewModel_DeferredExecutionCleared()
        {
            List<string> changes = new List<string>();

            _ext.CastAndCall<INavigationExtension>(ext => ext.Presenting(_vm));
            _vm.PropertyChanged += (s, e) => changes.Add(e.PropertyName);
            _ext.CastAndCall<INavigationExtension>(ext => ext.Dismissed(_vm));

            Assert.IsNull(_vm.SessionFactory);
            Assert.AreEqual(1, changes.Count);
            CollectionAssert.Contains(changes, "SessionFactory");
        }
    }
}

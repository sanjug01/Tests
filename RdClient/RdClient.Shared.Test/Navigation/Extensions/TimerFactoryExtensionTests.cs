namespace RdClient.Shared.Test.Navigation.Extensions
{
    using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    [TestClass]
    public sealed class TimerFactoryExtensionTests
    {
        private sealed class TestTimerFactory : ITimerFactory
        {
            [DebuggerNonUserCode] // exclude from code coverage
            ITimer ITimerFactory.CreateTimer()
            {
                throw new NotImplementedException();
            }
        }

        private sealed class TestViewModel : MutableObject, IViewModel, ITimerFactorySite
        {
            private ITimerFactory _timerFactory;

            public ITimerFactory TimerFactory
            {
                get { return _timerFactory; }
                set { this.SetProperty(ref _timerFactory, value); }
            }

            [DebuggerNonUserCode] // exclude from code coverage
            public void Presenting(INavigationService navigationService, object activationParameter, IModalPresentationContext presentationResult)
            {
                throw new NotImplementedException();
            }

            [DebuggerNonUserCode] // exclude from code coverage
            public void Dismissing()
            {
                throw new NotImplementedException();
            }

            [DebuggerNonUserCode] // exclude from code coverage
            public void NavigatingBack(IBackCommandArgs backArgs)
            {
                throw new NotImplementedException();
            }

            [DebuggerNonUserCode] // exclude from code coverage
            void ITimerFactorySite.SetTimerFactory(ITimerFactory timerFactory)
            {
                this.TimerFactory = timerFactory;
            }
        }

        private TestTimerFactory _factory;
        private TimerFactoryExtension _extension;

        [TestInitialize]
        public void SetUpTest()
        {
            _factory = new TestTimerFactory();
            _extension = new TimerFactoryExtension(_factory);
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _extension = null;
            _factory = null;
        }

        [TestMethod]
        public void PresentViewModel_TimerFactorySet()
        {
            TestViewModel testVM = new TestViewModel();
            IList<string> reportedProperties = new List<string>();

            testVM.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);

            _extension.CastAndCall<INavigationExtension>(ext => ext.Presenting(testVM));
            Assert.AreSame(testVM.TimerFactory, _factory);
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.AreEqual("TimerFactory", reportedProperties[0]);
        }

        [TestMethod]
        public void DismissViewModel_TimerFactoryCleared()
        {
            TestViewModel testVM = new TestViewModel();
            IList<string> reportedProperties = new List<string>();

            _extension.CastAndCall<INavigationExtension>(ext => ext.Presenting(testVM));
            testVM.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _extension.CastAndCall<INavigationExtension>(ext => ext.Dismissed(testVM));
            Assert.IsNull(testVM.TimerFactory);
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.AreEqual("TimerFactory", reportedProperties[0]);
        }
    }
}

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
    public class DeferredExecutionExtensionTests
    {
        private sealed class TestDeferredExecution : IDeferredExecution
        {
            [DebuggerNonUserCode] // exclude from code coverage
            void IDeferredExecution.Defer(Action action)
            {
                throw new System.NotImplementedException();
            }
        }

        private sealed class TestViewModel : MutableObject, IViewModel, IDeferredExecutionSite
        {
            private IDeferredExecution _deferredExecution;

            public IDeferredExecution DeferredExecution
            {
                get { return _deferredExecution; }
                set { this.SetProperty<IDeferredExecution>(ref _deferredExecution, value); }
            }

            [DebuggerNonUserCode] // exclude from code coverage
            public void Presenting(INavigationService navigationService, object activationParameter, IStackedPresentationContext presentationResult)
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

            void IDeferredExecutionSite.SetDeferredExecution(IDeferredExecution defEx)
            {
                this.DeferredExecution = defEx;
            }
        }

        private TestDeferredExecution _defEx;
        private DeferredExecutionExtension _extension;

        [TestInitialize]
        public void SetUpTest()
        {
            _defEx = new TestDeferredExecution();
            _extension = new DeferredExecutionExtension() { DeferredExecution = _defEx };
        }

        [TestCleanup]
        public void TearDownTest()
        {
            _extension = null;
            _defEx = null;
        }

        [TestMethod]
        public void NewDeferredExecutionExtension_CorrectDeferredExecution()
        {
            Assert.AreSame(_defEx, _extension.DeferredExecution);
        }

        [TestMethod]
        public void ChangeDeferredExecution_ChangeReported()
        {
            IList<string> reportedProperties = new List<string>();

            _extension.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _extension.DeferredExecution = null;
            Assert.IsNull(_extension.DeferredExecution);
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.AreEqual("DeferredExecution", reportedProperties[0]);
        }

        [TestMethod]
        public void PresentViewModel_DeferredExecutionSet()
        {
            TestViewModel testVM = new TestViewModel();
            IList<string> reportedProperties = new List<string>();

            testVM.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);

            _extension.CastAndCall<INavigationExtension>(ext => ext.Presenting(testVM));
            Assert.AreSame(testVM.DeferredExecution, _defEx);
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.AreEqual("DeferredExecution", reportedProperties[0]);
        }

        [TestMethod]
        public void DismissViewModel_DeferredExecutionCleared()
        {
            TestViewModel testVM = new TestViewModel();
            IList<string> reportedProperties = new List<string>();

            _extension.CastAndCall<INavigationExtension>(ext => ext.Presenting(testVM));
            testVM.PropertyChanged += (s, e) => reportedProperties.Add(e.PropertyName);
            _extension.CastAndCall<INavigationExtension>(ext => ext.Dismissed(testVM));
            Assert.IsNull(testVM.DeferredExecution);
            Assert.AreEqual(1, reportedProperties.Count);
            Assert.AreEqual("DeferredExecution", reportedProperties[0]);
        }
    }
}

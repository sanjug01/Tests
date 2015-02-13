namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.Navigation.Extensions;
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;

    [TestClass]
    public sealed class DeferringViewModelBaseTests
    {
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

        private sealed class TestViewModel : DeferringViewModelBase
        {
            public void DeferAction(Action action)
            {
                this.DeferToUI(action);
            }

            protected override void OnPresenting(object activationParameter)
            {
            }
        }

        [TestMethod]
        public void SanityCheck_TestDispatcher_DeferAction_Deferred()
        {
            int callCount = 0;
            TestDeferredExecution tde = new TestDeferredExecution();
            tde.CastAndCall<IDeferredExecution>(de => de.Defer(() => ++callCount));
            Assert.AreEqual(0, callCount);
            Assert.AreEqual(1, tde.Actions.Count);
            tde.ExecuteDeferred();
            Assert.AreEqual(1, callCount);
            Assert.AreEqual(0, tde.Actions.Count);
        }
        
        [TestMethod]
        public void NewDeferringViewModel_Defer_ThrowsCorrectException()
        {
            TestViewModel vm = new TestViewModel();

            try
            {
                vm.DeferAction(() => { });
                Assert.Fail("Unexpected success");
            }
            catch (DeferredExecutionException)
            {
                // Success
            }
            catch(Exception ex)
            {
                Assert.Fail(string.Format("Unexpected exception {0}", ex));
            }
        }

        [TestMethod]
        public void NewDeferringViewModel_SetDeferredExecutionAndDefer_ActionDeferred()
        {
            TestViewModel tvm = new TestViewModel();
            TestDeferredExecution tde = new TestDeferredExecution();
            int callCount = 0;

            tvm.CastAndCall<IDeferredExecutionSite>(ds =>
            {
                ds.SetDeferredExecution(tde);
                tvm.DeferAction(() => ++callCount);
            });
            Assert.AreEqual(0, callCount);
            tde.ExecuteDeferred();
            Assert.AreEqual(1, callCount);
        }

        [TestMethod]
        public void NewDeferringViewModel_SetAndClearDeferredExecutionAndDefer_Throws()
        {
            TestViewModel tvm = new TestViewModel();
            TestDeferredExecution tde = new TestDeferredExecution();

            tvm.CastAndCall<IDeferredExecutionSite>(ds =>
            {
                ds.SetDeferredExecution(tde);
                ds.SetDeferredExecution(null);

                try
                {
                    tvm.DeferAction(() => { });
                    Assert.Fail("Unexpected success");
                }
                catch(DeferredExecutionException)
                {
                    // Success
                }
                catch(Exception ex)
                {
                    Assert.Fail(string.Format("Unexpected exception {0}", ex));
                }
            });
            Assert.AreEqual(0, tde.Actions.Count);
        }
    }
}

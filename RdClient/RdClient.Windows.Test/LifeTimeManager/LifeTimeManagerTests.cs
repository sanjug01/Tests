using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.LifeTimeManagement;
using System;
using Windows.ApplicationModel.Activation;

namespace RdClient.Windows.Test
{
    [TestClass]
    public class LifeTimeManagerTests
    {
        private Mock.RootFrameManager _rootFrameManager;
        private LifeTimeManager _lifeTimeManager;

        [TestInitialize]
        public void TestSetup()
        {
            _rootFrameManager = new Mock.RootFrameManager();
            _lifeTimeManager = new LifeTimeManager();
            _lifeTimeManager.Initialize(_rootFrameManager);
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _lifeTimeManager = null;
            _rootFrameManager = null;
        }

        [TestMethod]
        public void OnLaunched()
        {
            IActivationArgs args = new ActivationArgs("", "", ActivationKind.Launch, ApplicationExecutionState.Terminated, null as SplashScreen, 0, false, null);
            _lifeTimeManager.OnLaunched(args);

            Assert.AreEqual(1, _rootFrameManager.LoadedCount);
        }

        [TestMethod]
        public void OnSuspending()
        {
            object sender = new object();
            DateTimeOffset date = new DateTimeOffset();
            Mock.MySuspendingDeferral deferral = new Mock.MySuspendingDeferral();
            SuspensionArgs.SuspendingOperationWrapper sow = new SuspensionArgs.SuspendingOperationWrapper(date, deferral);
            SuspensionArgs sa = new SuspensionArgs(sow);

            _lifeTimeManager.OnSuspending(sender, sa);

            Assert.AreEqual(1, deferral.CompletionCount);
        }
    }
}

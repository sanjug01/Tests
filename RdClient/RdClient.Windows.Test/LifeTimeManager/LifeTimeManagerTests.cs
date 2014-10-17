using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.LifeTimeManagement;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml.Controls;

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
            IActivationArgs args = new ActivationArgs();
            _lifeTimeManager.OnLaunched(args);

            Assert.AreEqual(1, _rootFrameManager.LoadedCount);
        }

        [TestMethod]
        public void OnSuspending()
        {
            object sender = new object();
            SuspensionArgs sa = new SuspensionArgs();
            sa.SuspendingOperation = new MySuspendingOperation();

            Mock.MySuspendingDeferral deferral = new Mock.MySuspendingDeferral();

            sa.SuspendingOperation.Deferral = deferral;

            _lifeTimeManager.OnSuspending(sender, sa);

            Assert.AreEqual(1, deferral.CompletionCount);
        }
    }
}

using RdClient.Navigation;

namespace Test.RdClient.Shared
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MockViewFactoryTests
    {
        private Mock.ViewFactory _viewFactory;

        [TestInitialize]
        public void TestSetUp()
        {
            _viewFactory = new Mock.ViewFactory();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _viewFactory = null;
        }

        [TestMethod]
        public void CreateOneView_ViewCreated()
        {
            IPresentableView pv = _viewFactory.CreateView("Don Pedro", new object());
            Assert.IsNotNull(pv);
            Assert.AreEqual(1, _viewFactory.Count);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro").CreationCount);
        }

        [TestMethod]
        public void ReCreateOneView_CounterIncremented()
        {
            IPresentableView pv1 = _viewFactory.CreateView("Don Pedro", new object()), pv2 = _viewFactory.CreateView("Don Pedro", new object());
            Assert.IsNotNull(pv1);
            Assert.IsNotNull(pv2);
            Assert.AreSame(pv1, pv2);
            Assert.AreEqual(1, _viewFactory.Count);
            Assert.AreEqual(2, _viewFactory.GetView("Don Pedro").CreationCount);
        }

        [TestMethod]
        public void CreateTwoViews_ViewsCreated()
        {
            IPresentableView pv1 = _viewFactory.CreateView("Don Pedro A", new object()), pv2 = _viewFactory.CreateView("Don Pedro B", new object());
            Assert.IsNotNull(pv1);
            Assert.IsNotNull(pv2);
            Assert.AreNotSame(pv1, pv2);
            Assert.AreEqual(2, _viewFactory.Count);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro A").CreationCount);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro B").CreationCount);
        }
    }
}

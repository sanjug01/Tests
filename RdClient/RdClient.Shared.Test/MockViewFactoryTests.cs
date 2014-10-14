using FadeTest.Navigation;

namespace Test.FadeTest.Shared
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
            IPresentableView pv = _viewFactory.CreateView("Don Pedro");
            Assert.IsNotNull(pv);
            Assert.AreEqual(1, _viewFactory.Count);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro").CreationCount);
        }

        [TestMethod]
        public void ReCreateOneView_CounterIncremented()
        {
            IPresentableView pv1 = _viewFactory.CreateView("Don Pedro"), pv2 = _viewFactory.CreateView("Don Pedro");
            Assert.IsNotNull(pv1);
            Assert.IsNotNull(pv2);
            Assert.AreSame(pv1, pv2);
            Assert.AreEqual(1, _viewFactory.Count);
            Assert.AreEqual(2, _viewFactory.GetView("Don Pedro").CreationCount);
        }

        [TestMethod]
        public void CreateTwoViews_ViewsCreated()
        {
            IPresentableView pv1 = _viewFactory.CreateView("Don Pedro A"), pv2 = _viewFactory.CreateView("Don Pedro B");
            Assert.IsNotNull(pv1);
            Assert.IsNotNull(pv2);
            Assert.AreNotSame(pv1, pv2);
            Assert.AreEqual(2, _viewFactory.Count);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro A").CreationCount);
            Assert.AreEqual(1, _viewFactory.GetView("Don Pedro B").CreationCount);
        }
    }
}

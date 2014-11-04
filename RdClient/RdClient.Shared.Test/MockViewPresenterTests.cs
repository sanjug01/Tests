using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.RdClient.Shared
{


    [TestClass]
    public class MockViewPresenterTests
    {
        private Mock.ViewPresenter _viewPresenter;

        [TestInitialize]
        public void TestSetUp()
        {
            _viewPresenter = new Mock.ViewPresenter();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _viewPresenter = null;
        }


    }
}

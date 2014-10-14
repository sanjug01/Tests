using FadeTest.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.FadeTest.Shared
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

        [TestMethod]
        public void PresentOneView_ViewPresented()
        {
            Mock.PresentableView view = new Mock.PresentableView("Don Pedro");
            _viewPresenter.PresentView(view, null, null);
            Assert.AreEqual(1, view.ActivationsCount);
            Assert.AreEqual(1, view.PresentationsCount);
            Assert.AreEqual(0, view.DismissalsCount);
        }

        [TestMethod]
        public void PresentOneViewWithParameter_ParameterPassed()
        {
            Mock.PresentableView view = new Mock.PresentableView("Don Pedro");
            object param = new object();
            _viewPresenter.PresentView(view, null, param);
            Assert.AreSame(param, view.LastActivationParameter);
        }

        [TestMethod]
        public void PresentTwoViews_ViewsPresented()
        {
            Mock.PresentableView view1 = new Mock.PresentableView("Don Pedro"), view2 = new Mock.PresentableView("Don Miguel");
            _viewPresenter.PresentView(view1, null, null);
            _viewPresenter.PresentView(view2, null, null);
            Assert.AreEqual(1, view1.ActivationsCount);
            Assert.AreEqual(1, view1.PresentationsCount);
            Assert.AreEqual(1, view1.DismissalsCount);
            Assert.AreEqual(1, view2.ActivationsCount);
            Assert.AreEqual(1, view2.PresentationsCount);
            Assert.AreEqual(0, view2.DismissalsCount);
        }

        [TestMethod]
        public void RePresentView_ViewRePresented()
        {
            Mock.PresentableView view1 = new Mock.PresentableView("Don Pedro"), view2 = new Mock.PresentableView("Don Miguel");
            object param1 = new object(), param2 = new object();
            _viewPresenter.PresentView(view1, null, param1);
            _viewPresenter.PresentView(view2, null, null);
            _viewPresenter.PresentView(view1, null, param2);
            Assert.AreEqual(2, view1.ActivationsCount);
            Assert.AreEqual(2, view1.PresentationsCount);
            Assert.AreEqual(1, view1.DismissalsCount);
            Assert.AreEqual(1, view2.ActivationsCount);
            Assert.AreEqual(1, view2.PresentationsCount);
            Assert.AreEqual(1, view2.DismissalsCount);
            Assert.AreSame(param2, view1.LastActivationParameter);
        }

        [TestMethod]
        public void PresentViewWithNewParameter_OnlyNewActivation()
        {
            Mock.PresentableView view = new Mock.PresentableView("Don Pedro");
            object param1 = new object(), param2 = new object();
            _viewPresenter.PresentView(view, null, param1);
            Assert.AreSame(param1, view.LastActivationParameter);
            _viewPresenter.PresentView(view, null, param2);
            Assert.AreEqual(2, view.ActivationsCount);
            Assert.AreEqual(1, view.PresentationsCount);
            Assert.AreEqual(0, view.DismissalsCount);
            Assert.AreSame(param2, view.LastActivationParameter);
        }
    }
}

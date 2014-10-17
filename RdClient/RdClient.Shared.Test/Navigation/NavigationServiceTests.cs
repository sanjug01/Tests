using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Navigation;

namespace Test.RdClient.Shared.Test
{
    [TestClass]
    public class NavigationServiceTests
    {
        private Mock.ViewPresenter _presenter;
        private Mock.ViewFactory _viewFactory;
        private INavigationService _navigationService;

        [TestInitialize]
        public void TestSetUp()
        {
            _presenter = new Mock.ViewPresenter();
            _viewFactory = new Mock.ViewFactory();
            _navigationService = new NavigationService(_presenter, _viewFactory);
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _presenter = null;
            _viewFactory = null;
            _navigationService = null;
        }

        [TestMethod]
        public void NavigateToView_Success()
        {
            Int32 narf = 3;

            _navigationService.NavigateToView("foo", narf);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(3, fooView.LastActivationParameter);

            _navigationService.NavigateToView("bar", narf);
            Mock.PresentableView barView = _viewFactory.GetView("foo");

            Assert.AreEqual(1, fooView.DismissalsCount);
            Assert.AreEqual(1, barView.ActivationsCount);
            Assert.AreEqual(3, barView.LastActivationParameter);
        }

        class InvalidFactory : Mock.ViewFactory
        {
            public override IPresentableView CreateView(string name, object activationParameter)
            {
                return null;
            }
        }

        [TestMethod]
        public void NavigateToInvalid()
        {
            bool exceptionThrown = false;

            _navigationService = new NavigationService(_presenter, new InvalidFactory());

            try
            {
                _navigationService.NavigateToView("foo", null);
            }
            catch(NavigationServiceException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void PresentOneView_ViewPresented()
        {
            _navigationService.NavigateToView("foo", null);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(1, fooView.PresentationsCount);
            Assert.AreEqual(0, fooView.DismissalsCount);
        }

        [TestMethod]
        public void PresentOneViewWithParameter_ParameterPassed()
        {
            object param = new object();

            _navigationService.NavigateToView("foo", param);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreSame(param, fooView.LastActivationParameter);
        }

        [TestMethod]
        public void PresentTwoViews_ViewsPresented()
        {
            _navigationService.NavigateToView("foo", null);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");
            Assert.AreEqual(1, _presenter.PresentViewCount);

            _navigationService.NavigateToView("bar", null);
            Mock.PresentableView barView = _viewFactory.GetView("bar");
            Assert.AreEqual(2, _presenter.PresentViewCount);

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(1, fooView.PresentationsCount);
            Assert.AreEqual(1, fooView.DismissalsCount);

            Assert.AreEqual(1, barView.ActivationsCount);
            Assert.AreEqual(1, barView.PresentationsCount);
            Assert.AreEqual(0, barView.DismissalsCount);
        }

        [TestMethod]
        public void RePresentView_ViewRePresented()
        {
            Int32 param1 = 3;
            Int32 param2 = 4;

            _navigationService.NavigateToView("foo", param1);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            _navigationService.NavigateToView("bar", null);
            Mock.PresentableView barView = _viewFactory.GetView("bar");

            _navigationService.NavigateToView("foo", param2);

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(2, fooView.PresentationsCount);
            Assert.AreEqual(1, fooView.DismissalsCount);
            Assert.AreEqual(1, barView.ActivationsCount);
            Assert.AreEqual(1, barView.PresentationsCount);
            Assert.AreEqual(1, barView.DismissalsCount);
            Assert.AreEqual(param2, fooView.LastActivationParameter);
        }

        [TestMethod]
        public void PresentViewWithNewParameter_OnlyNewActivation()
        {
            Int32 param1 = 3;
            Int32 param2 = 4;

            _navigationService.NavigateToView("foo", param1);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreEqual(param1, fooView.LastActivationParameter);

            _navigationService.NavigateToView("foo", param2);

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(2, fooView.PresentationsCount);
            Assert.AreEqual(0, fooView.DismissalsCount);
            Assert.AreEqual(param2, fooView.LastActivationParameter);
        }

        [TestMethod]
        public void PushModalView_Presenting()
        {
            Int32 param1 = 3;

            _navigationService.PushModalView("foo", param1);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(1, fooView.PresentationsCount);
            Assert.AreEqual(0, fooView.DismissalsCount);
            Assert.AreEqual(param1, fooView.LastActivationParameter);
        }

        [TestMethod]
        public void PushDismissModalView()
        {
            Int32 param1 = 3;

            _navigationService.PushModalView("foo", param1);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            Assert.AreEqual(param1, fooView.LastActivationParameter);

            _navigationService.DismissModalView(fooView);

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(1, fooView.PresentationsCount);
            Assert.AreEqual(1, fooView.DismissalsCount);
        }

        [TestMethod]
        public void PushDismissModalViewStack()
        {
            _navigationService.PushModalView("foo", null);
            Mock.PresentableView fooView = _viewFactory.GetView("foo");

            _navigationService.PushModalView("bar", null);
            Mock.PresentableView barView = _viewFactory.GetView("bar");

            _navigationService.PushModalView("narf", null);
            Mock.PresentableView narfView = _viewFactory.GetView("narf");

            _navigationService.DismissModalView(fooView);

            Assert.AreEqual(1, fooView.ActivationsCount);
            Assert.AreEqual(1, fooView.PresentationsCount);
            Assert.AreEqual(1, fooView.DismissalsCount);

            Assert.AreEqual(1, barView.ActivationsCount);
            Assert.AreEqual(1, barView.PresentationsCount);
            Assert.AreEqual(1, barView.DismissalsCount);

            Assert.AreEqual(1, narfView.ActivationsCount);
            Assert.AreEqual(1, narfView.PresentationsCount);
            Assert.AreEqual(1, narfView.DismissalsCount);
        }

        [TestMethod]
        public void PushModalViewStack_DoubleDismiss()
        {
            bool exceptionThrown = false;

            try
            {
                _navigationService.PushModalView("foo", null);
                Mock.PresentableView fooView = _viewFactory.GetView("foo");

                _navigationService.DismissModalView(fooView);
                _navigationService.DismissModalView(fooView);
            }
            catch (NavigationServiceException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }
    }
}

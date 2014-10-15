using RdClient.Navigation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Test.RdClient.Shared.Navigation
{

    [TestClass]
    public class PresentableViewFactoryTests
    {
        private PresentableViewFactory _factory;

        [TestInitialize]
        public void TestSetup()
        {
            _factory = PresentableViewFactory.Create();
        }

        [TestCleanup]
        public void TestTearDown()
        {
            _factory = null;
        }

        [TestMethod]
        public void EmptyFactory_CreateView_Fails()
        {
            bool exceptionThrown = false;
            try
            {
                IPresentableView view = _factory.CreateView("Don Pedro", new object());
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        [TestMethod]
        public void AddClass_CreateView_ViewCreated()
        {
            _factory.AddViewClass("Don Pedro", typeof(Mock.PresentableView), false);
            IPresentableView view = _factory.CreateView("Don Pedro", new object());
            Assert.IsNotNull(view);
        }

        [TestMethod]
        public void AddClass_Create2Views_2ViewsCreated()
        {
            _factory.AddViewClass("Don Pedro", typeof(Mock.PresentableView), false);
            IPresentableView view1 = _factory.CreateView("Don Pedro", new object()), view2 = _factory.CreateView("Don Pedro", new object());
            Assert.IsNotNull(view1);
            Assert.IsNotNull(view2);
            Assert.AreNotSame(view1, view2);
        }

        [TestMethod]
        public void AddSingletonClass_Create2Views_SingletonCreated()
        {
            _factory.AddViewClass("Don Pedro", typeof(Mock.PresentableView), true);
            IPresentableView view1 = _factory.CreateView("Don Pedro", new object()), view2 = _factory.CreateView("Don Pedro", new object());
            Assert.IsNotNull(view1);
            Assert.IsNotNull(view2);
            Assert.AreSame(view1, view2);
        }
    }
}

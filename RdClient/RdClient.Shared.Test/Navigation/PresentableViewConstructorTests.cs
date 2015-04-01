using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Navigation;

namespace RdClient.Shared.Test.Navigation
{
    [TestClass]
    public class PresentableViewConstructorTests
    {
        [TestMethod]
        public void AddClass_Create2Views_2ViewsCreated()
        {
            PresentableViewConstructor pvc = new PresentableViewConstructor();
            IPresentableView view1;
            IPresentableView view2;

            pvc.Initialize(typeof(Mock.PresentableView), false);

            view1 = pvc.CreateView();
            view2 = pvc.CreateView();

            Assert.AreNotSame(view1, view2);
        }

        [TestMethod]
        public void AddSingletonClass_Create2Views_SingletonCreated()
        {
            PresentableViewConstructor pvc = new PresentableViewConstructor();
            IPresentableView view1;
            IPresentableView view2;

            pvc.Initialize(typeof(Mock.PresentableView), true);

            view1 = pvc.CreateView();
            view2 = pvc.CreateView();

            Assert.AreSame(view1, view2);
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Navigation;
using RdClient.Shared.Navigation;
using System.Collections.Generic;

namespace RdClient.Shared.Test
{

    [TestClass]
    public class PresentableViewFactoryTests
    {
        [TestMethod]
        public void EmptyFactory_CreateView_Fails()
        {
            PresentableViewFactory<Mock.PresentableViewConstructor> factory = new PresentableViewFactory<Mock.PresentableViewConstructor>(); ;

            bool exceptionThrown = false;
            try
            {
                IPresentableView view = factory.CreateView("Don Pedro", new object());
            }
            catch(KeyNotFoundException /* e */)
            {
                exceptionThrown = true;
            }

            Assert.IsTrue(exceptionThrown);
        }

        private class DummyPresentableView : IPresentableView
        {
            public void Activating(object activationParameter)
            {
            }

            public void Presenting(INavigationService navigationService, object activationParameter)
            {
            }

            public void Dismissing()
            {
            }
        }

        [TestMethod]
        public void AddClass_CreateView_ViewCreated()
        {
            PresentableViewFactory<PresentableViewConstructor> factory = new PresentableViewFactory<PresentableViewConstructor>(); ;

            bool isSingleton = false;
            object activationParameter = new object();

            factory.AddViewClass("Don Pedro", typeof(DummyPresentableView), isSingleton);
            IPresentableView view = factory.CreateView("Don Pedro", activationParameter);
            Assert.IsNotNull(view);
        }
        
        [TestMethod]
        public void AddClass_CreateView_ViewCreated_Observed()
        {
            PresentableViewFactory<PresentableViewConstructor> factory = new PresentableViewFactory<PresentableViewConstructor>(); ;

            using(Mock.PresentableViewConstructor pvc = new Mock.PresentableViewConstructor())
            using(Mock.PresentableView pv = new Mock.PresentableView())
            {
                bool isSingleton = false;
                object activationParameter = new object();

                pvc.Expect("Initialize", new List<object>() { null, isSingleton }, 0);
                pvc.Expect("CreateView", new List<object>() { }, pv);
                pv.Expect("Activating", new List<object>() { activationParameter }, 0);

                factory.AddViewClass("Don Pedro", typeof(Mock.PresentableView), pvc, isSingleton);
                IPresentableView view = factory.CreateView("Don Pedro", activationParameter);
                Assert.IsNotNull(view);
            }
        }
    }
}

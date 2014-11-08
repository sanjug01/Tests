using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.ViewModels;

namespace Test.RdClient.Shared.Test
{
    [TestClass]
    public class ViewModelBaseTests
    {
        class DummyViewModel : ViewModelBase
        { 
            private bool _someProperty = false;

            protected override void OnPresenting(object activationParameter)
            {

            }

            public bool SomeProperty
            {
                get { return _someProperty; }
                set { SetProperty(ref _someProperty, value, "SomeProperty"); }
            }
        }

        [TestMethod]
        public void ViewModelBaseTest_Success()
        {
            DummyViewModel dvm = new DummyViewModel();
            string changedProperty = null;

            dvm.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            dvm.SomeProperty = true;

            Assert.AreEqual("SomeProperty", changedProperty);
            Assert.AreEqual(true, dvm.SomeProperty);
        }

        [TestMethod]
        public void ViewModelBaseTest_WriteSameValue()
        {
            DummyViewModel dvm = new DummyViewModel();
            string changedProperty = null;


            dvm.PropertyChanged += (s, e) => changedProperty = e.PropertyName;

            dvm.SomeProperty = false;
            Assert.AreEqual(null, changedProperty);
        }
    }
}

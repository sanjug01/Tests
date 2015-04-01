using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Navigation;
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

        [TestMethod]
        public void OnNavigatingBackDoesNotSetHandled()
        {
            DummyViewModel dvm = new DummyViewModel();
            IBackCommandArgs backAgs = new BackCommandArgs();
            IViewModel dvmAsViewModel = dvm as IViewModel;            
            Assert.IsNotNull(dvmAsViewModel, "test precondition failed");
            Assert.IsFalse(backAgs.Handled, "test precondition failed");
            
            dvmAsViewModel.NavigatingBack(backAgs);
            Assert.IsFalse(backAgs.Handled);

            backAgs.Handled = true;
            dvmAsViewModel.NavigatingBack(backAgs);
            Assert.IsTrue(backAgs.Handled);
        }
    }
}

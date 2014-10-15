using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.ViewModels;
using System.ComponentModel;

namespace Test.RdClient.Shared.Test
{
    [TestClass]
    public class ViewModelBaseTests
    {
        class DummyViewModel : ViewModelBase
        { 
            private bool _someProperty = false;

            public bool ChangedEventFired = false;
            public PropertyChangedEventArgs ChangedEventArgs;

            public bool SomeProperty
            {
                get { return _someProperty; }
                set { SetProperty(ref _someProperty, value, "SomeProperty"); }
            }

            public static void c_SomePropertyChanged(object sender, EventArgs e)
            {
                DummyViewModel dvm = (DummyViewModel) sender;
                PropertyChangedEventArgs args = (PropertyChangedEventArgs)e;

                dvm.ChangedEventFired = true;
                dvm.ChangedEventArgs = args;
            }
        }

        [TestMethod]
        public void ViewModelBaseTest_Success()
        {
            DummyViewModel dvm = new DummyViewModel();

            dvm.PropertyChanged += DummyViewModel.c_SomePropertyChanged;

            dvm.SomeProperty = true;

            Assert.IsTrue(dvm.ChangedEventFired);
            Assert.AreEqual("SomeProperty", dvm.ChangedEventArgs.PropertyName);
            Assert.AreEqual(true, dvm.SomeProperty);
        }

        [TestMethod]
        public void ViewModelBaseTest_WriteSameValue()
        {
            DummyViewModel dvm = new DummyViewModel();

            dvm.PropertyChanged += DummyViewModel.c_SomePropertyChanged;

            dvm.SomeProperty = false;

            Assert.IsFalse(dvm.ChangedEventFired);
            Assert.AreEqual(null, dvm.ChangedEventArgs);
        }
    }
}

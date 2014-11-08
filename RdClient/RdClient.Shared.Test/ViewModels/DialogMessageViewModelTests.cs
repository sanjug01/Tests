using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class DialogMessageViewModelTests
    {
        [TestMethod]
        public void DialogMessagePresent()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.PresentableView view = new Mock.PresentableView())
            {
                DialogMessageArgs dma = new DialogMessageArgs("test", null, null);
                DialogMessageViewModel dmvm = new DialogMessageViewModel();

                int propertyChangedCount = 0;

                dmvm.PropertyChanged += (sender, args) => { propertyChangedCount++; };
                dmvm.DialogView = view;
                dmvm.Presenting(navigation, dma);

                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.AreEqual("test", dmvm.Message);
                Assert.AreEqual(3, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message
            }
        }

        [TestMethod]
        public void DialogMessageOkCalledNoCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                bool okCalled = false;
                DialogMessageArgs dma = new DialogMessageArgs("test", () => { okCalled = true; }, null);
                DialogMessageViewModel dmvm = new DialogMessageViewModel();

                int propertyChangedCount = 0;

                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                dmvm.PropertyChanged += (sender, args) => { propertyChangedCount++; };
                dmvm.DialogView = view;
                dmvm.Presenting(navigation, dma);
                dmvm.OkCommand.Execute(null);

                Assert.IsTrue(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.IsTrue(okCalled);
                Assert.AreEqual(3, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message
            }
        }

        [TestMethod]
        public void DialogMessageCancelCalledNoOk()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                bool cancelCalled = false;
                DialogMessageArgs dma = new DialogMessageArgs("test", null, () => { cancelCalled = true; } );
                DialogMessageViewModel dmvm = new DialogMessageViewModel();

                int propertyChangedCount = 0;

                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                dmvm.PropertyChanged += (sender, args) => { propertyChangedCount++; };
                dmvm.DialogView = view;
                dmvm.Presenting(navigation, dma);
                dmvm.CancelCommand.Execute(null);

                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsTrue(dmvm.CancelVisible);
                Assert.IsTrue(cancelCalled);
                Assert.AreEqual(3, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message
            }
        }
    }
}

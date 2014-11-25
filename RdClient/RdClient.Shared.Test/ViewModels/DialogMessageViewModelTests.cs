namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    [TestClass]
    public class DialogMessageViewModelTests
    {
        [TestMethod]
        public void DialogMessagePresent_2Delegates_AllProperttiesReported()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.PresentableView view = new Mock.PresentableView())
            {
                DialogMessageArgs dma = new DialogMessageArgs("test", () => { }, () => { });
                DialogMessageViewModel dmvm = new DialogMessageViewModel();

                IList<string> reportedChanges = new List<string>();

                dmvm.PropertyChanged += (sender, args) => reportedChanges.Add(args.PropertyName);
                dmvm.DialogView = view;
                //
                // TODO:    REFACTOR THIS!
                //          Make the navigation service present "dmvm"
                //
                ((IViewModel)dmvm).Presenting(navigation, dma, null);

                Assert.IsTrue(dmvm.OkVisible);
                Assert.IsTrue(dmvm.CancelVisible);
                Assert.AreEqual("test", dmvm.Message);
                Assert.AreEqual(5, reportedChanges.Count); // NavigationService, Message, Title, OkVisible, and CancelVisible
                Assert.IsTrue(reportedChanges.Contains("Message"));
                Assert.IsTrue(reportedChanges.Contains("Title"));
                Assert.IsTrue(reportedChanges.Contains("NavigationService"));
                Assert.IsTrue(reportedChanges.Contains("OkVisible"));
                Assert.IsTrue(reportedChanges.Contains("CancelVisible"));
            }
        }

        [TestMethod]
        public void DialogMessagePresent_NoDelegates_VisibilityProperttiesNotReported()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                DialogMessageArgs dma = new DialogMessageArgs("test", null, null);
                DialogMessageViewModel dmvm = new DialogMessageViewModel();

                IList<string> reportedChanges = new List<string>();
                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);

                dmvm.PropertyChanged += (sender, args) => reportedChanges.Add(args.PropertyName);
                dmvm.DialogView = view;
                //
                // TODO:    REFACTOR THIS!
                //          Make the navigation service present "dmvm"
                //
                ((IViewModel)dmvm).Presenting(navigation, dma, null);

                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.AreEqual("test", dmvm.Message);
                Assert.AreEqual(3, reportedChanges.Count); // NavigationService, Message, Title
                Assert.IsTrue(reportedChanges.Contains("Message"));
                Assert.IsTrue(reportedChanges.Contains("Title"));
                Assert.IsTrue(reportedChanges.Contains("NavigationService"));
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
                //
                // TODO:    REFACTOR THIS!
                //          Make the navigation service present "dmvm"
                //
                ((IViewModel)dmvm).Presenting(navigation, dma, null);
                dmvm.OkCommand.Execute(null);

                Assert.IsTrue(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.IsTrue(okCalled);
                Assert.AreEqual(4, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message, one for default title
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
                //
                // TODO:    REFACTOR THIS!
                //          Make the navigation service present "dmvm"
                //
                ((IViewModel)dmvm).Presenting(navigation, dma, null);
                dmvm.CancelCommand.Execute(null);

                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsTrue(dmvm.CancelVisible);
                Assert.IsTrue(cancelCalled);
                Assert.AreEqual(4, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message, one for default title
            }
        }
    }
}

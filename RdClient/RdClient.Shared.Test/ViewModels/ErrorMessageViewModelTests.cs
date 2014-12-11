﻿namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.CxWrappers.Errors;
    using RdClient.Shared.Navigation;
    using RdClient.Shared.ViewModels;
    using System.Collections.Generic;

    class RdpError : IRdpError
    {
        public string Category
        {
            get { return "RdpError"; }
        }
    }

    [TestClass]
    public class ErrorMessageViewModelTests
    {
        [TestMethod]
        public void ErrorMessagePresent_2Delegates_AllProperttiesReported()
        {
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            using(Mock.PresentableView view = new Mock.PresentableView())
            {
                RdpError error = new RdpError();
                ErrorMessageArgs dma = new ErrorMessageArgs(error, () => { }, () => { });
                ErrorMessageViewModel dmvm = new ErrorMessageViewModel();

                IList<string> reportedChanges = new List<string>();

                dmvm.PropertyChanged += (sender, args) => reportedChanges.Add(args.PropertyName);
                dmvm.DialogView = view;

                ((IViewModel)dmvm).Presenting(navigation, dma, null);

                Assert.IsTrue(dmvm.OkVisible);
                Assert.IsTrue(dmvm.CancelVisible);
                Assert.AreEqual(error, dmvm.Error);
                Assert.AreEqual(5, reportedChanges.Count); // NavigationService, Message, Title, OkVisible, and CancelVisible
                Assert.IsTrue(reportedChanges.Contains("Error"));
                Assert.IsTrue(reportedChanges.Contains("Title"));
                Assert.IsTrue(reportedChanges.Contains("NavigationService"));
                Assert.IsTrue(reportedChanges.Contains("OkVisible"));
                Assert.IsTrue(reportedChanges.Contains("CancelVisible"));
            }
        }

        [TestMethod]
        public void ErrorMessagePresent_NoDelegates_VisibilityProperttiesNotReported()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                RdpError error = new RdpError();
                ErrorMessageArgs dma = new ErrorMessageArgs(error, null, null);
                ErrorMessageViewModel dmvm = new ErrorMessageViewModel();

                IList<string> reportedChanges = new List<string>();
                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);

                dmvm.PropertyChanged += (sender, args) => reportedChanges.Add(args.PropertyName);
                dmvm.DialogView = view;

                ((IViewModel)dmvm).Presenting(navigation, dma, null);

                Assert.IsFalse(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.AreEqual(error, dmvm.Error);
                Assert.AreEqual(3, reportedChanges.Count); // NavigationService, Message, Title
                Assert.IsTrue(reportedChanges.Contains("Error"));
                Assert.IsTrue(reportedChanges.Contains("Title"));
                Assert.IsTrue(reportedChanges.Contains("NavigationService"));
            }
        }

        [TestMethod]
        public void ErrorMessageOkCalledNoCancel()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                bool okCalled = false;
                RdpError error = new RdpError();
                ErrorMessageArgs dma = new ErrorMessageArgs(error, () => { okCalled = true; }, null);
                ErrorMessageViewModel dmvm = new ErrorMessageViewModel();

                int propertyChangedCount = 0;

                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                dmvm.PropertyChanged += (sender, args) => { propertyChangedCount++; };
                dmvm.DialogView = view;

                ((IViewModel)dmvm).Presenting(navigation, dma, null);
                dmvm.OkCommand.Execute(null);

                Assert.IsTrue(dmvm.OkVisible);
                Assert.IsFalse(dmvm.CancelVisible);
                Assert.IsTrue(okCalled);
                Assert.AreEqual(4, propertyChangedCount); // one for OkVisible, one for CancelVisible, one for setting the message, one for default title
            }
        }

        [TestMethod]
        public void ErrorMessageCancelCalledNoOk()
        {
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            using (Mock.PresentableView view = new Mock.PresentableView())
            {
                bool cancelCalled = false;
                RdpError error = new RdpError();
                ErrorMessageArgs dma = new ErrorMessageArgs(error, null, () => { cancelCalled = true; } );
                ErrorMessageViewModel dmvm = new ErrorMessageViewModel();

                int propertyChangedCount = 0;

                navigation.Expect("DismissModalView", new List<object> { view }, 0);

                dmvm.PropertyChanged += (sender, args) => { propertyChangedCount++; };
                dmvm.DialogView = view;

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
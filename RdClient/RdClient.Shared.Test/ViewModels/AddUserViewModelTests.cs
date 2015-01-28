using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class AddUserViewModelTests
    {
        [TestMethod]
        public void AddUserViewModel_ShouldSetStoreCredentials()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.StoreCredentials = true;
            Assert.IsTrue(auvm.StoreCredentials);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldUserNameValid()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = "Don Pedro";
            Assert.IsTrue(auvm.IsUsernameValid);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldUserNameInvalid()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = "Don Pedro>";
            Assert.IsFalse(auvm.IsUsernameValid);
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteTrue()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = "Don Pedro";
            auvm.Password = "secret";
            Assert.IsTrue(auvm.OkCommand.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse1()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = null;
            auvm.Password = "secret";
            Assert.IsFalse(auvm.OkCommand.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse2()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = "Don Pedro";
            auvm.Password = null;
            Assert.IsFalse(auvm.OkCommand.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_ShouldOkCanExecuteFalse3()
        {
            AddUserViewModel auvm = new AddUserViewModel();

            auvm.User = "Don Pedro";
            auvm.Password = "";
            Assert.IsFalse(auvm.OkCommand.CanExecute(null));
        }

        [TestMethod]
        public void AddUserViewModel_NullOkHandlerShouldntCrash()
        {
            AddUserViewModel auvm = new AddUserViewModel();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddUserViewArgs args = new AddUserViewArgs(null, null, true);
                ((IViewModel)auvm).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);

                auvm.OkCommand.Execute(null);
            }
        }

        [TestMethod]
        public void AddUserViewModel_ShouldCallOkHandler()
        {
            AddUserViewModel auvm = new AddUserViewModel();
            using(Mock.NavigationService navigation = new Mock.NavigationService())
            {
                bool handlerCalled = false;
                bool cancelledHandlerCalled = false;
                AddUserViewResultHandler handler = (Credentials credentials, bool store) => { handlerCalled = true; };
                AddUserViewCancelledHandler cancelledHandler = (Credentials credentials) => { cancelledHandlerCalled = true; };
                AddUserViewArgs args = new AddUserViewArgs(handler, cancelledHandler, true);
                ((IViewModel)auvm).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);

                auvm.OkCommand.Execute(null);

                Assert.IsTrue(handlerCalled);
                Assert.IsFalse(cancelledHandlerCalled);
            }
        }

        [TestMethod]
        public void AddUserViewModel_NullCancelHandlerShouldntCrash()
        {
            AddUserViewModel auvm = new AddUserViewModel();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                AddUserViewArgs args = new AddUserViewArgs(null, null, true);
                ((IViewModel)auvm).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);

                auvm.CancelCommand.Execute(null);
            }
        }

        [TestMethod]
        public void AddUserViewModel_ShouldCallCancelHandler()
        {
            AddUserViewModel auvm = new AddUserViewModel();
            using (Mock.NavigationService navigation = new Mock.NavigationService())
            {
                bool handlerCalled = false;
                bool cancelledHandlerCalled = false;
                AddUserViewResultHandler handler = (Credentials credentials, bool store) => { handlerCalled = true; };
                AddUserViewCancelledHandler cancelledHandler = (Credentials credentials) => { cancelledHandlerCalled = true; };
                AddUserViewArgs args = new AddUserViewArgs(handler, cancelledHandler, true);
                ((IViewModel)auvm).Presenting(navigation, args, null);

                navigation.Expect("DismissModalView", new List<object> { null }, null);

                auvm.CancelCommand.Execute(null);

                Assert.IsFalse(handlerCalled);
                Assert.IsTrue(cancelledHandlerCalled);
            }
        }
    }
}

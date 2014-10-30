using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.ViewModels;
using System.ComponentModel;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void TestBasicExecute()
        {
            bool actionCalled = false;
            Int32 actionParam = 0;

            Action<object> action = (o) => { actionCalled = true; actionParam = (Int32) o; };
            RelayCommand rc = new RelayCommand(action);

            rc.Execute(3);

            Assert.IsTrue(actionCalled);
            Assert.AreEqual(3, actionParam);
        }

        [TestMethod]
        public void TestCanExecute()
        {
            bool canExecute = false;
            bool predicateCalled = false;

            Action<object> action = (o) => {  };
            Predicate<object> predicate = (o) => { predicateCalled = true; return ((Int32)o).Equals(3); };
            RelayCommand rc = new RelayCommand(action, predicate);

            canExecute = rc.CanExecute(3);

            Assert.IsTrue(predicateCalled);
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void TestCanExecuteNil()
        {
            bool canExecute = false;

            Action<object> action = (o) => { };
            RelayCommand rc = new RelayCommand(action, null);

            canExecute = rc.CanExecute(3);

            Assert.IsTrue(canExecute);
        }
    }
}

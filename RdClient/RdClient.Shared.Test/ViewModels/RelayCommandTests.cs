using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.ViewModels;
using System;
using System.Collections.Generic;

namespace RdClient.Shared.Test.ViewModels
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void BasicExecute()
        {
            bool actionCalled = false;
            Int32 actionParam = 0;

            RelayCommand rc = new RelayCommand(o => { actionCalled = true; actionParam = (Int32)o; });

            rc.Execute(3);

            Assert.IsTrue(actionCalled);
            Assert.AreEqual(3, actionParam);
        }

        [TestMethod]
        public void CanExecute()
        {
            bool canExecute = false;
            bool predicateCalled = false;

            Predicate<object> predicate = (o) => { predicateCalled = true; return ((Int32)o).Equals(3); };
            RelayCommand rc = new RelayCommand(o => { }, predicate);

            canExecute = rc.CanExecute(3);

            Assert.IsTrue(predicateCalled);
            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void CanExecuteNil()
        {
            bool canExecute = false;

            RelayCommand rc = new RelayCommand(o => { }, null);

            canExecute = rc.CanExecute(3);

            Assert.IsTrue(canExecute);
        }

        [TestMethod]
        public void CanExecuteChangedEmitted()
        {
            bool canExecuteChanged = false;
            RelayCommand rc = new RelayCommand(o => { }, null);

            rc.EmitCanExecuteChanged();
            Assert.IsFalse(canExecuteChanged);

            rc.CanExecuteChanged += (sender, args) => canExecuteChanged = true;

            rc.EmitCanExecuteChanged();

            Assert.IsTrue(canExecuteChanged);
        }

        [TestMethod]
        public void NewRelayCommand_DisposeAndExecute_Throws()
        {
            RelayCommand rc = new RelayCommand(o => { });

            rc.Dispose();

            try
            {
                rc.Execute(null);
                Assert.Fail("Unexpected success");
            }
            catch (ObjectDisposedException)
            {
                // Success
            }
            catch
            {
                Assert.Fail("Unexpected exception");
            }
        }

        [TestMethod]
        public void NewRelayCommand_DisposeAndSubscribe_Throws()
        {
            RelayCommand rc = new RelayCommand(o => { });

            rc.Dispose();

            try
            {
                rc.CanExecuteChanged += (s, e) => { };
                Assert.Fail("Unexpected success");
            }
            catch (ObjectDisposedException)
            {
                // Success
            }
            catch
            {
                Assert.Fail("Unexpected exception");
            }
        }

        [TestMethod]
        public void NewRelayCommand_SubscribeDisposeAndUnsubscribe_Throws()
        {
            RelayCommand rc = new RelayCommand(o => { });
            EventHandler handler = (s, e) => { };

            rc.CanExecuteChanged += handler;
            rc.Dispose();

            try
            {
                rc.CanExecuteChanged -= handler;
                Assert.Fail("Unexpected success");
            }
            catch (ObjectDisposedException)
            {
                // Success
            }
            catch
            {
                Assert.Fail("Unexpected exception");
            }
        }

        [TestMethod]
        public void NewRelayCommand_SubscribeUnsubscribe_StopsEvents()
        {
            IList<EventArgs> reported = new List<EventArgs>();
            RelayCommand rc = new RelayCommand(o => { });
            EventHandler handler = (s, e) => reported.Add(e);

            rc.CanExecuteChanged += handler;
            rc.EmitCanExecuteChanged();
            Assert.AreEqual(1, reported.Count);
            rc.CanExecuteChanged -= handler;
            rc.EmitCanExecuteChanged();
            Assert.AreEqual(1, reported.Count);
        }
    }
}

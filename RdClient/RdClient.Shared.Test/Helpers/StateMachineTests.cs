using Microsoft.VisualStudio.TestTools.UnitTesting;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]
    public class StateMachineTests
    {
        [TestMethod]
        public void StateMachine_ShouldTransition()
        {
            StateMachine sm = new StateMachine();
            bool actionCalled = false;

            sm.AddTransition("narf", "zod", (o) => { return true; }, (o) => { actionCalled = true; });
            sm.SetStart("narf");
            sm.DoTransition("whatever");

            Assert.IsTrue(actionCalled);
        }

        [TestMethod]
        public void StateMachine_ShouldSetParam()
        {
            StateMachine sm = new StateMachine();
            bool actionCalled = false;
            int param = 0;

            sm.AddTransition("narf", "zod", (o) => { return (int)o == 23; }, (o) => { param = (int)o; actionCalled = true; });
            sm.SetStart("narf");
            sm.DoTransition(23);

            Assert.IsTrue(actionCalled);
            Assert.AreEqual(23, param);
        }

        [TestMethod]
        public void StateMachine_ShouldTransitionTwice()
        {
            StateMachine sm = new StateMachine();
            bool action1Called = false;
            bool action2Called = false;
            bool action3Called = false;

            int param1 = 0;
            int param2 = 0;
            int param3 = 0;

            sm.AddTransition("narf", "zod", (o) => { return (int)o == 23; }, (o) => { param1 = (int)o; action1Called = true; });
            sm.AddTransition("narf", "poit", (o) => { return (int)o == 7; }, (o) => { param2 = (int)o; action2Called = true; });
            sm.AddTransition("zod", "poit", (o) => { return (int)o == 42; }, (o) => { param3 = (int)o; action3Called = true; });

            sm.SetStart("narf");
            sm.DoTransition(23);
            sm.DoTransition(42);

            Assert.IsTrue(action1Called);
            Assert.IsFalse(action2Called);
            Assert.IsTrue(action3Called);

            Assert.AreEqual(23, param1);
            Assert.AreEqual(0, param2);
            Assert.AreEqual(42, param3);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StateMachine_InvalidStateFails()
        {
            StateMachine sm = new StateMachine();

            sm.DoTransition(23);
        }
    }
}

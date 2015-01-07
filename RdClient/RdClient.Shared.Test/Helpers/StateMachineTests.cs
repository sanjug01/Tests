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
        enum TestStates
        { 
            Narf,
            Zod,
            Poit
        }

        [TestMethod]
        public void StateMachine_ShouldTransition()
        {
            StateMachine<TestStates, string> sm = new StateMachine<TestStates, string>();
            bool actionCalled = false;

            sm.AddTransition(TestStates.Narf, TestStates.Zod, (o) => { return true; }, (o) => { actionCalled = true; });
            sm.SetStart(TestStates.Narf);
            sm.Consume("whatever");

            Assert.IsTrue(actionCalled);
        }

        [TestMethod]
        public void StateMachine_ShouldSetParam()
        {
            StateMachine<TestStates, object> sm = new StateMachine<TestStates, object>();
            bool actionCalled = false;
            int param = 0;

            sm.AddTransition(TestStates.Narf, TestStates.Zod, (o) => { return (int)o == 23; }, (o) => { param = (int)o; actionCalled = true; });
            sm.SetStart(TestStates.Narf);
            sm.Consume(23);

            Assert.IsTrue(actionCalled);
            Assert.AreEqual(23, param);
        }

        [TestMethod]
        public void StateMachine_ShouldTransitionTwice()
        {
            StateMachine<TestStates, int> sm = new StateMachine<TestStates, int>();
            bool action1Called = false;
            bool action2Called = false;
            bool action3Called = false;

            int param1 = 0;
            int param2 = 0;
            int param3 = 0;

            sm.AddTransition(TestStates.Narf, TestStates.Zod, (o) => { return o == 23; }, (o) => { param1 = o; action1Called = true; });
            sm.AddTransition(TestStates.Narf, TestStates.Poit, (o) => { return o == 7; }, (o) => { param2 = o; action2Called = true; });
            sm.AddTransition(TestStates.Zod, TestStates.Zod, (o) => { return o == 42; }, (o) => { param3 = o; action3Called = true; });

            sm.SetStart(TestStates.Narf);
            sm.Consume(23);
            sm.Consume(42);

            Assert.IsTrue(action1Called);
            Assert.IsFalse(action2Called);
            Assert.IsTrue(action3Called);

            Assert.AreEqual(23, param1);
            Assert.AreEqual(0, param2);
            Assert.AreEqual(42, param3);
        }

        [TestMethod]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void StateMachine_InvalidStateFails()
        {
            StateMachine<TestStates, int> sm = new StateMachine<TestStates, int>();

            sm.Consume(23);
        }
    }
}

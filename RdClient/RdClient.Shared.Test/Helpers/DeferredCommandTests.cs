namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using RdClient.Shared.ViewModels;
    using System;
using System.Collections.Generic;

    [TestClass]
    public sealed class DeferredCommandTests
    {
        private sealed class TestTimer : ITimer
        {
            public Action Callback;
            public TimeSpan Period;
            public bool Recurring;

            public TestTimer()
            {
            }

            void ITimer.Start(Action callback, TimeSpan period, bool recurring)
            {
                Assert.IsNotNull(callback);
                Assert.IsNull(this.Callback);

                this.Callback = callback;
                this.Period = period;
                this.Recurring = recurring;
            }

            void ITimer.Stop()
            {
            }
        }

        private sealed class TestTimerFactory : ITimerFactory
        {
            public readonly TestTimer Timer = new TestTimer();

            ITimer ITimerFactory.CreateTimer()
            {
                return this.Timer;
            }
        }

        private sealed class Command
        {
            private bool _canExecute;

            public readonly RelayCommand Cmd;
            public int Executed = 0;

            public bool CanExecute
            {
                get { return _canExecute; }
                set
                {
                    if(value != _canExecute)
                    {
                        _canExecute = value;
                        Cmd.EmitCanExecuteChanged();
                    }
                }
            }

            public Command()
            {
                this.Cmd = new RelayCommand(this.ExecuteCommand, this.CanExecuteCommand);
            }

            private void ExecuteCommand(object parameter)
            {
                ++this.Executed;
            }

            private bool CanExecuteCommand(object parameter)
            {
                return _canExecute;
            }
        }

        private sealed class TestDeferredExecution : IDeferredExecution
        {
            public readonly IList<Action> Actions = new List<Action>();

            public void ExecuteActions()
            {
                int count = this.Actions.Count;
                foreach (Action a in this.Actions)
                    a();
                Assert.AreEqual(count, this.Actions.Count);
                this.Actions.Clear();
            }

            void IDeferredExecution.Defer(Action action)
            {
                Assert.IsNotNull(action);
                this.Actions.Add(action);
            }
        }

        [TestMethod]
        public void NewDeferredCommand_DisabledCommand_TimerNotSet()
        {
            Command c = new Command() { CanExecute = false };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            Assert.IsNull(tf.Timer.Callback);
        }

        [TestMethod]
        public void NewDeferredCommand_EnabledCommand_TimerSet()
        {
            Command c = new Command() { CanExecute = true };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            Assert.IsNotNull(tf.Timer.Callback);
        }

        [TestMethod]
        public void NewDeferredCommand_EnabledCommandExecuteTimerAndDeferred_CommandCalled()
        {
            Command c = new Command() { CanExecute = true };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            Assert.IsNotNull(tf.Timer.Callback);
            tf.Timer.Callback();
            Assert.AreEqual(1, de.Actions.Count);
            de.ExecuteActions();
            Assert.AreEqual(1, c.Executed);
        }

        [TestMethod]
        public void NewDeferredCommand_DisabledCommandEnable_TimerSet()
        {
            Command c = new Command() { CanExecute = false };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            c.CanExecute = true;

            Assert.IsNotNull(tf.Timer.Callback);
        }

        [TestMethod]
        public void NewDeferredCommand_DisabledCommandEnableExecuteTimerAndDeferred_CommandCalled()
        {
            Command c = new Command() { CanExecute = false };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            c.CanExecute = true;

            Assert.IsNotNull(tf.Timer.Callback);
            tf.Timer.Callback();
            Assert.AreEqual(1, de.Actions.Count);
            de.ExecuteActions();
            Assert.AreEqual(1, c.Executed);
        }

        [TestMethod]
        public void NewDeferredCommand_DisabledCommandEnableDisableExecuteTimerAndDeferred_DeferredButNotCalled()
        {
            Command c = new Command() { CanExecute = false };
            TestTimerFactory tf = new TestTimerFactory();
            TestDeferredExecution de = new TestDeferredExecution();
            DeferredCommand dc = new DeferredCommand(c.Cmd, de, tf, 100);

            c.CanExecute = true;
            c.CanExecute = false;

            Assert.IsNotNull(tf.Timer.Callback);
            tf.Timer.Callback();
            Assert.AreEqual(1, de.Actions.Count);
            de.ExecuteActions();
            Assert.AreEqual(0, c.Executed);
        }
    }
}

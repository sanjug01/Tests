namespace RdClient.Shared.Test.Helpers
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Input;

    [TestClass]
    public sealed class GroupCommandTests
    {
        private sealed class TestCommand : ICommand
        {
            private EventHandler _canExecuteChanged;
            private bool _canExecute;

            public readonly IList<object> ExecuteCalls = new List<object>();
            public readonly IList<object> CanExecuteCalls = new List<object>();

            public bool CanExecute
            {
                get { return _canExecute; }
                set
                {
                    if(value != _canExecute)
                    {
                        _canExecute = value;
                        if (null != _canExecuteChanged)
                            _canExecuteChanged(this, EventArgs.Empty);
                    }
                }
            }

            public TestCommand()
            {
                _canExecute = true;
            }

            bool ICommand.CanExecute(object parameter)
            {
                this.CanExecuteCalls.Add(parameter);
                return this.CanExecute;
            }

            event EventHandler ICommand.CanExecuteChanged
            {
                add { _canExecuteChanged += value; }
                remove { _canExecuteChanged -= value; }
            }

            void ICommand.Execute(object parameter)
            {
                this.ExecuteCalls.Add(parameter);
            }
        }

        [TestMethod]
        public void NewGroupCommand_CannotExecute()
        {
            GroupCommand group = new GroupCommand();
            Assert.IsNotNull(group.Command);
            Assert.IsNull(group.CommandParameter);
            Assert.IsFalse(group.Command.CanExecute(null));
        }

        [TestMethod]
        public void GroupCommand_SetCommandParameter_ChangeReported()
        {
            IList<PropertyChangedEventArgs> changes = new List<PropertyChangedEventArgs>();
            object parameter = new object();
            GroupCommand group = new GroupCommand();
            group.PropertyChanged += (sender, e) => changes.Add(e);

            group.CommandParameter = parameter;

            Assert.AreSame(parameter, group.CommandParameter);
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual("CommandParameter", changes[0].PropertyName);
        }

        [TestMethod]
        public void GroupCommand_SetParameterAddCommand_CanExecuteCalledWithParameter()
        {
            GroupCommand group = new GroupCommand() { CommandParameter = new object() };
            TestCommand tc = new TestCommand();

            group.Add(tc);

            Assert.AreEqual(1, tc.CanExecuteCalls.Count);
            Assert.AreSame(group.CommandParameter, tc.CanExecuteCalls[0]);
        }

        [TestMethod]
        public void GroupCommand_AddCommandCanExecute_CallsCommand()
        {
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand();
            object parameter = new object();

            group.Add(tc);
            Assert.AreEqual(1, tc.CanExecuteCalls.Count);
            Assert.IsNull(tc.CanExecuteCalls[0]);

            Assert.IsTrue(tc.CanExecute);
            Assert.IsTrue(group.Command.CanExecute(parameter));
            Assert.AreEqual(2, tc.CanExecuteCalls.Count);
            Assert.AreSame(parameter, tc.CanExecuteCalls[1]);
        }

        [TestMethod]
        public void GroupCommand_AddCommand_ChangeReported()
        {
            IList<ICommand> changed = new List<ICommand>();
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand();
            group.Command.CanExecuteChanged += (sender, e) => changed.Add((ICommand)sender);

            group.Add(tc);
            Assert.AreEqual(1, changed.Count);
            Assert.AreSame(group.Command, changed[0]);
        }

        [TestMethod]
        public void GroupCommand_AddDisabledCommand_ChangeNotReported()
        {
            IList<ICommand> changed = new List<ICommand>();
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand() { CanExecute = false };
            group.Command.CanExecuteChanged += (sender, e) => changed.Add((ICommand)sender);

            group.Add(tc);
            Assert.AreEqual(0, changed.Count);
            Assert.IsFalse(group.Command.CanExecute(null));
        }

        [TestMethod]
        public void GroupCommand_AddDisabledCommandEnable_ChangeReported()
        {
            IList<ICommand> changed = new List<ICommand>();
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand() { CanExecute = false };
            group.Command.CanExecuteChanged += (sender, e) => changed.Add((ICommand)sender);
            group.Add(tc);
            tc.CanExecuteCalls.Clear();

            tc.CanExecute = true;
            Assert.AreEqual(1, changed.Count);
            Assert.AreEqual(1, tc.CanExecuteCalls.Count);
            Assert.IsNull(tc.CanExecuteCalls[0]);
            Assert.IsTrue(group.Command.CanExecute(null));
        }

        [TestMethod]
        public void GroupCommand_AddEnabledCommandDisable_ChangeReported()
        {
            IList<ICommand> changed = new List<ICommand>();
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand();
            group.Add(tc);
            tc.CanExecuteCalls.Clear();
            group.Command.CanExecuteChanged += (sender, e) => changed.Add((ICommand)sender);

            tc.CanExecute = false;

            Assert.AreEqual(1, changed.Count);
            Assert.AreEqual(1, tc.CanExecuteCalls.Count);
            Assert.IsNull(tc.CanExecuteCalls[0]);
            Assert.IsFalse(group.Command.CanExecute(null));
        }

        [TestMethod]
        public void GroupCommand_RemoveCommandChangeCanExecute_ChangeIgnored()
        {
            IList<ICommand> changed = new List<ICommand>();
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand() { CanExecute = false };
            group.Add(tc);
            tc.CanExecuteCalls.Clear();
            group.Command.CanExecuteChanged += (sender, e) => changed.Add((ICommand)sender);

            group.Remove(tc);
            Assert.AreEqual(1, changed.Count);

            tc.CanExecute = false;

            Assert.AreEqual(1, changed.Count);
            Assert.AreEqual(0, tc.CanExecuteCalls.Count);
            Assert.IsFalse(group.Command.CanExecute(null));
            Assert.AreEqual(0, tc.CanExecuteCalls.Count);
        }

        [TestMethod]
        public void GroupCommand_AddExecute_Executed()
        {
            GroupCommand group = new GroupCommand();
            TestCommand tc = new TestCommand();
            object parameter = new object();

            group.Add(tc);
            group.Command.Execute(parameter);

            Assert.AreEqual(1, tc.ExecuteCalls.Count);
            Assert.AreSame(parameter, tc.ExecuteCalls[0]);
        }

        [TestMethod]
        public void GroupCommand_AddManyExecute_AllExecuted()
        {
            IList<TestCommand> commands = new List<TestCommand>();
            GroupCommand group = new GroupCommand();
            object parameter = new object();

            for (int i = 0; i < 100; ++i)
            {
                TestCommand tc = new TestCommand();
                group.Add(tc);
                commands.Add(tc);
            }

            group.Command.Execute(parameter);

            foreach (TestCommand tc in commands)
            {
                Assert.AreEqual(1, tc.ExecuteCalls.Count);
                Assert.AreSame(parameter, tc.ExecuteCalls[0]);
            }
        }

        [TestMethod]
        public void GroupCommand_AddManySomeDisabledExecute_OnlyEnabledExecuted()
        {
            IList<TestCommand> commands = new List<TestCommand>();
            GroupCommand group = new GroupCommand();
            object parameter = new object();

            for (int i = 0; i < 400; ++i)
            {
                TestCommand tc = new TestCommand();
                group.Add(tc);
                //
                // Change "CanExecute" after adding thecommand to the group
                // to test that the groupcorrectly tracks many commands.
                //
                if (0 == i % 2)
                    tc.CanExecute = false;
                commands.Add(tc);
            }

            group.Command.Execute(parameter);

            foreach (TestCommand tc in commands)
            {
                if (tc.CanExecute)
                {
                    Assert.AreEqual(1, tc.ExecuteCalls.Count);
                    Assert.AreSame(parameter, tc.ExecuteCalls[0]);
                }
                else
                {
                    Assert.AreEqual(0, tc.ExecuteCalls.Count);
                }
            }
        }
    }
}

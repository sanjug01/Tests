using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using RdClient.Shared.Helpers;
using RdClient.Shared.ViewModels;
using System.Collections.Generic;

namespace RdClient.Shared.Test.Helpers
{
    [TestClass]
    public sealed class CommandBindingTests
    {
        [TestMethod]
        public void NewCommandBinding_CannotExecute()
        {
            CommandBinding cb = new CommandBinding();

            Assert.IsNull(cb.Command);
            Assert.IsNull(cb.Parameter);
            Assert.IsFalse(cb.CanExecute);
        }

        [TestMethod]
        public void CommandBinding_ChangeParameter_ChangeReported()
        {
            CommandBinding cb = new CommandBinding();
            bool changed = false;

            cb.PropertyChanged += (sender, e) =>
            {
                Assert.AreEqual("Parameter", e.PropertyName);
                changed = true;
            };
            cb.Parameter = new object();

            Assert.IsTrue(changed);
        }

        [TestMethod]
        public void CommandBinding_SetEnabledCommand_CanExecuteChangeReported()
        {
            CommandBinding cb = new CommandBinding();
            RelayCommand command = new RelayCommand(p => { });
            List<string> changes = new List<string>();

            cb.PropertyChanged += (sender, e) => changes.Add(e.PropertyName);
            cb.Command = command;

            Assert.AreEqual(2, changes.Count);
            CollectionAssert.Contains(changes, "Command");
            CollectionAssert.Contains(changes, "CanExecute");
        }

        [TestMethod]
        public void CommandBinding_SetDisabledCommand_CanExecuteChangeNotReported()
        {
            CommandBinding cb = new CommandBinding();
            RelayCommand command = new RelayCommand(p => { }, p => false);
            List<string> changes = new List<string>();

            cb.PropertyChanged += (sender, e) => changes.Add(e.PropertyName);
            cb.Command = command;

            Assert.AreEqual(1, changes.Count);
            CollectionAssert.Contains(changes, "Command");
        }

        [TestMethod]
        public void CommandBinding_SetEnabledCommand_CanExecute()
        {
            CommandBinding cb = new CommandBinding();
            RelayCommand command = new RelayCommand(p => { });

            cb.Command = command;

            Assert.AreSame(cb.Command, command);
            Assert.IsTrue(cb.CanExecute);
        }

        [TestMethod]
        public void CommandBinding_ChangeParameter_CanExecuteChanges()
        {
            CommandBinding cb = new CommandBinding();
            RelayCommand command = new RelayCommand(p => { }, p => null == p);

            cb.Command = command;
            Assert.IsTrue(cb.CanExecute);

            cb.Parameter = new object();

            Assert.IsFalse(cb.CanExecute);
        }
    }
}

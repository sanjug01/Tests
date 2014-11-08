namespace RdClient.Shared.Test.ViewModels
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using RdClient.Shared.ViewModels;
    using System.Windows.Input;

    [TestClass]
    public class BarButtonModelTests
    {
        private bool _canExecuteCommand;
        private RelayCommand _command;
        private BarButtonModel _model;
        private bool _commandExecuted;

        const string CommandLabel = "label";

        private class TestModel : BarButtonModel
        {
            public TestModel(ICommand command, string label) : base(command, label) { }
            protected override void OnPresenting(object activationParameter)
            {
                //throw new System.NotImplementedException();
            }
        }

        [TestInitialize]
        public void SetUpTest()
        {
            _commandExecuted = false;
            _canExecuteCommand = false;
            _command = new RelayCommand(o => _commandExecuted = true, o => _canExecuteCommand);
            _model = new TestModel(_command, "label");
        }

        public void TearDownTest()
        {
            _model = null;
            _command = null;
        }

        [TestMethod]
        public void NewBarButtonModel_CorrectLabel()
        {
            Assert.AreEqual(CommandLabel, _model.Label);
        }

        [TestMethod]
        public void NewBarButtonModel_CannotExecute()
        {
            Assert.IsFalse(_model.Command.CanExecute(null));
            Assert.IsFalse(_model.IsVisible);
        }

        [TestMethod]
        public void NewBarButtonModel_EnableCommandExecution_BecomesVisible()
        {
            _canExecuteCommand = true;
            _command.EmitCanExecuteChanged();
            Assert.IsTrue(_model.Command.CanExecute(null));
            Assert.IsTrue(_model.IsVisible);
        }

        [TestMethod]
        public void NewBarButtonModel_DisableCommandExecution_Hides()
        {
            _canExecuteCommand = true;
            _command.EmitCanExecuteChanged();
            Assert.IsTrue(_model.Command.CanExecute(null));
            Assert.IsTrue(_model.IsVisible);

            _canExecuteCommand = false;
            _command.EmitCanExecuteChanged();
            Assert.IsFalse(_model.Command.CanExecute(null));
            Assert.IsFalse(_model.IsVisible);
        }

        [TestMethod]
        public void NewBarButtonModel_EnableAndExecute_CommandExecutes()
        {
            _canExecuteCommand = true;
            _command.EmitCanExecuteChanged();
            Assert.IsFalse(_commandExecuted);
            _model.Command.Execute(null);
            Assert.IsTrue(_commandExecuted);
        }
    }
}

namespace RdClient.Shared.Helpers
{
    using RdClient.Shared.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// A group of commands that must be executed together.
    /// The class keeps track of the commands that had reported that they can be executed
    /// and executes only the ones that can.
    /// </summary>
    public sealed class GroupCommand : MutableObject
    {
        private readonly ISet<ICommand> _trackedCommands;
        private readonly ISet<ICommand> _pendingCommands;
        private readonly RelayCommand _command;

        private bool _eventSuppressed;
        private object _commandParameter;

        public object CommandParameter
        {
            get { return _commandParameter; }
            set { this.SetProperty(ref _commandParameter, value); }
        }

        public ICommand Command
        {
            get
            {
                Contract.Ensures(null != Contract.Result<ICommand>());
                return _command;
            }
        }

        public GroupCommand()
        {
            Contract.Ensures(null != _command);
            Contract.Ensures(null != _trackedCommands);
            Contract.Ensures(null != _pendingCommands);

            _trackedCommands = new HashSet<ICommand>();
            _pendingCommands = new HashSet<ICommand>();
            _command = new RelayCommand(this.Execute, this.CanExecute);
            _eventSuppressed = false;
        }

        public void Add(ICommand command)
        {
            Contract.Assert(null != command);

            if (_trackedCommands.Add(command))
            {
                command.CanExecuteChanged += this.OnCanExecuteChanged;

                if (command.CanExecute(_commandParameter))
                {
                    _pendingCommands.Add(command);
                    EmitCanExecuteChanged();
                }
            }
        }

        public void Remove(ICommand command)
        {
            Contract.Assert(null != command);

            if (_trackedCommands.Remove(command))
            {
                _pendingCommands.Remove(command);
                EmitCanExecuteChanged();
            }
        }

        private void OnCanExecuteChanged(object sender, EventArgs e)
        {
            ICommand saveCommand = sender as ICommand;
            Contract.Assert(null != saveCommand);

            bool changed;

            if (saveCommand.CanExecute(_commandParameter))
            {
                changed = _pendingCommands.Add(saveCommand);
            }
            else
            {
                changed = _pendingCommands.Remove(saveCommand);
            }

            if (changed)
                EmitCanExecuteChanged();
        }

        private void EmitCanExecuteChanged()
        {
            if (!_eventSuppressed)
            {
                _command.EmitCanExecuteChanged();
            }
        }

        private void Execute(object parameter)
        {
            try
            {
                //
                // Copy all commands to a temporary list so the enumeration of the set
                // is protected from changes made to the set while it is being enumerated.
                //
                IList<ICommand> allCommands = new List<ICommand>(_pendingCommands);

                _eventSuppressed = true;

                foreach (ICommand command in allCommands)
                {
                    if (command.CanExecute(parameter))
                    {
                        command.Execute(parameter);
                    }
                }
            }
            finally
            {
                _eventSuppressed = false;
            }

            EmitCanExecuteChanged();
        }

        private bool CanExecute(object parameter)
        {
            bool canSave = false;

            foreach (ICommand command in _pendingCommands)
            {
                if (command.CanExecute(parameter))
                {
                    canSave = true;
                    break;
                }
            }

            return canSave;
        }
    }
}

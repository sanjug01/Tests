namespace RdClient.Shared.Helpers
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Windows.Input;

    /// <summary>
    /// The class tracks the status of an ICommand object and when the command reports that it can be
    /// executed, sets a timer that delays execution of the command for a specified interval.
    /// </summary>
    public sealed class DeferredCommand
    {
        private readonly ICommand _command;
        private readonly IDeferredExecution _deferredExecution;
        private readonly ITimerFactory _timerFactory;
        private readonly int _delayMilliseconds;

        private ITimer _timer;

        public DeferredCommand(ICommand command, IDeferredExecution deferredExecution, ITimerFactory timerFactory, int delayMilliseconds)
        {
            Contract.Assert(null != command);
            Contract.Assert(null != deferredExecution);
            Contract.Assert(null != timerFactory);
            Contract.Assert(delayMilliseconds > 0);

            _deferredExecution = deferredExecution;
            _timerFactory = timerFactory;
            _delayMilliseconds = delayMilliseconds;
            _command = command;
            _command.CanExecuteChanged += (sender, e) => this.DelayCommand();

            DelayCommand();
        }

        private void DelayCommand()
        {
            //
            // Set the timer if the command can be executed and the timer is not already set.
            //
            if (null == _timer && _command.CanExecute(null))
            {
                _timer = _timerFactory.CreateTimer();
                _timer.Start(() => _deferredExecution.Defer(this.TimerAction), TimeSpan.FromMilliseconds((double)_delayMilliseconds), false);
            }
        }

        private void TimerAction()
        {
            Contract.Assert(null != _timer);
            _timer = null;

            if(_command.CanExecute(null))
            {
                _command.Execute(null);
            }
        }
    }
}

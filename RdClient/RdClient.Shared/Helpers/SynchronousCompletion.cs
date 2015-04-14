namespace RdClient.Shared.Helpers
{
    using System;

    /// <summary>
    /// Helper class that broadcasts completion of some process to its subscribers.
    /// </summary>
    public sealed class SynchronousCompletion
    {
        private bool _isCompleted;
        private EventHandler _completed;

        public event EventHandler Completed
        {
            add
            {
                _completed += value;
                if (_isCompleted)
                    value(this, EventArgs.Empty);
            }

            remove
            {
                _completed -= value;
            }
        }

        /// <summary>
        /// Emit the Completed event and mark the object as completed; once marked completed, the object
        /// emits the event to all new subscribers when they subscribe.
        /// </summary>
        public void Complete()
        {
            if (!_isCompleted)
            {
                _isCompleted = true;

                if (null != _completed)
                    _completed(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Reset the object - mark it as not completed.
        /// </summary>
        public void Reset()
        {
            _isCompleted = false;
        }
    }
}

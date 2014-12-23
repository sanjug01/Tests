namespace RdClient.Shared.Input
{
    using System;

    public interface IKeyboardCapture
    {
        event EventHandler<KeystrokeEventArgs> Keystroke;

        /// <summary>
        /// Start capturing and reporting keystrokes.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop capturing and reporting keystrokes and send "key released" events for all currently
        /// pressed keys to the subscribers.
        /// </summary>
        void Stop();
    }
}

namespace RdClient.Shared.Test.Input.Keyboard
{
    using RdClient.Shared.Input.Keyboard;
    using System;

    internal sealed class TestSink : IKeyboardCaptureSink
    {
        internal sealed class SinkEventArgs : EventArgs
        {
            public readonly int KeyCode;
            public readonly bool IsScanCode, IsExtendedKey, IsKeyReleased;

            public SinkEventArgs(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
            {
                KeyCode = keyCode;
                IsScanCode = isScanCode;
                IsExtendedKey = isExtendedKey;
                IsKeyReleased = isKeyReleased;
            }
        }

        public event EventHandler<SinkEventArgs> Keystroke;

        void IKeyboardCaptureSink.ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            if (null != this.Keystroke)
                this.Keystroke(this, new SinkEventArgs(keyCode, isScanCode, isExtendedKey, isKeyReleased));
        }
    }
}

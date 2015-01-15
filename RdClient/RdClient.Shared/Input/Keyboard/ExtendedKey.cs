namespace RdClient.Shared.Input.Keyboard
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using Windows.System;
    using Windows.UI.Core;

    /// <summary>
    /// Extended virtual key - representation of pairs of keys with the same scan code and virtual key code
    /// differing in the "extended key" flag. For each such virtual key the pressed status of all physical keys
    /// is tracked.
    /// </summary>
    public sealed class ExtendedKey : VirtualKeyBase
    {
        public ExtendedKey(VirtualKey virtualKey, IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
            : base(virtualKey, keyboardSink, keyboardState)
        {
        }

        protected override bool OnPressed(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            //
            // Report the key down to the sink
            //
            this.ReportKeyEvent((int)keyStatus.ScanCode, true, keyStatus.IsExtendedKey, false);
            //
            // Return true to indicate that no further events for the key are needed. Even if the key
            // can produce character events, the class is not interested in them.
            //
            return true;
        }

        protected override void OnReleased(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            //
            // Report the key up tp the sink
            //
            this.ReportKeyEvent((int)keyStatus.ScanCode, true, keyStatus.IsExtendedKey, true);
        }
    }
}

namespace RdClient.Shared.Input.Keyboard
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    /// <summary>
    /// Keyboard event handler for keys that share a virtual key code (Shift).
    /// For such keys "key down" event is reported for each physical key, but only one "key up"
    /// event is reported when the last key is released.
    /// </summary>
    public sealed class SingleReleaseKey : VirtualKeyBase
    {
        public SingleReleaseKey(VirtualKey virtualKey, IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
            : base(virtualKey, keyboardSink, keyboardState)
        {
        }

        protected override bool OnPressed(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            this.ReportKeyEvent((int)keyStatus.ScanCode, true, keyStatus.IsExtendedKey, false);
            return false;
        }

        protected override void OnReleased(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            this.ForEachPhysicalKey(kvp => this.ReportKeyEvent((int)kvp.Key.ScanCode, true, kvp.Key.IsExtendedKey, true));
            //
            // When the method is called, the physical key described in keyStatus has already been released,
            // so it won't be included in the ForEachPhysicalKey enumeration and must be reported separately.
            //
            this.ReportKeyEvent((int)keyStatus.ScanCode, true, keyStatus.IsExtendedKey, true);
            //
            // Remove all physical keys so VirtualKeyBase can remove itself from the keyboard state.
            //
            ClearPhysicalKeys();
        }
    }
}

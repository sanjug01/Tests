namespace RdClient.Shared.Input.Keyboard
{
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public sealed class VirtualKeyFactory : IVirtualKeyFactory
    {
        private readonly IKeyboardState _keyboardState;
        private readonly IKeyboardCaptureSink _keyboardSink;

        /// <summary>
        /// Classification of a virtual key
        /// </summary>
        private enum VirtualKeyClass
        {
            Ignored,    // nothing is sent
            ReleaseAll, // single "key up" releases all physical keys
            Extended,   // extended keys - send as scan codes
            Character
        }

        public VirtualKeyFactory(IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
        {
            Contract.Requires(null != keyboardState);
            Contract.Requires(null != keyboardSink);
            _keyboardState = keyboardState;
            _keyboardSink = keyboardSink;
        }

        IVirtualKey IVirtualKeyFactory.MakeVirtualKey(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            Contract.Ensures(null != Contract.Result<IVirtualKey>());

            IVirtualKey key;

            switch(GetKeyClass(virtualKey, keyStatus))
            {
                case VirtualKeyClass.Extended:
                    key = RegisterVirtualKey(virtualKey, new ExtendedKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                case VirtualKeyClass.ReleaseAll:
                    key = RegisterVirtualKey(virtualKey, new SingleReleaseKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                case VirtualKeyClass.Character:
                    key = RegisterVirtualKey(virtualKey, new CharacterKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                default:
                    //
                    // All other classes are served by the single instance of DummyVirtualKey that is not
                    // registered with the keyboard state object.
                    //
                    key = DummyVirtualKey.Dummy;
                    break;
            }

            return key;
        }

        private IVirtualKey RegisterVirtualKey(VirtualKey virtualKey, IVirtualKey vk)
        {
            _keyboardState.RegisterVirtualKey(virtualKey, vk);
            return vk;
        }

        private static VirtualKeyClass GetKeyClass(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            VirtualKeyClass keyClass = VirtualKeyClass.Ignored;

            switch (virtualKey)
            {
                case VirtualKey.None:
                case VirtualKey.LeftWindows:
                case VirtualKey.RightWindows:
                    break;

                case VirtualKey.Shift:
                    //
                    // Left or right "Shift" keys; when both keys are pressed and released, one "KeyUp" event is emitted
                    // by the dispatcher. The event must release both pressed keys.
                    //
                    keyClass = VirtualKeyClass.ReleaseAll;
                    break;

                case VirtualKey.Control:
                case VirtualKey.Menu:
                case VirtualKey.Enter:
                case VirtualKey.Home:
                case VirtualKey.End:
                case VirtualKey.PageUp:
                case VirtualKey.PageDown:
                case VirtualKey.Left:
                case VirtualKey.Right:
                case VirtualKey.Up:
                case VirtualKey.Down:
                case VirtualKey.Delete:
                case VirtualKey.Insert:
                //
                // Arithmetic operations in the numeric keypad area
                //
                case VirtualKey.Divide:
                case VirtualKey.Multiply:
                case VirtualKey.Subtract:
                case VirtualKey.Add:
                //
                // "Lock" keys
                //
                case VirtualKey.CapitalLock:
                case VirtualKey.NumberKeyLock:
                case VirtualKey.Scroll:
                    keyClass = VirtualKeyClass.Extended;
                    break;

                case (VirtualKey)231:
                    //
                    // VirtualKey 231 represents the emoticons on the touch keyboard.
                    // All emoticon keys share the same scan code 0 (dumb as it is, that is how the keyboard team has done it),
                    // and only one may be pressed at any time. Pressing the second emoticon key on the touch keyboard immediately
                    // releases the one already pressed.
                    // Emoticon key events are tracked by the Character key class that sends Unicode code points instead of scan codes.
                    //
                    keyClass = VirtualKeyClass.Character;
                    break;

                default:
                    //
                    // All other keys are sent as scan codes.
                    //
#if false
                    if(IsKeyInRange(virtualKey, VirtualKey.A, VirtualKey.Z)
                    || IsKeyInRange(virtualKey, VirtualKey.Number0, VirtualKey.Number9)
                    || IsCharacterScanCode(keyStatus.ScanCode)
                    || VirtualKey.Space == virtualKey)
                    {
                        keyClass = VirtualKeyClass.Character;
                    }
                    else
                    {
                        keyClass = VirtualKeyClass.Extended;
                    }
#endif
                    keyClass = VirtualKeyClass.Extended;
                    break;
            }

            return keyClass;
        }

        private static bool IsKeyInRange(VirtualKey key, VirtualKey first, VirtualKey last)
        {
            return key >= first && key <= last;
        }

        private static bool IsCharacterScanCode(uint scanCode)
        {
            bool character = false;

            switch(scanCode)
            {
                case 12:
                case 13:
                case 26:
                case 27:
                case 39:
                case 40:
                case 41:
                case 43:
                case 51:
                case 52:
                case 53:
                    character = true;
                    break;
            }

            return character;
        }
    }
}

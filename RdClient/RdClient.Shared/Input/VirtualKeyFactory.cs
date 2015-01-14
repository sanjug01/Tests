namespace RdClient.Shared.Input
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
                    key = AddVirtualKey(virtualKey, new ExtendedKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                case VirtualKeyClass.ReleaseAll:
                    key = AddVirtualKey(virtualKey, new SingleReleaseKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                case VirtualKeyClass.Character:
                    key = AddVirtualKey(virtualKey, new CharacterKey(virtualKey, _keyboardSink, _keyboardState));
                    break;

                default:
                    key = DummyVirtualKey.Dummy;
                    break;
            }

            return key;
        }

        private IVirtualKey AddVirtualKey(VirtualKey virtualKey, IVirtualKey vk)
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
                case VirtualKey.Divide:
                case VirtualKey.Multiply:
                case VirtualKey.Subtract:
                case VirtualKey.Add:
                    keyClass = VirtualKeyClass.Extended;
                    break;

                case VirtualKey.CapitalLock:
                case VirtualKey.NumberKeyLock:
                case VirtualKey.Scroll:
                    break;

                default:
                    //
                    // Alphanumeric
                    //
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

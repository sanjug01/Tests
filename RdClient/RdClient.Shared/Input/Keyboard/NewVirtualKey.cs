namespace RdClient.Shared.Input.Keyboard
{
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public sealed class NewVirtualKey : IVirtualKey
    {
        private readonly VirtualKey _virtualKey;
        private readonly IVirtualKeyFactory _keyFactory;
        private readonly IKeyboardState _keyboardState;

        public NewVirtualKey(VirtualKey virtualKey, IVirtualKeyFactory keyFactory, IKeyboardState keyboardState)
        {
            Contract.Requires(null != keyboardState);
            Contract.Ensures(null != _keyboardState);

            _virtualKey = virtualKey;
            _keyFactory = keyFactory;
            _keyboardState = keyboardState;
        }

        bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            bool completelyProcessed = false;
            IVirtualKey newKey;

            switch(eventType)
            {
                case CoreAcceleratorKeyEventType.KeyDown:
                case CoreAcceleratorKeyEventType.SystemKeyDown:
                    newKey = _keyFactory.MakeVirtualKey(_virtualKey, keyStatus, _keyboardState);
                    completelyProcessed = newKey.Update(eventType, virtualKey, keyStatus);
                    break;

                default:
                    break;
            }

            return completelyProcessed;
        }



        void IVirtualKey.Clear()
        {
        }
    }
}

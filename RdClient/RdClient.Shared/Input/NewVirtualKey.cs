namespace RdClient.Shared.Input
{
    using Windows.System;
    using Windows.UI.Core;

    public sealed class NewVirtualKey : IVirtualKey
    {
        private readonly VirtualKey _virtualKey;
        private readonly IVirtualKeyFactory _keyFactory;

        public NewVirtualKey(VirtualKey virtualKey, IVirtualKeyFactory keyFactory)
        {
            _virtualKey = virtualKey;
            _keyFactory = keyFactory;
        }

        bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            bool completelyProcessed = false;
            IVirtualKey newKey;

            switch(eventType)
            {
                case CoreAcceleratorKeyEventType.KeyDown:
                case CoreAcceleratorKeyEventType.SystemKeyDown:
                    //
                    // TODO: separate creation of the key and processing the key down event.
                    //
                    newKey = _keyFactory.MakeVirtualKey(_virtualKey, keyStatus);
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

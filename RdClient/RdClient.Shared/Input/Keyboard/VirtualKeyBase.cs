namespace RdClient.Shared.Input.Keyboard
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public abstract class VirtualKeyBase : IVirtualKey
    {
        private readonly VirtualKey _virtualKey;
        private readonly IKeyboardState _keyboardState;
        private readonly IKeyboardCaptureSink _keyboardSink;
        private readonly IDictionary<CorePhysicalKeyStatus, PhysicalKeyDataContainer> _physicalKeys;

        protected VirtualKey VirtualKey { get { return _virtualKey; } }

        protected VirtualKeyBase(VirtualKey virtualKey, IKeyboardCaptureSink keyboardSink, IKeyboardState keyboardState)
        {
            Contract.Assert(VirtualKey.None != virtualKey);
            Contract.Assert(null != keyboardState);
            Contract.Assert(null != keyboardSink);
            Contract.Ensures(null != _keyboardState);
            Contract.Ensures(null != _keyboardSink);

            _virtualKey = virtualKey;
            _keyboardState = keyboardState;
            _keyboardSink = keyboardSink;
            _physicalKeys = new SortedDictionary<CorePhysicalKeyStatus, PhysicalKeyDataContainer>(new ScanCodeComparer());
        }

        protected void RegisterPhysicalKey(CorePhysicalKeyStatus keyStatus)
        {
            _keyboardState.RegisterPhysicalKey(keyStatus, this);
        }

        protected void UnregisterPhysicalKey(CorePhysicalKeyStatus keyStatus)
        {
            IVirtualKey vk = _keyboardState.UnregisterPhysicalKey(keyStatus);
            Contract.Assert(object.ReferenceEquals(vk, this));
        }

        protected void ForEachPhysicalKey(Action<KeyValuePair<CorePhysicalKeyStatus, PhysicalKeyDataContainer>> action)
        {
            foreach(KeyValuePair<CorePhysicalKeyStatus, PhysicalKeyDataContainer> p in _physicalKeys)
            {
                action(p);
            }
        }

        protected void ClearPhysicalKeys()
        {
            _physicalKeys.Clear();
        }

        protected void ReportKeyEvent(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            _keyboardSink.ReportKeystroke(keyCode, isScanCode, isExtendedKey, isKeyReleased);
        }

        bool IVirtualKey.Update(CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
        {
            bool completelyHandled = false;
            PhysicalKeyDataContainer dataContainer;

            switch (eventType)
            {
                case CoreAcceleratorKeyEventType.KeyDown:
                case CoreAcceleratorKeyEventType.SystemKeyDown:
                    if(!_physicalKeys.TryGetValue(keyStatus, out dataContainer))
                    {
                        dataContainer = new PhysicalKeyDataContainer();
                        _physicalKeys.Add(keyStatus, dataContainer);
                    }
                    completelyHandled = OnPressed(keyStatus, dataContainer);
                    break;

                case CoreAcceleratorKeyEventType.KeyUp:
                case CoreAcceleratorKeyEventType.SystemKeyUp:
                    if (_physicalKeys.TryGetValue(keyStatus, out dataContainer))
                    {
                        _physicalKeys.Remove(keyStatus);

                        OnReleased(keyStatus, dataContainer);
                        //
                        // OnReleased may have called ClearPhysicalKeys()
                        //
                        if (0 == _physicalKeys.Count)
                        {
                            IVirtualKey vk = _keyboardState.UnregisterVirtualKey(_virtualKey);
                            Contract.Assert(object.ReferenceEquals(vk, this));
                        }
                    }
                    completelyHandled = true;
                    break;

                case CoreAcceleratorKeyEventType.Character:
                case CoreAcceleratorKeyEventType.UnicodeCharacter:
                case CoreAcceleratorKeyEventType.SystemCharacter:
                    if (_physicalKeys.TryGetValue(keyStatus, out dataContainer))
                    {
                        OnCharacter((char)virtualKey, eventType, keyStatus, dataContainer);
                    }
                    completelyHandled = true;
                    break;
            }

            return completelyHandled;
        }

        void IVirtualKey.Clear()
        {
            //
            // Simulate delease of all physical keys. The dance with the do-while loop and enumerator is
            // necessary because for some keys release of one physical keys releses all of them (Shift
            // is one of such keys).
            //
            do
            {
                PhysicalKeyDataContainer dataContainer;
                var en = _physicalKeys.GetEnumerator();

                en.MoveNext();

                CorePhysicalKeyStatus keyStatus = en.Current.Key;

                if (_physicalKeys.TryGetValue(keyStatus, out dataContainer))
                {
                    _physicalKeys.Remove(keyStatus);
                    keyStatus.IsKeyReleased = true;
                    OnReleased(keyStatus, dataContainer);
                }
            } while (0 != _physicalKeys.Count);

            IVirtualKey vk = _keyboardState.UnregisterVirtualKey(_virtualKey);
            Contract.Assert(object.ReferenceEquals(vk, this));
        }


        protected virtual bool OnPressed(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
            return false;
        }

        protected virtual void OnReleased(CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
        }

        protected virtual void OnCharacter(char character, CoreAcceleratorKeyEventType eventType, CorePhysicalKeyStatus keyStatus, PhysicalKeyDataContainer dataContainer)
        {
        }
    }
}

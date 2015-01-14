﻿namespace RdClient.Shared.Input
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public sealed class KeyboardState : IKeyboardState
    {
        private readonly IDictionary<VirtualKey, IVirtualKey> _vkStates;
        private readonly IDictionary<CorePhysicalKeyStatus, IVirtualKey> _characterKeys;
        private readonly IVirtualKeyFactory _vkFactory;

        public KeyboardState(IKeyboardCaptureSink keyboardSink)
        {
            Contract.Requires(null != keyboardSink);

            _vkStates = new SortedDictionary<VirtualKey, IVirtualKey>();
            _characterKeys = new SortedDictionary<CorePhysicalKeyStatus, IVirtualKey>(new ScanCodeComparer());
            _vkFactory = new VirtualKeyFactory(keyboardSink, this);
        }

        /// <summary>
        /// Update the internal state of the keyboard with a keyboard event reported by CoreDispatcher of the UI thread,
        /// and call the registered callback interface if necessary.
        /// </summary>
        /// <param name="eventType">Type of the keyboard event</param>
        /// <param name="virtualKey">Virtual key code of the keyboard key for which the event has been reported</param>
        /// <param name="keyStatus">Description of the physical key for which the event has been reported (more than one
        /// physical key may represent a virtual key code; physical keys are identified by the combination of a scan code
        /// and "extended key" flag.</param>
        /// <returns>True if the event has been completely processed; false otherwise.</returns>
        /// <remarks>The returned value is assigned to the Handled property of the event object reported
        /// by CoreDispather. Returning true for KeyDown and SysKeyDown events tells the dispatcher to
        /// report follow-up character events for character keys.</remarks>
        public bool UpdateState( CoreAcceleratorKeyEventType eventType, VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus )
        {
#if false
            switch (eventType)
            {
                case CoreAcceleratorKeyEventType.Character:
                case CoreAcceleratorKeyEventType.UnicodeCharacter:
                case CoreAcceleratorKeyEventType.SystemCharacter:
                    Debug.WriteLine("{0}: VirtualKey='{1}' | ScanCode={2} | IsExtendedKey={3} | IsMenuKeyDown={4}",
                        eventType,
                        (char)virtualKey,
                        keyStatus.ScanCode,
                        keyStatus.IsExtendedKey,
                        keyStatus.IsMenuKeyDown);
                    break;

                default:
                    Debug.WriteLine("{0}: VirtualKey={1} | ScanCode={2} | IsExtendedKey={3} | IsMenuKeyDown={4}",
                        eventType,
                        virtualKey,
                        keyStatus.ScanCode,
                        keyStatus.IsExtendedKey,
                        keyStatus.IsMenuKeyDown);
                    break;
            }
#endif

            bool completelyProcessed = false;

            switch(eventType)
            {
                case CoreAcceleratorKeyEventType.Character:
                case CoreAcceleratorKeyEventType.UnicodeCharacter:
                case CoreAcceleratorKeyEventType.SystemCharacter:
                    completelyProcessed = GetPhysicalKey(keyStatus).Update(eventType, virtualKey, keyStatus);
                    break;

                case CoreAcceleratorKeyEventType.DeadCharacter:
                case CoreAcceleratorKeyEventType.SystemDeadCharacter:
                    completelyProcessed = true;
                    break;

                default:
                    completelyProcessed = GetVirtualKey(virtualKey).Update(eventType, virtualKey, keyStatus);
                    break;
            }

            return completelyProcessed;
        }

        /// <summary>
        /// Release all keys and clear all internal state.
        /// </summary>
        public void Clear()
        {
            while(0 != _vkStates.Count)
            {
                var en = _vkStates.GetEnumerator();
                en.MoveNext();

                en.Current.Value.Clear();
                Contract.Assert(!_vkStates.ContainsKey(en.Current.Key));
            }
            Contract.Assert(0 == _characterKeys.Count);
        }

        void IKeyboardState.RegisterVirtualKey(VirtualKey virtualKey, IVirtualKey vk)
        {
            Contract.Requires(null != vk);
            _vkStates.Add(virtualKey, vk);
        }

        IVirtualKey IKeyboardState.UnregisterVirtualKey(VirtualKey virtualKey)
        {
            IVirtualKey removed;

            if (_vkStates.TryGetValue(virtualKey, out removed))
            {
                _vkStates.Remove(virtualKey);
            }
            else
            {
                removed = DummyVirtualKey.Dummy;
            }

            return removed;
        }

        void IKeyboardState.RegisterPhysicalKey(CorePhysicalKeyStatus keyStatus, IVirtualKey vk)
        {
            Contract.Requires(null != vk);
            IVirtualKey registeredKey;

            if (!_characterKeys.TryGetValue(keyStatus, out registeredKey))
                _characterKeys.Add(keyStatus, vk);
            else
                Contract.Assert(object.ReferenceEquals(registeredKey, vk));
        }

        IVirtualKey IKeyboardState.UnregisterPhysicalKey(CorePhysicalKeyStatus keyStatus)
        {
            IVirtualKey removed;

            if (_characterKeys.TryGetValue(keyStatus, out removed))
            {
                _characterKeys.Remove(keyStatus);
            }
            else
            {
                removed = DummyVirtualKey.Dummy;
            }

            return removed;
        }

        /// <summary>
        /// Get an event handler object for the virtual key code.
        /// </summary>
        /// <param name="virtualKey">Virtual key code</param>
        /// <returns>An IVirtualKey object that may handle keyboard events for the specified virtual key</returns>
        /// <remarks>If the class is tracking the virtual key, the actual handler associated with the key is retuned;
        /// otherwise, an instance of the NewVirtualKey class is returned that will start tracking the key
        /// upon receiving a "key down" event.</remarks>
        private IVirtualKey GetVirtualKey(VirtualKey virtualKey)
        {
            Contract.Ensures(null != Contract.Result<IVirtualKey>());

            IVirtualKey key;

            if (!_vkStates.TryGetValue(virtualKey, out key))
            {
                //
                // The key is not in the collection of pressed virtual keys;
                // create a "new virtual key" object that will add an actual 
                //
                key = new NewVirtualKey(virtualKey, _vkFactory);
            }

            return key;
        }

        private IVirtualKey GetPhysicalKey(CorePhysicalKeyStatus keyStatus)
        {
            Contract.Ensures(null != Contract.Result<IVirtualKey>());

            IVirtualKey key;

            if (!_characterKeys.TryGetValue(keyStatus, out key))
            {
                //
                // The physical key is not registered, but the method must return a valid object.
                // Return a dummy virtual key.
                //
                key = DummyVirtualKey.Dummy;
            }

            return key;
        }
    }
}

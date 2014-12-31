namespace RdClient.Shared.Input
{
    using RdClient.Shared.Helpers;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Windows.System;
    using Windows.UI.Core;

    public sealed class CoreWindowKeyboardCore : DisposableObject
    {
        private readonly IDictionary<uint, PressedKeyInfo> _pressedKeys;
        private IKeyboardCaptureSink _sink;

        private sealed class PressedKeyInfo
        {
            private static readonly ISet<VirtualKey> _characterKeys;
            private readonly VirtualKey _virtualKey;
            private readonly CorePhysicalKeyStatus _keyStatus;
            private SendMode _sentAs;
            private int _sentCharacter;

            [Flags]
            private enum SendMode : ushort
            {
                NotSent = 0,
                ScanCode = 1,
                Character = 2
            }

            public VirtualKey VirtualKey { get { return _virtualKey; } }
            public CorePhysicalKeyStatus KeyStatus { get { return _keyStatus; } }

            static PressedKeyInfo()
            {
                _characterKeys = new SortedSet<VirtualKey>()
                {
                    VirtualKey.A, VirtualKey.B, VirtualKey.C, VirtualKey.D, VirtualKey.E, VirtualKey.F,
                    VirtualKey.G, VirtualKey.H, VirtualKey.I, VirtualKey.J, VirtualKey.K, VirtualKey.L,
                    VirtualKey.M, VirtualKey.N, VirtualKey.O, VirtualKey.P, VirtualKey.Q, VirtualKey.R,
                    VirtualKey.S, VirtualKey.T, VirtualKey.U, VirtualKey.V, VirtualKey.W, VirtualKey.X,
                    VirtualKey.Y, VirtualKey.Z,
                    VirtualKey.Number0, VirtualKey.Number1, VirtualKey.Number2, VirtualKey.Number3,
                    VirtualKey.Number4, VirtualKey.Number5, VirtualKey.Number6, VirtualKey.Number7,
                    VirtualKey.Number8, VirtualKey.Number9,
                    VirtualKey.Multiply, VirtualKey.Add, VirtualKey.Separator, VirtualKey.Subtract,
                    VirtualKey.Decimal, VirtualKey.Divide, VirtualKey.Tab, VirtualKey.Enter
                };
            }

            public PressedKeyInfo(VirtualKey virtualKey, CorePhysicalKeyStatus keyStatus)
            {
                _virtualKey = virtualKey;
                _keyStatus = keyStatus;
                _sentAs = SendMode.NotSent;
                _sentCharacter = 0;
            }

            public void Down(IKeyboardCaptureSink sink)
            {
                Contract.Requires(null != sink);

                if(SendMode.NotSent == (_sentAs & SendMode.Character) && !_characterKeys.Contains(_virtualKey))
                {
                    //
                    // The key will not produce a character, send it as a scan code.
                    //
                    sink.ReportKeystroke((int)_keyStatus.ScanCode, true, _keyStatus.IsExtendedKey, false);
                    _sentAs = SendMode.ScanCode;
                }
            }

            public void Character(IKeyboardCaptureSink sink, uint keyCode)
            {
                Contract.Requires(null != sink);
                //
                // If the key hadn't been send as a scan code, send it as a character
                //
                if( SendMode.NotSent == (_sentAs & SendMode.ScanCode) )
                {
                    _sentCharacter = (int)keyCode;
                    _sentAs = SendMode.Character;
                    sink.ReportKeystroke(_sentCharacter, false, _keyStatus.IsExtendedKey, false);
                }
            }

            public void Up(IKeyboardCaptureSink sink)
            {
                Contract.Requires(null != sink);

                if (SendMode.Character == _sentAs)
                    sink.ReportKeystroke(_sentCharacter, false, _keyStatus.IsExtendedKey, true);
                else
                    sink.ReportKeystroke((int)_keyStatus.ScanCode, true, _keyStatus.IsExtendedKey, true);
                _sentAs = SendMode.NotSent;
            }
        }

        public CoreWindowKeyboardCore(IKeyboardCaptureSink sink)
        {
            Contract.Requires(null != sink);
            Contract.Ensures(null != _sink);

            _pressedKeys = new SortedDictionary<uint, PressedKeyInfo>();
            _sink = sink;
        }

        protected override void DisposeManagedState()
        {
            Contract.Assert(null != _sink);

            base.DisposeManagedState();
            //
            // Release all pressed keys in arbitrary order.
            // TODO: release modifier keys after alphanumeric ones.
            //
            foreach (var kc in _pressedKeys)
                kc.Value.Up(_sink);
            _pressedKeys.Clear();
            _sink = null;
        }

        public void ProcessKeyDown(VirtualKey virtualKey, CorePhysicalKeyStatus physicalKeyStatus)
        {
            Contract.Assert(null != _sink);
            //
            // 1. If the key is not marked as pressed, mark it.
            // 2. If the key will be followed up with a character code, don't send anything;
            //    otherwise, send the scan code.
            //
            PressedKeyInfo keyInfo;

            if (!_pressedKeys.TryGetValue(physicalKeyStatus.ScanCode, out keyInfo))
            {
                keyInfo = new PressedKeyInfo(virtualKey, physicalKeyStatus);
                _pressedKeys.Add(physicalKeyStatus.ScanCode, keyInfo);
            }

            keyInfo.Down(_sink);
        }

        public void ProcessKeyUp(VirtualKey virtualKey, CorePhysicalKeyStatus physicalKeyStatus)
        {
            Contract.Assert(null != _sink);
            //
            // 1. If the key is not marked as pressed, don't do anything.
            //
            PressedKeyInfo keyInfo;

            if(_pressedKeys.TryGetValue(physicalKeyStatus.ScanCode, out keyInfo))
            {
                _pressedKeys.Remove(physicalKeyStatus.ScanCode);
                keyInfo.Up(_sink);
            }
        }

        public void ProcessCharacterReceived(uint keyCode, CorePhysicalKeyStatus physicalKeyStatus)
        {
            Contract.Assert(null != _sink);
            PressedKeyInfo keyInfo;

            if (_pressedKeys.TryGetValue(physicalKeyStatus.ScanCode, out keyInfo))
            {
                keyInfo.Character(_sink, keyCode);
            }
        }
    }
}

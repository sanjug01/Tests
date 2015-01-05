﻿namespace RdClient.Shared.Input
{
    using System;

    public sealed class KeystrokeEventArgs : EventArgs
    {
        private readonly int _keyCode;
        private bool _isScanCode, _isExtendedKey, _isKeyReleased;

        public int KeyCode { get { return _keyCode; } }
        public bool IsScanCode { get { return _isScanCode; } }
        public bool IsExtendedKey { get { return _isExtendedKey; } }
        public bool IsKeyReleased { get { return _isKeyReleased; } }

        public KeystrokeEventArgs(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased)
        {
            _keyCode = keyCode;
            _isScanCode = isScanCode;
            _isExtendedKey = isExtendedKey;
            _isKeyReleased = isKeyReleased;
        }
    }
}
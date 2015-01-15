﻿namespace RdClient.Shared.Input
{
    /// <summary>
    /// Interface of an object that receives RDP keystrokes from CoreWindowKeyboardCore.
    /// </summary>
    public interface IKeyboardCaptureSink
    {
        void ReportKeystroke(int keyCode, bool isScanCode, bool isExtendedKey, bool isKeyReleased);
    }
}

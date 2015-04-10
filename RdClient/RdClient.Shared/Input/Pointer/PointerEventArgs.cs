namespace RdClient.Shared.Input.Pointer
{
    using System;

    public sealed class PointerEventArgs : EventArgs
    {
        private readonly PointerEventOld _pointerEvent;

        public PointerEventOld PointerEvent { get { return _pointerEvent; } }

        public PointerEventArgs(PointerEventOld pointerEvent)
        {
            _pointerEvent = pointerEvent;
        }
    }
}

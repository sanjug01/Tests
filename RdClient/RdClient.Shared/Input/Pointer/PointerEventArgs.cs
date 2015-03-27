namespace RdClient.Shared.Input.Pointer
{
    using System;

    public sealed class PointerEventArgs : EventArgs
    {
        private readonly PointerEvent _pointerEvent;

        public PointerEvent PointerEvent { get { return _pointerEvent; } }

        public PointerEventArgs(PointerEvent pointerEvent)
        {
            _pointerEvent = pointerEvent;
        }
    }
}

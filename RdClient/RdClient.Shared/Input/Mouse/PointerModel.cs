using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using MousePointer = System.Tuple<int, float, float>;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerManipulator : IPointerManipulator, IPointerEventConsumer
    {
        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        private Point _cursorPosition = new Point(0.0, 0.0);
        public Point CursorPosition { get { return _cursorPosition; } set { _cursorPosition = value; } }

        private Size _windowSize = new Size(0.0, 0.0);
        public Size WindowSize { get { return _windowSize; } set { _windowSize = value; } }

        public event EventHandler<MousePointer> MousePointerChanged;

        public PointerManipulator(ITimer timer)
        {
            _pointerConsumers[PointerType.Mouse] = new MouseMode(this);
            _pointerConsumers[PointerType.Pen] = new MouseMode(this);
            _pointerConsumers[PointerType.Touch] = new PointerMode(timer, this);
        }

        public void ChangeMousePointer(PointerEventType eventType)
        {
            MousePointerChanged(this, new MousePointer((int) eventType, (float)_cursorPosition.X, (float)_cursorPosition.Y));
        }

        public void ConsumeEvent(PointerEvent pointerEvent)
        {
            if (_lastPointerType != pointerEvent.PointerType)
            {
                _pointerConsumers[_lastPointerType].Reset();
            }

            _pointerConsumers[pointerEvent.PointerType].ConsumeEvent(pointerEvent);

            _lastPointerType = pointerEvent.PointerType;
        }

        public void Reset()
        {
            _pointerConsumers[_lastPointerType].Reset();
        }
    }
}

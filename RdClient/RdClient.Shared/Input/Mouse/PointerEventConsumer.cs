using RdClient.Shared.CxWrappers;
using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using MousePointer = System.Tuple<int, float, float>;


namespace RdClient.Shared.Input.Mouse
{
    public class PointerEventConsumer : IPointerEventConsumer
    {
        private Dictionary<PointerType, IPointerEventConsumer> _pointerConsumers = new Dictionary<PointerType,IPointerEventConsumer>();
        private PointerType _lastPointerType = PointerType.Mouse;

        //private Point _cursorPosition = new Point(0.0, 0.0);
        //public Point CursorPosition { 
        //    get 
        //    { 
        //        return _cursorPosition; 
        //    } 
        //    set 
        //    {
        //        _cursorPosition.X = Math.Min(Math.Max(0.0, value.X), _windowSize.Width); 
        //        _cursorPosition.Y = Math.Min(Math.Max(0.0, value.Y), _windowSize.Height);
        //    } 
        //}

        //private Size _windowSize = new Size(0.0, 0.0);
        //public Size WindowSize { set { _windowSize = value; } }

        public PointerEventConsumer(ITimer timer, IPointerManipulator pointerManipulator)
        {
            _pointerConsumers[PointerType.Mouse] = new MouseMode(pointerManipulator);
            _pointerConsumers[PointerType.Pen] = new MouseMode(pointerManipulator);
            _pointerConsumers[PointerType.Touch] = new PointerMode(timer, pointerManipulator);
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

using RdClient.Shared.CxWrappers;
using RdClient.Shared.Models;
using System;

namespace RdClient.Shared.Input.Pointer
{
    public class MultiTouchConsumer : IPointerEventConsumer
    {
        private IPointerRoutedEventProperties _masterTouch;
        private IPointerRoutedEventProperties _lastTouch;
        private IRemoteSessionControl _sessionControl;
        private IPointerPosition _pointerPosition;        

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public MultiTouchConsumer(IRemoteSessionControl sessionControl, IPointerPosition pointerPosition)
        {
            _sessionControl = sessionControl;
            _pointerPosition = pointerPosition;
        }

        private void InternalConsume(IPointerRoutedEventProperties pointerEvent)
        {
            // the time-stamp delta is relative to the first touch ever
            ulong delta = pointerEvent.Timestamp - _masterTouch.Timestamp;

            // don't send duplicate position updates
            if (pointerEvent.Action == PointerEventAction.PointerMoved &&
                _lastTouch != null && 
                _lastTouch.Position == pointerEvent.Position)
            {
                return;
            }

            // if the position to the up event is not the same as the last recorded position, 
            // update the position before sending the up event
            if ((pointerEvent.Action == PointerEventAction.PointerReleased ||
                 pointerEvent.Action == PointerEventAction.PointerCancelled) &&                 
                _lastTouch != null && 
                _lastTouch.Position != pointerEvent.Position)
            {
                _sessionControl.SendTouchAction(TouchEventType.Update, pointerEvent.PointerId, pointerEvent.Position, delta);
            }

            // touch events have a location indicator hint which needs the correct position
            _pointerPosition.ViewportPosition = pointerEvent.Position;

            TouchEventType touchType = TouchEventType.Down;

            switch(pointerEvent.Action)
            {
                case PointerEventAction.PointerPressed:
                    touchType = TouchEventType.Down;
                    break;
                case PointerEventAction.PointerMoved:
                    touchType = TouchEventType.Update;
                    break;
                case PointerEventAction.PointerReleased:
                case PointerEventAction.PointerCancelled:
                    touchType = TouchEventType.Up;
                    break;
                default:
                    throw new InvalidOperationException("trying to convert unknown PointerEventAction");
            }

            _sessionControl.SendTouchAction(touchType, pointerEvent.PointerId, _pointerPosition.SessionPosition, delta);
        }


        void IPointerEventConsumer.Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IPointerRoutedEventProperties)
            {
                IPointerRoutedEventProperties prep = pointerEvent as IPointerRoutedEventProperties;
                if (_masterTouch == null && pointerEvent.Action == PointerEventAction.PointerPressed)
                {
                    _masterTouch = new PointerRoutedEventArgsCopy(prep);
                }

                if (_masterTouch != null)
                {
                    this.InternalConsume(prep);
                }

                _lastTouch = new PointerRoutedEventArgsCopy(prep);
            }

            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        void IPointerEventConsumer.Reset()
        {
            // don't reset the master touch because it may crash the protocol stack o_O
            _lastTouch = null;
        }
    }
}

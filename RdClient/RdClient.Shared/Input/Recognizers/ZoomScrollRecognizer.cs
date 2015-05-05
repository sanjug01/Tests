using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using System;
using System.Diagnostics;

namespace RdClient.Shared.Input.Recognizers
{
    public class ZoomScrollRecognizer : IZoomScrollRecognizer
    {
        private ITimer _timer;
        private bool _expired;
        private PointerEventAction _stateAction;
        private ZoomScrollType _stateType;

        public event EventHandler<IZoomScrollEvent> ZoomScrollEvent;

        public ZoomScrollRecognizer(ITimer timer)
        {
            _timer = timer;
            _expired = true;
            _stateAction = PointerEventAction.ZoomScrollCompleted;
            _stateType = ZoomScrollType.Scroll;
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        private void EmitPanZoomScrollEvent(IZoomScrollEvent e)
        {
            if(ZoomScrollEvent != null)
            {
                ZoomScrollEvent(this, e);
            }
        }

        public void Consume(IPointerEventBase pointerEvent)
        {
            if(pointerEvent is IManipulationRoutedEventProperties)
            {
                IManipulationRoutedEventProperties manipulation = (IManipulationRoutedEventProperties)pointerEvent;
                

                if (manipulation.Action == PointerEventAction.ManipulationStarted)
                {
                    _expired = false;
                    _timer.Start(() => _expired = true, TimeSpan.FromMilliseconds(100), false);
                }
                else if (manipulation.Action == PointerEventAction.ManipulationCompleted)
                {
                    _expired = true;
                    _timer.Stop();
                    _stateAction = PointerEventAction.ZoomScrollCompleted;
                    EmitPanZoomScrollEvent(new ZoomScrollEvent(_stateAction, _stateType, manipulation.Cummulative, manipulation.IsInertial, manipulation.Position));

                }
                else if ((manipulation.Action == PointerEventAction.ManipulationDelta || manipulation.Action == PointerEventAction.ManipulationInertiaStarting) && _expired == true)
                {
                    if (_stateAction != PointerEventAction.ZoomScrollUpdating)
                    {
                        _stateType = GuessType(manipulation);
                        _stateAction = PointerEventAction.ZoomScrollUpdating;

                        EmitPanZoomScrollEvent(new ZoomScrollEvent(PointerEventAction.ZoomScrollStarted, _stateType, manipulation.Cummulative, manipulation.IsInertial, manipulation.Position));
                    }
                    else
                    {
                        EmitPanZoomScrollEvent(new ZoomScrollEvent(_stateAction, _stateType, manipulation.Delta, manipulation.IsInertial, manipulation.Position));
                    }
                }

                if (ConsumedEvent != null)
                {
                    ConsumedEvent(this, pointerEvent);
                }
            }
        }

        private ZoomScrollType GuessType(IManipulationRoutedEventProperties manipulation)
        {
            if (Math.Abs(manipulation.Velocities.Expansion) > 0.04)
            {
                return ZoomScrollType.ZoomPan;
            }
            else
            {
                if(Math.Abs(RdMath.Angle(manipulation.Cummulative.Translation)) > 45.0)
                {
                    return ZoomScrollType.Scroll;
                }
                else
                {
                    return ZoomScrollType.HScroll;
                }
            }
        }

        void IPointerEventConsumer.Reset()
        {
            _timer.Stop();
        }
    }
}

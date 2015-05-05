using RdClient.Shared.Helpers;
using RdClient.Shared.Input.Pointer;
using System;
using System.Diagnostics;
using Windows.Foundation;

namespace RdClient.Shared.Input.Recognizers
{

    public class TapTimer
    {
        private bool _expired;
        public bool IsExpired { get { return _expired; } }

        private ITimer _timer;
        private TimeSpan _period;

        public TapTimer(ITimer timer, int period)
        {
            _timer = timer;
            _period = TimeSpan.FromMilliseconds(period);
            _expired = true;
        }

        public void Reset(Action action)
        {
            _timer.Stop();
            _expired = false;

            _timer.Start(() =>
            {
                _expired = true;
                if(action != null)
                {
                    action();
                }
            }, _period, false);
        }

        public void Stop()
        {
            _timer.Stop();
        }
    }

    public class TapRecognizer : ITapRecognizer
    {
        public event EventHandler<IPointerEventBase> ConsumedEvent;
        public event EventHandler<ITapEvent> TapEvent;

        private TapTimer _timer;

        private int _taps;
        private int _fingersDown;
        private Point _position;
        private bool _holding;

        public TapRecognizer(ITimer timer)
        {
            _timer = new TapTimer(timer, 300);
        }

        private void EmitTapEvent(TapEventType type)
        {
            if(TapEvent != null)
            {
                TapEvent(this, new TapEvent(PointerEventAction.Tapped, _position, type));
            }
        }

        public void Consume(IPointerEventBase pointerEvent)
        {
            switch(pointerEvent.Action)
            {
                case PointerEventAction.PointerPressed:
                    _fingersDown++;
                    _position = pointerEvent.Position;
                    _timer.Reset(TapTimerExpired);
                    break;
                case PointerEventAction.ManipulationDelta:
                    if (_fingersDown > 0 && 
                        _taps > 0 && 
                        RdMath.Distance(((IManipulationRoutedEventProperties) pointerEvent).Delta.Translation) > GlobalConstants.TouchMoveThreshold)
                    { 
                        EmitTapEvent(TapEventType.TapMovingStarted);
                    }
                    break;
                case PointerEventAction.PointerReleased:
                    if(_timer.IsExpired == false)
                    {
                        _taps++;
                    }

                    if(_holding)
                    {
                        EmitTapEvent(TapEventType.HoldingCompleted);
                        _holding = false;
                    }

                    _fingersDown--;
                    break;
                case PointerEventAction.PointerCancelled:
                    if (_holding)
                    {
                        EmitTapEvent(TapEventType.HoldingCancelled);
                        _holding = false;
                    }

                    Reset();
                    break;
            }

            if(ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        private void TapTimerExpired()
        {
            if(_fingersDown == 0)
            {
                if(_taps == 1)
                {
                    EmitTapEvent(TapEventType.Tap);
                }
                else if(_taps > 1)
                {
                    EmitTapEvent(TapEventType.DoubleTap);
                }
            }
            else if(_fingersDown == 1)
            {
                _holding = true;

                if (_taps == 0)
                {
                    EmitTapEvent(TapEventType.HoldingStarted);
                }
                else if(_taps > 0)
                {
                    EmitTapEvent(TapEventType.TapHoldingStarted);
                }
            }

            _taps = 0;
        }

        public void Reset()
        {
            _taps = 0;
            _fingersDown = 0;
            _holding = false;
            _timer.Stop();
        }
    }
}

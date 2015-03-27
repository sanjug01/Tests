using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using Windows.Foundation;

namespace RdClient.Shared.Input.Pointer
{
    public class DoubleClickTimer
    {
        public enum ClickTimerType
        {
            LeftClick,
            RightClick
        }

        private ITimer _timer;
        private double _interval;
        private Dictionary<ClickTimerType, Action<PointerEvent>> _actions;
        private ClickTimerType _timerType;

        private bool _expired;
        public bool IsExpired(ClickTimerType timerType)
        {
            bool matchingType = (timerType == _timerType);

            if (matchingType == false)
            {
                return true;
            }
            else
            {
                return _expired;
            }
        }

        public DoubleClickTimer(ITimer timer, double interval)
        {
            _actions = new Dictionary<ClickTimerType, Action<PointerEvent>>();
            _timer = timer;
            _interval = interval;
            _expired = true;
        }

        public void AddAction(ClickTimerType timerType, Action<PointerEvent> action)
        {
            _actions[timerType] = action;
        }

        public void Reset(ClickTimerType timerType, PointerEvent pointerEvent)
        {
            _timerType = timerType;
            _expired = false;
            _timer.Stop();
            _timer.Start(() => { _expired = true; _actions[timerType](pointerEvent); }, TimeSpan.FromMilliseconds(_interval), false);
        }

        public void Stop()
        {
            _expired = true;
            _timer.Stop();
        }
    }
}

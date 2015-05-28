﻿using RdClient.Shared.Helpers;
using RdClient.Shared.Models;
using RdClient.Shared.Navigation.Extensions;
using System;
using System.Collections.Generic;
using Windows.Devices.Input;

namespace RdClient.Shared.Input.Pointer
{
    public class PointerEventDispatcher : IPointerEventConsumer
    {
        private PointerDeviceDispatcher _deviceDispatcher;
        private PointerVisibilityConsumer _visibilityConsumer;

        public ConsumptionModeType ConsumptionMode
        {
            set
            {
                _deviceDispatcher.ConsumptionMode = value;

                _visibilityConsumer.ConsumptionMode = value;
            }
        }

        public event EventHandler<IPointerEventBase> ConsumedEvent;

        public PointerEventDispatcher(ITimerFactory timerFactory, IRemoteSessionControl sessionControl, IPointerPosition pointerPosition, IDeferredExecution dispatcher)
        {
            _deviceDispatcher = new PointerDeviceDispatcher(timerFactory, sessionControl, pointerPosition, dispatcher);
            _visibilityConsumer = new PointerVisibilityConsumer(timerFactory, sessionControl, pointerPosition, dispatcher);
        }

        public void Consume(IPointerEventBase pointerEvent)
        {

            if(pointerEvent.Action == PointerEventAction.PointerEntered || pointerEvent.Action== PointerEventAction.PointerExited)
            {
                _visibilityConsumer.Consume(pointerEvent);
            }
            else
            {
                _deviceDispatcher.Consume(pointerEvent);
            }
             
            if (ConsumedEvent != null)
            {
                ConsumedEvent(this, pointerEvent);
            }
        }

        public void Reset()
        {
            _deviceDispatcher.Reset();
        }
    }
}

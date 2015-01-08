﻿using RdClient.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace RdClient.Shared.Converters
{
    public class PointerEventConverter
    {
        public static PointerType PointerTypeConverter(PointerDeviceType type)
        {
            PointerType rtype = PointerType.Unknown;

            switch(type)
            {
                case PointerDeviceType.Mouse:
                    rtype = PointerType.Mouse;
                    break;
                case PointerDeviceType.Pen:
                    rtype = PointerType.Pen;
                    break;
                case PointerDeviceType.Touch:
                    rtype = PointerType.Touch;
                    break;
                default:
                    rtype = PointerType.Unknown;
                    break;
            }

            return rtype;
        }

        public static PointerEvent ManipulationStartedArgsConverter(ManipulationStartedRoutedEventArgs args)
        {
            PointerEvent pe = new PointerEvent(args.Position, false, args.Cumulative.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEvent ManipulationInertiaStartingArgsConverter(ManipulationInertiaStartingRoutedEventArgs args)
        {
            PointerEvent pe = new PointerEvent(new Point(0.0, 0.0), true, args.Delta.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEvent ManipulationDeltaArgsConverter(ManipulationDeltaRoutedEventArgs args)
        {
            PointerEvent pe = new PointerEvent(args.Position, args.IsInertial, args.Delta.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEvent ManipulationCompletedArgsConverter(ManipulationCompletedRoutedEventArgs args)
        {
            PointerEvent pe = new PointerEvent(args.Position, false, args.Cumulative.Translation, false, false, PointerTypeConverter(args.PointerDeviceType), 0);

            return pe;
        }

        public static PointerEvent PointerArgsConverter(UIElement receiver, PointerRoutedEventArgs args)
        {
            PointerPoint ppoint = args.GetCurrentPoint(receiver);
            Point position = new Point(ppoint.Position.X, ppoint.Position.Y);
            PointerEvent pe = new PointerEvent(
                position, false, new Point(0.0, 0.0),
                ppoint.Properties.IsLeftButtonPressed, ppoint.Properties.IsRightButtonPressed,
                PointerTypeConverter(ppoint.PointerDevice.PointerDeviceType), ppoint.PointerId);

            return pe;
        }
    }
}